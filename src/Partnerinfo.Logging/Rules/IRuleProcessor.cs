// Copyright (c) János Janka. All rights reserved.

using System.Threading;
using System.Threading.Tasks;

namespace Partnerinfo.Logging.Rules
{
    public interface IRuleProcessor
    {
        /// <summary>
        /// Called by the runtime to apply a collection of rules to the specified event.
        /// </summary>
        /// <param name="context">The <see cref="RuleProContext"/> to associate with this event and execution.</param>
        /// <param name="cancellationToken">A token to observe while waiting for the task to complete.</param>
        /// <returns>
        /// The <see cref="RuleProResult"/> of the run task, which determines whether the context remains in the executing state, or transitions to the closed state.
        /// </returns>
        Task ExecuteAsync(RuleProContext context, CancellationToken cancellationToken);
    }
}
