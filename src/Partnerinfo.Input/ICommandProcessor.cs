// Copyright (c) János Janka. All rights reserved.

using System.Threading;
using System.Threading.Tasks;

namespace Partnerinfo.Input
{
    public interface ICommandProcessor
    {
        /// <summary>
        /// Called by the runtime to execute a command.
        /// </summary>
        /// <param name="context">The <see cref="CommandContext" /> to associate with this command and execution.</param>
        /// <param name="cancellationToken">A token to observe while waiting for the task to complete.</param>
        /// <returns>
        /// The <see cref="CommandResult" /> of the run task, which determines whether the command remains in the executing state, or transitions to the closed state.
        /// </returns>
        Task<CommandResult> ExecuteAsync(CommandContext context, CancellationToken cancellationToken);
    }
}
