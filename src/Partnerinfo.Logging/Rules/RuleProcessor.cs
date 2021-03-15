// Copyright (c) János Janka. All rights reserved.

using System;
using System.Threading;
using System.Threading.Tasks;

namespace Partnerinfo.Logging.Rules
{
    public class RuleProcessor : IRuleProcessor
    {
        /// <summary>
        /// Called by the runtime to apply a collection of rules to the specified event.
        /// </summary>
        /// <param name="context">The <see cref="RuleProContext"/> to associate with this event and execution.</param>
        /// <param name="cancellationToken">A token to observe while waiting for the task to complete.</param>
        /// <returns>
        /// The <see cref="RuleProResult"/> of the run task, which determines whether the context remains in the executing state, or transitions to the closed state.
        /// </returns>
        public virtual async Task ExecuteAsync(RuleProContext context, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            if (context == null)
            {
                throw new ArgumentNullException(nameof(context));
            }

            foreach (var action in context.RuleItem.Actions)
            {
                switch (action.Code)
                {
                    case RuleActionCode.Categorize:
                        {
                            int categoryId;
                            if (int.TryParse(action.Value, out categoryId))
                            {
                                var eventManager = context.Resolve<EventManager>();
                                await eventManager.BulkSetCategoriesAsync(context.EventItem.User.Id, new[] { context.EventItem.Id }, categoryId, cancellationToken);
                            }
                        }
                        break;
                    case RuleActionCode.Remove:
                        {
                            var eventManager = context.Resolve<EventManager>();
                            await eventManager.BulkDeleteAsync(context.EventItem.User.Id, new[] { context.EventItem.Id }, cancellationToken);
                        }
                        break;
                }
            }
        }
    }
}
