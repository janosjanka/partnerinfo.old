// Copyright (c) János Janka. All rights reserved.

using System.Threading;
using System.Threading.Tasks;
using Partnerinfo.Logging;

namespace Partnerinfo.Project.Actions
{
    public sealed class UnregisterActionActivity : IActionActivity
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
            var projectManager = context.Resolve<ProjectManager>();
            if (!context.ContactExists || context.ContactState == ObjectState.Deleted)
            {
                return context.CreateResult(ActionActivityStatusCode.Failed);
            }
            var validationResult = await projectManager.RemoveContactAsync(context.Project, context.Contact, cancellationToken);
            if (validationResult.Succeeded)
            {
                context.ContactState = ObjectState.Deleted;
                return context.CreateResult(ActionActivityStatusCode.Success);
            }
            return context.CreateResult(ActionActivityStatusCode.Failed);
        }
    }
}
