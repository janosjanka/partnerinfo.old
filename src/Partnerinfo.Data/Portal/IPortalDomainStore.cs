// Copyright (c) János Janka. All rights reserved.

using System.Threading;
using System.Threading.Tasks;

namespace Partnerinfo.Portal
{
    /// <summary>
    /// Provides facilities for querying and working with system data as objects.
    /// </summary>
    public interface IPortalDomainStore : IPortalStore
    {
        /// <summary>
        /// Finds and returns a portal, if any, which has the specified normalized domain.
        /// </summary>
        /// <param name="normalizedDomain">The normalized domain to search for.</param>
        /// <param name="fields">The related fields to include in the query results.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken" /> used to propagate notifications that the operation should be canceled.</param>
        /// <returns>
        /// The <see cref="Task" /> that represents the asynchronous operation, containing the portal matching the specified <paramref name="normalizedDomain" /> if it exists.
        /// </returns>
        Task<PortalItem> FindByDomainAsync(string normalizedDomain, PortalField fields, CancellationToken cancellationToken);
    }
}
