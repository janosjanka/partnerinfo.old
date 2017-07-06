// Copyright (c) János Janka. All rights reserved.

using System.Threading;
using System.Threading.Tasks;

namespace Partnerinfo.Input
{
    public interface ICommandInvoker
    {
        /// <summary>
        /// Called by the runtime to execute a command.
        /// </summary>
        Task<CommandResult> InvokeAsync(string command, string htmlContent, string textContent, CancellationToken cancellationToken);
    }
}
