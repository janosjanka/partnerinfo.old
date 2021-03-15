// Copyright (c) János Janka. All rights reserved.

using System.Threading;
using System.Threading.Tasks;

namespace Partnerinfo.Portal
{
    public interface IPortalMediaStore : IPortalStore
    {
        /// <summary>
        /// Adds a new <see cref="MediaItem" /> to the specified <paramref name="portal" /> as an asynchronous operation.
        /// </summary>
        /// <param name="portal">The portal which owns the page.</param>
        /// <param name="parentMedia">The parent media which owns the new media.</param>
        /// <param name="media">The media to add.</param>
        /// <param name="cancellationToken">A token to observe while waiting for the task to complete.</param>
        /// <returns>
        /// A task that represents the asynchronous operation.
        /// </returns>
        Task AddMediaAsync(PortalItem portal, MediaItem parentMedia, MediaItem media, CancellationToken cancellationToken);

        /// <summary>
        /// Removes a <see cref="MediaItem" /> from the specified <paramref name="portal" /> as an asynchronous operation.
        /// </summary>
        /// <param name="portal">The portal which owns the media.</param>
        /// <param name="media">The media to remove.</param>
        /// <param name="cancellationToken">A token to observe while waiting for the task to complete.</param>
        /// <returns>
        /// A task that represents the asynchronous operation.
        /// </returns>
        Task RemoveMediaAsync(PortalItem portal, MediaItem media, CancellationToken cancellationToken);

        /// <summary>
        /// Finds and returns a <see cref="MediaItem" />, if any, which has the specified normalized uri.
        /// </summary>
        /// <param name="portal">The portal which owns the media.</param>
        /// <param name="normalizedUri">The normalized uri to search for.</param>
        /// <param name="fields">The related fields to include in the query results.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken" /> used to propagate notifications that the operation should be canceled.</param>
        /// <returns>
        /// The <see cref="Task" /> that represents the asynchronous operation, containing the page matching the specified <paramref name="normalizedUri" /> if it exists.
        /// </returns>
        Task<MediaItem> GetMediaByUriAsync(PortalItem portal, string normalizedUri, MediaField fields, CancellationToken cancellationToken);

        /// <summary>
        /// Gets a list of <see cref="MediaItem" />s to be belonging to the specified <paramref name="portal" /> as an asynchronous operation.
        /// </summary>
        /// <param name="portal">The portal which owns the pages.</param>
        /// <param name="parentMedia">The parent media which owns the media.</param>
        /// <param name="name">The name for the item to be found.</param>
        /// <param name="orderBy">The order in which items are returned in a result set.</param>
        /// <param name="fields">The related fields to include in the query results.</param>
        /// <param name="cancellationToken">A token to observe while waiting for the task to complete.</param>
        /// <returns>
        /// A task that represents the asynchronous operation.
        /// </returns>
        Task<ListResult<MediaItem>> GetMediaAsync(PortalItem portal, MediaItem parentMedia, string name, MediaSortOrder orderBy, MediaField fields, CancellationToken cancellationToken);
    }
}
