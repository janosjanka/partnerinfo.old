// Copyright (c) János Janka. All rights reserved.

using System.Threading;
using System.Threading.Tasks;

namespace Partnerinfo.Media.EntityFramework
{
    /// <summary>
    /// Provides facilities for querying and working with system data as objects.
    /// </summary>
    public interface IMediaStore
    {
        /// <summary>
        /// Sets the specified playlist as default playlist.
        /// </summary>
        /// <param name="playlist">A <see cref="MediaPlaylist" /> object that represents the playlist to be updated and the updated information for the playlist.</param>
        /// <param name="cancellationToken">A token to observe while waiting for the task to complete.</param>
        /// <returns>
        /// A task that represents the asynchronous operation.
        /// </returns>
        Task SetDefaultAsync(MediaPlaylist playlist, CancellationToken cancellationToken);

        /// <summary>
        /// Creates a new playlist and inserts it to the database.
        /// </summary>
        /// <param name="playlist">A <see cref="MediaPlaylist" /> object that represents the playlist to be inserted and the inserted information for the playlist.</param>
        /// <param name="cancellationToken">A token to observe while waiting for the task to complete.</param>
        /// <returns>
        /// A task that represents the asynchronous operation.
        /// </returns>
        Task CreateAsync(MediaPlaylist playlist, CancellationToken cancellationToken);

        /// <summary>
        /// Updates the database with the information for the specified playlist.
        /// </summary>
        /// <param name="playlist">A <see cref="MediaPlaylist" /> object that represents the playlist to be updated and the updated information for the playlist.</param>
        /// <param name="cancellationToken">A token to observe while waiting for the task to complete.</param>
        /// <returns>
        /// A task that represents the asynchronous operation.
        /// </returns>
        Task UpdateAsync(MediaPlaylist playlist, CancellationToken cancellationToken);

        /// <summary>
        /// Deletes the playlist.
        /// </summary>
        /// <param name="id">The unique playlist identifier from the data source for the playlist.</param>
        /// <param name="cancellationToken">A token to observe while waiting for the task to complete.</param>
        /// <returns>
        /// A task that represents the asynchronous operation.
        /// </returns>
        Task DeleteAsync(int id, CancellationToken cancellationToken);

        /// <summary>
        /// Gets the information from the data source for the playlist associated with the specified unique identifier.
        /// </summary>
        /// <param name="id">The unique identifier from the data source for the playlist.</param>
        /// <param name="cancellationToken">A token to observe while waiting for the task to complete.</param>
        /// <returns>
        /// A task that represents the asynchronous operation.
        /// </returns>
        Task<MediaPlaylist> FindByIdAsync(int id, CancellationToken cancellationToken);

        /// <summary>
        /// Gets the information from the data source for the playlist associated with the specified unique identifier.
        /// </summary>
        /// <param name="uri">The URI.</param>
        /// <param name="cancellationToken">A token to observe while waiting for the task to complete.</param>
        /// <returns>
        /// A task that represents the asynchronous operation.
        /// </returns>
        Task<MediaPlaylist> FindByUriAsync(string uri, CancellationToken cancellationToken);

        /// <summary>
        /// Gets the information from the data source for the playlist associated with the specified unique identifier.
        /// </summary>
        /// <param name="contactId">The contact identifier to be found.</param>
        /// <param name="cancellationToken">A token to observe while waiting for the task to complete.</param>
        /// <returns>
        /// A task that represents the asynchronous operation.
        /// </returns>
        Task<MediaPlaylist> FindDefaultByContactAsync(int contactId, CancellationToken cancellationToken);

        /// <summary>
        /// Gets a collection of playlists, in a page of data, where the playlist name contains the specified playlist name to match.
        /// </summary>
        /// <param name="userId">The user identifier to search for.</param>
        /// <param name="name">The name to search for.</param>
        /// <param name="pageIndex">The index of the page of results to return. Use 1 to indicate the first page.</param>
        /// <param name="pageSize">The size of the page of results to return. <paramref name="pageIndex" /> is non-zero-based.</param>
        /// <param name="cancellationToken">A token to observe while waiting for the task to complete.</param>
        /// <returns>
        /// A task that represents the asynchronous operation.
        /// </returns>
        Task<ListResult<MediaPlaylistResult>> FindAllAsync(int userId, string name, int pageIndex, int pageSize, CancellationToken cancellationToken);

        /// <summary>
        /// Creates a new playlist item and inserts it to the database.
        /// </summary>
        /// <param name="item">A <see cref="MediaPlaylistItem" /> object that represents the playlist item to be inserted and the inserted information for the playlist item.</param>
        /// <param name="cancellationToken">A token to observe while waiting for the task to complete.</param>
        /// <returns>
        /// A task that represents the asynchronous operation.
        /// </returns>
        Task AddItemAsync(MediaPlaylistItem item, CancellationToken cancellationToken);

        /// <summary>
        /// Updates the database with the information for the specified playlist item.
        /// </summary>
        /// <param name="item">A <see cref="MediaPlaylistItem" /> object that represents the playlist item to be updated and the updated information for the playlist item.</param>
        /// <param name="cancellationToken">A token to observe while waiting for the task to complete.</param>
        /// <returns>
        /// A task that represents the asynchronous operation.
        /// </returns>
        Task ReplaceItemAsync(MediaPlaylistItem item, CancellationToken cancellationToken);

        /// <summary>
        /// Moves the playlist item at the specified index to a new location in the database.
        /// </summary>
        /// <param name="id">The unique identifier from the data source for the playlist item.</param>
        /// <param name="sortOrderId">The new location of the playlist item in the database.</param>
        /// <param name="cancellationToken">A token to observe while waiting for the task to complete.</param>
        /// <returns>
        /// A task that represents the asynchronous operation.
        /// </returns>
        Task MoveItemAsync(int id, int sortOrderId, CancellationToken cancellationToken);

        /// <summary>
        /// Deletes the playlist item.
        /// </summary>
        /// <param name="id">The unique playlist item identifier from the data source for the playlist item.</param>
        /// <param name="cancellationToken">A token to observe while waiting for the task to complete.</param>
        /// <returns>
        /// A task that represents the asynchronous operation.
        /// </returns>
        Task DeleteItemAsync(int id, CancellationToken cancellationToken);

        /// <summary>
        /// Gets the information from the data source for the playlist item associated with the specified unique identifier.
        /// </summary>
        /// <param name="id">The unique identifier from the data source for the playlist item.</param>
        /// <param name="cancellationToken">A token to observe while waiting for the task to complete.</param>
        /// <returns>
        /// A task that represents the asynchronous operation.
        /// </returns>
        Task<MediaPlaylistItem> GetItemAsync(int id, CancellationToken cancellationToken);

        /// <summary>
        /// Gets a collection of playlist items, in a page of data, where the playlist item name contains the specified name to match.
        /// </summary>
        /// <param name="playlistId">The playlist identifier to search for.</param>
        /// <param name="name">The name to search for.</param>
        /// <param name="pageIndex">The index of the page of results to return. Use 1 to indicate the first page.</param>
        /// <param name="pageSize">The size of the page of results to return. <paramref name="pageIndex" /> is non-zero-based.</param>
        /// <param name="cancellationToken">A token to observe while waiting for the task to complete.</param>
        /// <returns>
        /// A task that represents the asynchronous operation.
        /// </returns>
        Task<ListResult<MediaPlaylistItemResult>> GetItemsByPlaylistAsync(int playlistId, string name, int pageIndex, int pageSize, CancellationToken cancellationToken);

        /// <summary>
        /// Gets a collection of playlist items, in a page of data, where the playlist item name contains the specified name to match.
        /// </summary>
        /// <param name="contactId">The contact identifier to search for.</param>
        /// <param name="name">The name to search for.</param>
        /// <param name="pageIndex">The index of the page of results to return. Use 1 to indicate the first page.</param>
        /// <param name="pageSize">The size of the page of results to return. <paramref name="pageIndex" /> is non-zero-based.</param>
        /// <param name="cancellationToken">A token to observe while waiting for the task to complete.</param>
        /// <returns>
        /// A task that represents the asynchronous operation.
        /// </returns>
        Task<ListResult<MediaPlaylistItemResult>> GetItemsByContactAsync(int contactId, string name, int pageIndex, int pageSize, CancellationToken cancellationToken);
    }
}
