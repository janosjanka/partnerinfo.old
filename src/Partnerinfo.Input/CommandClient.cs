// Copyright (c) János Janka. All rights reserved.

using System.Threading;
using System.Threading.Tasks;

namespace Partnerinfo.Input
{
    public abstract class CommandClient
    {
        /// <summary>
        /// Asynchronously authorizes the client.
        /// </summary>
        /// <returns>
        /// A task that represents the asynchronous operation.
        /// </returns>
        public abstract Task AuthorizeAsync(CancellationToken cancellationToken);

        /// <summary>
        /// Asynchronusly executes the client.
        /// </summary>
        /// <returns>
        /// A task that represents the asynchronous operation.
        /// </returns>
        public abstract Task ExecuteAsync(CancellationToken cancellationToken);
    }
}
