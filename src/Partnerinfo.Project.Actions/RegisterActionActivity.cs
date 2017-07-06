// Copyright (c) János Janka. All rights reserved.

using System.Threading;
using System.Threading.Tasks;
using Partnerinfo.Logging;

namespace Partnerinfo.Project.Actions
{
    public sealed class RegisterActionActivity : IActionActivity
    {
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
            if (context.Contact == null || context.ContactState == ObjectState.Deleted)
            {
                return context.CreateResult(ActionActivityStatusCode.Failed);
            }
            ValidationResult validationResult;
            var projectManager = context.Resolve<ProjectManager>();
            if (context.ContactExists)
            {
                validationResult = await projectManager.ReplaceContactAsync(context.Project, context.Contact, context.Contact, cancellationToken);
                if (validationResult.Succeeded)
                {
                    context.ContactState = ObjectState.Modified;
                }
            }
            else
            {
                validationResult = await projectManager.AddContactAsync(context.Project, context.Contact, cancellationToken);
                if (validationResult.Succeeded)
                {
                    context.ContactState = ObjectState.Added;
                }
            }
            return context.CreateResult(validationResult.Succeeded ? ActionActivityStatusCode.Success : ActionActivityStatusCode.Failed);
        }
    }
}
/*
var invitation = context.Properties["Invitation"] as ProjectInvitation;
if (invitation != null && invitation.From != null)
{
    var sponsor = await projectManager.GetContactByMailAsync(context.Project.Id, invitation.From.Email.Address);
    if (sponsor != null)
    {
        context.Contact.SponsorId = sponsor.Id;
        context.Contact.Sponsor = sponsor;
    }
}
*/
