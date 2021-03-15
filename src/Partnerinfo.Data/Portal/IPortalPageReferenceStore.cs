// Copyright (c) János Janka. All rights reserved.

using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Partnerinfo.Portal
{
    public interface IPortalPageReferenceStore : IPortalPageStore
    {
        /// <summary>
        /// Gets a list of references for the specified <paramref name="page" />.
        /// </summary>
        /// <param name="portal">The portal which owns the page.</param>
        /// <param name="page">The page to be found.</param>
        /// <param name="cancellationToken">A token to observe while waiting for the task to complete.</param>
        /// <returns>
        /// A task that represents the asynchronous operation.
        /// </returns>
        Task<ListResult<ReferenceItem>> GetPageReferencesAsync(PortalItem portal, PageItem page, CancellationToken cancellationToken);

        /// <summary>
        /// Sets a list of references for the specified <paramref name="page" />.
        /// </summary>
        /// <param name="portal">The portal which owns the page.</param>
        /// <param name="page">The page to update.</param>
        /// <param name="references">The references to add.</param>
        /// <param name="cancellationToken">A token to observe while waiting for the task to complete.</param>
        /// <returns>
        /// A task that represents the asynchronous operation.
        /// </returns>
        Task SetPageReferencesAsync(PortalItem portal, PageItem page, IEnumerable<ReferenceItem> references, CancellationToken cancellationToken);
    }
}