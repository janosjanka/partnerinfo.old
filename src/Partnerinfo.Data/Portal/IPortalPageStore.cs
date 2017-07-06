// Copyright (c) János Janka. All rights reserved.

using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Partnerinfo.Portal
{
    public interface IPortalPageStore : IPortalStore
    {
        /// <summary>
        /// Gets the URI for the specified <paramref name="page" />.
        /// </summary>
        /// <param name="portal">The portal which owns the page.</param>
        /// <param name="page">The page whose URI should be returned.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken" /> used to propagate notifications that the operation should be canceled.</param>
        /// <returns>
        /// The task object representing the asynchronous operation.
        /// </returns>
        Task<string> GetPageUriAsync(PortalItem portal, PageItem page, CancellationToken cancellationToken);

        /// <summary>
        /// Sets the <paramref name="normalizedUri" /> for a <paramref name="page" />.
        /// </summary>
        /// <param name="portal">The portal which owns the page.</param>
        /// <param name="page">The page whose URI should be set.</param>
        /// <param name="normalizedUri">The normalized URI to set.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken" /> used to propagate notifications that the operation should be canceled.</param>
        /// <returns>
        /// The task object representing the asynchronous operation.
        /// </returns>
        Task SetPageUriAsync(PortalItem portal, PageItem page, string normalizedUri, CancellationToken cancellationToken);

        /// <summary>
        /// Gets the master page for the specified <paramref name="page" />.
        /// </summary>
        /// <param name="portal">The portal whose master page should be returned.</param>
        /// <param name="page">The page whose master page should be returned.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken" /> used to propagate notifications that the operation should be canceled.</param>
        /// <returns>
        /// The task object containing the results of the asynchronous operation, the master page for the specified <paramref name="page" />.
        /// </returns>
        Task<PageItem> GetPageMasterAsync(PortalItem portal, PageItem page, CancellationToken cancellationToken);

        /// <summary>
        /// Sets the <paramref name="masterPage" /> for a <paramref name="page" />.
        /// </summary>
        /// <param name="portal">The portal whose master page should be set.</param>
        /// <param name="page">The page whose master page should be set.</param>
        /// <param name="masterPage">The master page to set.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken" /> used to propagate notifications that the operation should be canceled.</param>
        /// <returns>
        /// The task object representing the asynchronous operation.
        /// </returns>
        Task SetPageMasterAsync(PortalItem portal, PageItem page, PageItem masterPage, CancellationToken cancellationToken);

        /// <summary>
        /// Adds a new page.
        /// </summary>
        /// <param name="portal">The portal which owns the page.</param>
        /// <param name="parentPage">The parent page. If this parameter is null, the page will be a root page.</param>
        /// <param name="page">The page to insert.</param>
        /// <param name="cancellationToken">A token to observe while waiting for the task to complete.</param>
        /// <returns>
        /// A task that represents the asynchronous operation.
        /// </returns>
        Task AddPageAsync(PortalItem portal, PageItem parentPage, PageItem page, CancellationToken cancellationToken);

        /// <summary>
        /// Updates the given page information with the given new page information.
        /// </summary>
        /// <param name="portal">The portal which owns the page.</param>
        /// <param name="page">The page.</param>
        /// <param name="newPage">The new page.</param>
        /// <param name="cancellationToken">A token to observe while waiting for the task to complete.</param>
        /// <returns>
        /// A task that represents the asynchronous operation.
        /// </returns>
        Task ReplacePageAsync(PortalItem portal, PageItem page, PageItem newPage, CancellationToken cancellationToken);

        /// <summary>
        /// Copies an existing page to a new page.
        /// </summary>
        /// <param name="portal">The portal which owns the page.</param>
        /// <param name="page">The page to copy.</param>
        /// <param name="normalizedUri">The normalized URI to set.</param>
        /// <param name="cancellationToken">A token to observe while waiting for the task to complete.</param>
        /// <returns>
        /// A task that represents the asynchronous operation.
        /// </returns>
        Task CopyPageAsync(PortalItem portal, PageItem page, string normalizedUri, CancellationToken cancellationToken);

        /// <summary>
        /// Moves the specified <paramref name="page" /> to a new location as a child of the current page.
        /// </summary>
        /// <param name="portal">The portal which owns the page.</param>
        /// <param name="page">The page to move.</param>
        /// <param name="referencePage">The reference page. If null, <paramref name="page" /> is inserted at the end of the list of child nodes.</param>
        /// <param name="cancellationToken">A token to observe while waiting for the task to complete.</param>
        /// <returns>
        /// A task that represents the asynchronous operation.
        /// </returns>
        Task MovePageAsync(PortalItem portal, PageItem page, PageItem referencePage, CancellationToken cancellationToken);

        /// <summary>
        /// Removes a page.
        /// </summary>
        /// <param name="portal">The portal which owns the page.</param>
        /// <param name="page">The page to remove.</param>
        /// <param name="cancellationToken">A token to observe while waiting for the task to complete.</param>
        /// <returns>
        /// A task that represents the asynchronous operation.
        /// </returns>
        Task RemovePageAsync(PortalItem portal, PageItem page, CancellationToken cancellationToken);

        /// <summary>
        /// Finds and returns a page, if any, which has the specified primary key.
        /// </summary>
        /// <param name="id">The primary key for the item to be found.</param>
        /// <param name="fields">The related fields to include in the query results.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken" /> used to propagate notifications that the operation should be canceled.</param>
        /// <returns>
        /// The <see cref="Task" /> that represents the asynchronous operation, containing the page matching the specified <paramref name="id" /> if it exists.
        /// </returns>
        Task<PageItem> GetPageByIdAsync(int id, PageField fields, CancellationToken cancellationToken);

        /// <summary>
        /// Finds and returns a page, if any, which has the specified normalized uri.
        /// </summary>
        /// <param name="portal">The portal which owns the page.</param>
        /// <param name="normalizedUri">The normalized uri to search for.</param>
        /// <param name="fields">The related fields to include in the query results.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken" /> used to propagate notifications that the operation should be canceled.</param>
        /// <returns>
        /// The <see cref="Task" /> that represents the asynchronous operation, containing the page matching the specified <paramref name="normalizedUri" /> if it exists.
        /// </returns>
        Task<PageItem> GetPageByUriAsync(PortalItem portal, string normalizedUri, PageField fields, CancellationToken cancellationToken);

        /// <summary>
        /// Gets a list of <see cref="PageItem" />s to be belonging to the specified <paramref name="portal" /> as an asynchronous operation.
        /// </summary>
        /// <param name="portal">The portal which owns the pages.</param>
        /// <param name="cancellationToken">A token to observe while waiting for the task to complete.</param>
        /// <returns>
        /// A task that represents the asynchronous operation.
        /// </returns>
        Task<IList<PageItem>> GetPagesAsync(PortalItem portal, CancellationToken cancellationToken);

        /// <summary>
        /// Gets a list of <see cref="PageItem" />s to be belonging to the specified <paramref name="portal" /> as an asynchronous operation.
        /// The last page in the list represents the specified page, and all other pages represent an ordered list of master pages.
        /// </summary>
        /// <param name="portal">The portal which owns the pages.</param>
        /// <param name="normalizedUri">The normalized uri to search for.</param>
        /// <param name="cancellationToken">A token to observe while waiting for the task to complete.</param>
        /// <returns>
        /// A task that represents the asynchronous operation.
        /// </returns>
        Task<PageLayers> GetPageLayersByUriAsync(PortalItem portal, string normalizedUri, CancellationToken cancellationToken);
    }
}
