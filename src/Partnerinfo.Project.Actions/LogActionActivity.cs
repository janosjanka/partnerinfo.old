// Copyright (c) János Janka. All rights reserved.

using System.Threading;
using System.Threading.Tasks;
using Partnerinfo.Logging;

namespace Partnerinfo.Project.Actions
{
    public sealed class LogActionActivity : IActionActivity
    {
        public sealed class Options
        {
            /// <summary>
            /// Adds anonymous events
            /// </summary>
            public bool Anonymous { get; set; }
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
            var logManager = context.Resolve<LogManager>();
            var options = context.Action.Options?.ToObject<Options>();
            if (options == null || (!options.Anonymous && !context.ContactExists))
            {
                return context.CreateResult(ActionActivityStatusCode.Failed);
            }
            if (context.ContactExists)
            {
                context.Event.Contact = context.Contact;
                context.Event.ContactState = context.ContactState;
            }
            else
            {
                context.Event.Contact = null;
                context.Event.ContactState = ObjectState.Unchanged;
            }
            await logManager.LogAsync(context.Event, context.Project.Owners, cancellationToken);
            return context.CreateResult(ActionActivityStatusCode.Success);
        }
    }
}
