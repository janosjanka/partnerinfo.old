// Copyright (c) János Janka. All rights reserved.

using System;
using System.Threading;
using System.Threading.Tasks;

namespace Partnerinfo.Project.Actions
{
    public sealed class WorkflowInvoker
    {
        /// <summary>
        /// Gets or sets the action activity for this <see cref="WorkflowInvoker" />.
        /// </summary>
        /// <value>
        /// The action activity.
        /// </value>
        public IActionActivity Action { get; }

        /// <summary>
        /// Gets or sets a dependency injection container for this <see cref="WorkflowInvoker" />.
        /// </summary>
        /// <value>
        /// The dependency injection container.
        /// </value>
        public IServiceResolver Resolver { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="WorkflowInvoker" /> class.
        /// </summary>
        /// <param name="action">The action to execute.</param>
        /// <exception cref="System.ArgumentNullException"></exception>
        public WorkflowInvoker(IActionActivity action)
        {
            if (action == null)
            {
                throw new ArgumentNullException(nameof(action));
            }
            Action = action;
        }

        /// <summary>
        /// Called by the workflow runtime to execute an activity.
        /// </summary>
        /// <param name="context">The <see cref="ActionActivityContext"/> to associate with this activity and execution.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/> used to propagate notifications that the operation should be canceled.</param>
        /// <returns>
        /// The <see cref="ActionActivityResult"/> of the run task, which determines whether the activity remains in the executing state, or transitions to the closed state.
        /// </returns>
        public async Task<ActionActivityResult> InvokeAsync(ActionActivityContext context, CancellationToken cancellationToken)
        {
            if (context == null)
            {
                throw new ArgumentNullException(nameof(context));
            }
            if (context.Resolver == null)
            {
                context.Resolver = Resolver;
            }
            return await Action.ExecuteAsync(context, cancellationToken);
        }
    }
}
