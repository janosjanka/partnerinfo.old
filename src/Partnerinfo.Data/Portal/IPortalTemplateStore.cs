// Copyright (c) János Janka. All rights reserved.

using System;
using System.Threading;
using System.Threading.Tasks;

namespace Partnerinfo.Portal
{
    /// <summary>
    /// Provides facilities for querying and working with system data as objects.
    /// </summary>
    public interface IPortalTemplateStore : IDisposable
    {
        /// <summary>
        /// Finds and returns a portal template, if any, which has the specified name.
        /// </summary>
        /// <param name="name">The name of the template to be found.</param>
        /// <param name="cancellationToken">A token to observe while waiting for the task to complete.</param>
        /// <returns>
        /// A task that represents the asynchronous operation.
        /// </returns>
        Task<PortalTemplate> GetTemplateByNameAsync(string name, CancellationToken cancellationToken);
    }
}
