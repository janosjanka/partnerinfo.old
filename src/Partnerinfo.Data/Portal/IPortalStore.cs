// Copyright (c) János Janka. All rights reserved.

using System;
using System.Threading;
using System.Threading.Tasks;

namespace Partnerinfo.Portal
{
    /// <summary>
    /// Provides facilities for querying and working with system data as objects.
    /// </summary>
    public interface IPortalStore : IDisposable
    {
        /// <summary>
        /// Gets the URI for the specified <paramref name="portal" />.
        /// </summary>
        /// <param name="portal">The portal whose URI should be retrieved.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken" /> used to propagate notifications that the operation should be canceled.</param>
        /// <returns>
        /// The task object representing the asynchronous operation.
        /// </returns>
        Task<string> GetUriAsync(PortalItem portal, CancellationToken cancellationToken);

        /// <summary>
        /// Sets the <paramref name="normalizedUri" /> for a <paramref name="portal" />.
        /// </summary>
        /// <param name="portal">The portal whose name should be set.</param>
        /// <param name="normalizedUri">The normalized URI to set.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken" /> used to propagate notifications that the operation should be canceled.</param>
        /// <returns>
        /// The task object representing the asynchronous operation.
        /// </returns>
        Task SetUriAsync(PortalItem portal, string normalizedUri, CancellationToken cancellationToken);

        /// <summary>
        /// Gets the home page for the specified <paramref name="portal" />.
        /// </summary>
        /// <param name="portal">The portal whose home page should be returned.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken" /> used to propagate notifications that the operation should be canceled.</param>
        /// <returns>
        /// The task object containing the results of the asynchronous operation, the home page for the specified <paramref name="portal" />.
        /// </returns>
        Task<ResourceItem> GetHomePageAsync(PortalItem portal, CancellationToken cancellationToken);

        /// <summary>
        /// Sets the <paramref name="homePage" /> for a <paramref name="portal" />.
        /// </summary>
        /// <param name="portal">The portal whose home page should be set.</param>
        /// <param name="homePage">The home page to set.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken" /> used to propagate notifications that the operation should be canceled.</param>
        /// <returns>
        /// The task object representing the asynchronous operation.
        /// </returns>
        Task SetHomePageAsync(PortalItem portal, ResourceItem homePage, CancellationToken cancellationToken);

        /// <summary>
        /// Gets the master page for the specified <paramref name="portal" />.
        /// </summary>
        /// <param name="portal">The portal whose master page should be returned.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken" /> used to propagate notifications that the operation should be canceled.</param>
        /// <returns>
        /// The task object containing the results of the asynchronous operation, the master page for the specified <paramref name="portal" />.
        /// </returns>
        Task<ResourceItem> GetMasterPageAsync(PortalItem portal, CancellationToken cancellationToken);

        /// <summary>
        /// Sets the <paramref name="masterPage" /> for a <paramref name="portal" />.
        /// </summary>
        /// <param name="portal">The portal whose master page should be set.</param>
        /// <param name="masterPage">The master page to set.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken" /> used to propagate notifications that the operation should be canceled.</param>
        /// <returns>
        /// The task object representing the asynchronous operation.
        /// </returns>
        Task SetMasterPageAsync(PortalItem portal, ResourceItem masterPage, CancellationToken cancellationToken);

        /// <summary>
        /// Gets the project for the specified <paramref name="portal" />.
        /// </summary>
        /// <param name="portal">The portal whose project should be returned.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken" /> used to propagate notifications that the operation should be canceled.</param>
        /// <returns>
        /// The task object containing the results of the asynchronous operation, the project for the specified <paramref name="portal" />.
        /// </returns>
        Task<UniqueItem> GetProjectAsync(PortalItem portal, CancellationToken cancellationToken);

        /// <summary>
        /// Sets the <paramref name="project" /> for a <paramref name="portal" />.
        /// </summary>
        /// <param name="portal">The portal whose project should be set.</param>
        /// <param name="project">The project to set.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken" /> used to propagate notifications that the operation should be canceled.</param>
        /// <returns>
        /// The task object representing the asynchronous operation.
        /// </returns>
        Task SetProjectAsync(PortalItem portal, UniqueItem project, CancellationToken cancellationToken);

        /// <summary>
        /// Creates the specified <paramref name="portal" /> in the portal store, as an asynchronous operation.
        /// </summary>
        /// <param name="portal">The portal to create.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken" /> used to propagate notifications that the operation should be canceled.</param>
        /// <returns>
        /// The <see cref="Task" /> that represents the asynchronous operation, containing the <see cref="ValidationResult" /> of the creation operation.
        /// </returns>
        Task<ValidationResult> CreateAsync(PortalItem portal, CancellationToken cancellationToken);

        /// <summary>
        /// Updates the specified <paramref name="portal" /> in the portal store, as an asynchronous operation
        /// </summary>
        /// <param name="portal">The portal to update.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken" /> used to propagate notifications that the operation should be canceled.</param>
        /// <returns>
        /// The <see cref="Task" /> that represents the asynchronous operation, containing the <see cref="ValidationResult" /> of the update operation.
        /// </returns>
        Task<ValidationResult> UpdateAsync(PortalItem portal, CancellationToken cancellationToken);

        /// <summary>
        /// Copies an existing portal to a new portal.
        /// </summary>
        /// <param name="portal">The portal to copy.</param>
        /// <param name="normalizedUri">The normalized uri for the new portal.</param>
        /// <param name="cancellationToken">A token to observe while waiting for the task to complete.</param>
        /// <returns>
        /// A task that represents the asynchronous operation.
        /// </returns>
        Task<ValidationResult> CopyAsync(PortalItem portal, string normalizedUri, CancellationToken cancellationToken);

        /// <summary>
        /// Deletes the specified <paramref name="portal" /> from the portal store, as an asynchronous operation.
        /// </summary>
        /// <param name="portal">The portal to delete.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken" /> used to propagate notifications that the operation should be canceled.</param>
        /// <returns>
        /// The <see cref="Task" /> that represents the asynchronous operation, containing the <see cref="ValidationResult" /> of the update operation.
        /// </returns>
        Task<ValidationResult> DeleteAsync(PortalItem portal, CancellationToken cancellationToken);

        /// <summary>
        /// Finds and returns a portal, if any, which has the specified normalized uri.
        /// </summary>
        /// <param name="normalizedUri">The normalized uri to search for.</param>
        /// <param name="fields">The related fields to include in the query results.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken" /> used to propagate notifications that the operation should be canceled.</param>
        /// <returns>
        /// The <see cref="Task" /> that represents the asynchronous operation, containing the portal matching the specified <paramref name="normalizedUri" /> if it exists.
        /// </returns>
        Task<PortalItem> FindByUriAsync(string normalizedUri, PortalField fields, CancellationToken cancellationToken);

        /// <summary>
        /// Finds a collection of portals with the given values.
        /// </summary>
        /// <param name="userId">The user identifier for the item to be found.</param>
        /// <param name="name">The name for the item to be found.</param>
        /// <param name="orderBy">The order in which items are returned in a result set.</param>
        /// <param name="pageIndex">The index of the page of results to return. Use 1 to indicate the first page.</param>
        /// <param name="pageSize">The size of the page of results to return. <paramref name="pageIndex" /> is non-zero-based.</param>
        /// <param name="fields">The related fields to include in the query results.</param>
        /// <param name="cancellationToken">A token to observe while waiting for the task to complete.</param>
        /// <returns>
        /// A task that represents the asynchronous operation.
        /// </returns>
        Task<ListResult<PortalItem>> FindAllAsync(int userId, string name, PortalSortOrder orderBy, int pageIndex, int pageSize, PortalField fields, CancellationToken cancellationToken);
    }
}
