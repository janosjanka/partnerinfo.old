// Copyright (c) János Janka. All rights reserved.

using System.Threading;
using System.Threading.Tasks;
using Partnerinfo.Logging;
using Partnerinfo.Project.Mail;

namespace Partnerinfo.Project.Actions
{
    public sealed class SendMailActionActivity : IActionActivity
    {
        public sealed class Options
        {
            public int Id { get; set; }

            public PropertyDictionary Placeholders { get; set; }
        }

        /// <summary>
        /// Called by the workflow runtime to execute an activity.
        /// </summary>
        /// <param name="context">The <see cref="ActionActivityContext"/> to associate with this activity and execution.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/> used to propagate notifications that the operation should be canceled.</param>
        /// <returns>
        /// The <see cref="ActionActivityResult"/> of the run task, which determines whether the activity remains in the executing state, or transitions to the closed state.
        /// </returns>
        public async Task<ActionActivityResult> ExecuteAsync(ActionActivityContext context, CancellationToken cancellationToken)
        {
            var projectManager = context.Resolve<ProjectManager>();
            var mailService = context.Resolve<IMailClientService>();

            if (!context.ContactExists || context.ContactState == ObjectState.Deleted)
            {
                return context.CreateResult(ActionActivityStatusCode.Failed);
            }
            var options = context.Action.Options?.ToObject<Options>();
            if (options == null)
            {
                return context.CreateResult(ActionActivityStatusCode.Failed);
            }
            var message = await projectManager.GetMailMessageByIdAsync(options.Id, MailMessageField.Body, cancellationToken);
            if (message == null)
            {
                return context.CreateResult(ActionActivityStatusCode.Failed);
            }
            var project = await projectManager.FindByIdAsync(context.Project.Id, cancellationToken);
            if (project == null)
            {
                return context.CreateResult(ActionActivityStatusCode.Failed);
            }
            var header = new MailMessageHeader();
            header.To.Add(context.Contact.Id);
            var invitation = context.Properties["Invitation"] as ProjectInvitation;
            if (invitation != null)
            {
                header.From = invitation.From;
                header.Placeholders[ActionResources.Invitation] = invitation;
            }
            await mailService.SendAsync(project, header, message, cancellationToken);
            return context.CreateResult(ActionActivityStatusCode.Success);
        }
    }
}
