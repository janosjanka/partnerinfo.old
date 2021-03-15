// Copyright (c) János Janka. All rights reserved.

using System;
using System.Threading;
using System.Threading.Tasks;
using Partnerinfo.Logging;

namespace Partnerinfo.Project.Actions
{
    public sealed class AuthenticateActionActivity : IActionActivity
    {
        public sealed class Options
        {
            /// <summary>
            /// Returns true if a valid password must be included in the request
            /// </summary>
            public string CheckPassword { get; set; }
        }

        /// <summary>
        /// Called by the workflow runtime to execute an activity.
        /// </summary>
        /// <param name="context">The <see cref="ActionActivityContext"/> to associate with this activity and execution.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/> used to propagate notifications that the operation should be canceled.</param>
        /// <returns>
        /// The <see cref="ActionActivityResult"/> of the run task, which determines whether the activity remains in the executing state, or transitions to the closed state.
        /// </returns>
        public Task<ActionActivityResult> ExecuteAsync(ActionActivityContext context, CancellationToken cancellationToken)
        {
            if (!context.ContactExists)
            {
                return Task.FromResult(context.CreateResult(ActionActivityStatusCode.Forbidden));
            }
            if (context.ContactState == ObjectState.Deleted)
            {
                return Task.FromResult(context.CreateResult(ActionActivityStatusCode.Failed));
            }
            if ((context.AuthTicket == null) || (context.Contact.Id != context.AuthTicket.Id))
            {
                context.AnonymId = GuidUtility.NewSequentialGuid();
            }

            context.AuthTicket = AuthUtility.Create(context.Contact);

            return Task.FromResult(context.CreateResult(ActionActivityStatusCode.Success));
        }
    }
}
