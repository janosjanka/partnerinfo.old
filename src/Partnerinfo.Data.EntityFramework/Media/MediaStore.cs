// Copyright (c) János Janka. All rights reserved.

using System;
using System.Data.Entity;
using System.Data.SqlClient;
using System.Threading;
using System.Threading.Tasks;

namespace Partnerinfo.Media.EntityFramework
{
    /// <summary>
    /// Provides facilities for querying and working with system data as objects.
    /// </summary>
    public sealed class MediaStore : IMediaStore
    {
        private readonly PartnerDbContext _context;

        /// <summary>
        /// If true will call SaveChanges after Create/Update/Delete.
        /// </summary>
        public bool AutoSaveChanges { get; set; } = true;

        /// <summary>
        /// Initializes a new instance of the <see cref="MediaStore"/> class.
        /// </summary>
        /// <param name="context">The context.</param>
        public MediaStore(PartnerDbContext context)
        {
            if (context == null)
            {
                throw new ArgumentNullException(nameof(context));
            }

            _context = context;
        }

        /// <summary>
        /// Sets the specified playlist as default playlist.
        /// </summary>
        /// <param name="playlist">A <see cref="MediaPlaylist" /> object that represents the playlist to be updated and the updated information for the playlist.</param>
        /// <param name="cancellationToken">A token to observe while waiting for the task to complete.</param>
        /// <returns>
        /// A task that represents the asynchronous operation.
        /// </returns>
        public async Task SetDefaultAsync(MediaPlaylist playlist, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            if (playlist == null)
            {
                throw new ArgumentNullException(nameof(playlist));
            }

            var defaultPlaylist = await FindDefaultByContactAsync(playlist.ContactId, cancellationToken);
            if (defaultPlaylist != null)
            {
                defaultPlaylist.DefaultList = false;
            }
            playlist.DefaultList = true;
        }

        /// <summary>
        /// Creates a new playlist and inserts it to the database.
        /// </summary>
        /// <param name="playlist">A <see cref="MediaPlaylist" /> object that represents the playlist to be inserted and the inserted information for the playlist.</param>
        /// <param name="cancellationToken">A token to observe while waiting for the task to complete.</param>
        /// <returns>
        /// A task that represents the asynchronous operation.
        /// </returns>
        /// <exception cref="System.ArgumentNullException"></exception>
        public async Task CreateAsync(MediaPlaylist playlist, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            if (playlist == null)
            {
                throw new ArgumentNullException(nameof(playlist));
            }

            if (playlist.DefaultList)
            {
                await SetDefaultAsync(playlist, cancellationToken);
            }
            _context.Add(playlist);
        }

        /// <summary>
        /// Updates the database with the information for the specified playlist.
        /// </summary>
        /// <param name="playlist">A <see cref="MediaPlaylist" /> object that represents the playlist to be updated and the updated information for the playlist.</param>
        /// <param name="cancellationToken">A token to observe while waiting for the task to complete.</param>
        /// <returns>
        /// A task that represents the asynchronous operation.
        /// </returns>
        /// <exception cref="System.ArgumentNullException"></exception>
        public async Task UpdateAsync(MediaPlaylist playlist, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            if (playlist == null)
            {
                throw new ArgumentNullException(nameof(playlist));
            }

            if (playlist.DefaultList)
            {
                await SetDefaultAsync(playlist, cancellationToken);
            }
            _context.Update(playlist);
        }

        /// <summary>
        /// Deletes the playlist.
        /// </summary>
        /// <param name="id">The unique playlist identifier from the data source for the playlist.</param>
        /// <param name="cancellationToken">A token to observe while waiting for the task to complete.</param>
        /// <returns>
        /// A task that represents the asynchronous operation.
        /// </returns>
        public Task DeleteAsync(int id, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            return _context.Database.ExecuteSqlCommandAsync(
                string.Format("{0}.DeletePlaylist @Id", DbSchema.Media),
                cancellationToken, new SqlParameter("Id", id));
        }

        /// <summary>
        /// Gets the information from the data source for the playlist associated with the specified unique identifier.
        /// </summary>
        /// <param name="id">The unique identifier from the data source for the playlist.</param>
        /// <param name="cancellationToken">A token to observe while waiting for the task to complete.</param>
        /// <returns>
        /// A task that represents the asynchronous operation.
        /// </returns>
        public Task<MediaPlaylist> FindByIdAsync(int id, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            return Playlists.FindAsync(cancellationToken, id);
        }

        /// <summary>
        /// Gets the information from the data source for the playlist associated with the specified unique identifier.
        /// </summary>
        /// <param name="uri">The URI.</param>
        /// <param name="cancellationToken">A token to observe while waiting for the task to complete.</param>
        /// <returns>
        /// A task that represents the asynchronous operation.
        /// </returns>
        public Task<MediaPlaylist> FindByUriAsync(string uri, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            return Playlists.FirstOrDefaultAsync(p => p.Uri.ToUpper() == uri.ToUpper(), cancellationToken);
        }

        /// <summary>
        /// Gets the information from the data source for the playlist associated with the specified unique identifier.
        /// </summary>
        /// <param name="contactId">The contact identifier to be found.</param>
        /// <param name="cancellationToken">A token to observe while waiting for the task to complete.</param>
        /// <returns>
        /// A task that represents the asynchronous operation.
        /// </returns>
        public Task<MediaPlaylist> FindDefaultByContactAsync(int contactId, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            return Playlists.FirstOrDefaultAsync(p => (p.ContactId == contactId) && p.DefaultList, cancellationToken);
        }

        /// <summary>
        /// Gets a collection of playlists, in a page of data, where the playlist name contains the specified playlist name to match.
        /// </summary>
        /// <param name="contactId">The user identifier to search for.</param>
        /// <param name="name">The name to search for.</param>
        /// <param name="pageIndex">The index of the page of results to return. Use 1 to indicate the first page.</param>
        /// <param name="pageSize">The size of the page of results to return. <paramref name="pageIndex" /> is non-zero-based.</param>
        /// <param name="cancellationToken">A <see cref="System.Threading.CancellationToken" /> to observe while waiting for the task to complete.</param>
        /// <returns>
        /// A collection of the <see cref="MediaPlaylistResult" /> objects representing the playlists in the database.
        /// </returns>
        public Task<ListResult<MediaPlaylistResult>> FindAllAsync(int contactId, string name, int pageIndex, int pageSize, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            return _context.FindItemsAsync(
                MediaQueries.GetPlaylists(_context, contactId, name).AsNoTracking(),
                pageIndex,
                pageSize,
                cancellationToken);
        }

        //
        // PlaylistItem
        //

        /// <summary>
        /// Gets a collection of playlist items, in a page of data, where the playlist item name contains the specified name to match.
        /// </summary>
        /// <param name="playlistId">The playlist identifier to search for.</param>
        /// <param name="name">The name to search for.</param>
        /// <param name="pageIndex">The index of the page of results to return. Use 1 to indicate the first page.</param>
        /// <param name="pageSize">The size of the page of results to return. <paramref name="pageIndex" /> is non-zero-based.</param>
        /// <param name="cancellationToken">A <see cref="System.Threading.CancellationToken" /> to observe while waiting for the task to complete.</param>
        /// <returns>
        /// A collection of the <see cref="MediaPlaylistItemResult" /> objects representing the playlist items in the database.
        /// </returns>
        public Task<ListResult<MediaPlaylistItemResult>> GetItemsByPlaylistAsync(int playlistId, string name, int pageIndex, int pageSize, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            return _context.FindItemsAsync(
                MediaQueries.GetItemsByPlaylist(_context, playlistId, name).AsNoTracking(),
                pageIndex,
                pageSize,
                cancellationToken);
        }

        /// <summary>
        /// Gets a collection of playlist items, in a page of data, where the playlist item name contains the specified name to match.
        /// </summary>
        /// <param name="contactId">The contact identifier to search for.</param>
        /// <param name="name">The name to search for.</param>
        /// <param name="pageIndex">The index of the page of results to return. Use 1 to indicate the first page.</param>
        /// <param name="pageSize">The size of the page of results to return. <paramref name="pageIndex" /> is non-zero-based.</param>
        /// <param name="cancellationToken">A <see cref="System.Threading.CancellationToken" /> to observe while waiting for the task to complete.</param>
        /// <returns>
        /// A collection of the <see cref="MediaPlaylistItemResult" /> objects representing the playlist items in the database.
        /// </returns>
        public Task<ListResult<MediaPlaylistItemResult>> GetItemsByContactAsync(int contactId, string name, int pageIndex, int pageSize, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            return _context.FindItemsAsync(
                MediaQueries.GetItemsByContact(_context, contactId, name).AsNoTracking(),
                pageIndex,
                pageSize,
                cancellationToken);
        }

        /// <summary>
        /// Gets the information from the data source for the playlist item associated with the specified unique identifier.
        /// </summary>
        /// <param name="id">The unique identifier from the data source for the playlist item.</param>
        /// <returns>
        /// Returns the playlist item associated with the specified unique identifier.
        /// </returns>
        public Task<MediaPlaylistItem> GetItemAsync(int id, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            return _context.MediaPlaylistItems.FindAsync(id, cancellationToken);
        }

        /// <summary>
        /// Creates a new playlist item and inserts it to the database.
        /// </summary>
        /// <param name="playlistItem">A <see cref="MediaPlaylistItem" /> object that represents the playlist item to be inserted and the inserted information for the playlist item.</param>
        /// <returns>
        /// The <see cref="MediaPlaylistItem" /> instance if the update was successful; otherwise, <c>null</c>.
        /// </returns>
        public Task AddItemAsync(MediaPlaylistItem playlistItem, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            if (playlistItem == null)
            {
                throw new ArgumentNullException(nameof(playlistItem));
            }
            _context.Add(playlistItem);
            return Task.CompletedTask;
        }

        /// <summary>
        /// Updates the database with the information for the specified playlist item.
        /// </summary>
        /// <param name="playlistItem">A <see cref="MediaPlaylistItem" /> object that represents the playlist item to be updated and the updated information for the playlist item.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>
        ///   <c>true</c> if the operation was successful; otherwise, <c>false</c>.
        /// </returns>
        /// <exception cref="System.ArgumentNullException"></exception>
        public Task ReplaceItemAsync(MediaPlaylistItem playlistItem, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            if (playlistItem == null)
            {
                throw new ArgumentNullException(nameof(playlistItem));
            }
            _context.Update(playlistItem);
            return Task.CompletedTask;
        }

        /// <summary>
        /// Moves the playlist item at the specified index to a new location in the database.
        /// </summary>
        /// <param name="id">The unique identifier from the data source for the playlist item.</param>
        /// <param name="sortOrderId">The new location of the playlist item in the database.</param>
        /// <param name="cancellationToken">A token to observe while waiting for the task to complete.</param>
        /// <returns>
        /// A task that represents the asynchronous operation.
        /// </returns>
        public Task MoveItemAsync(int id, int sortOrderId, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            return _context.Database.ExecuteSqlCommandAsync(
                string.Format("{0}.MovePlaylistItem @Id, @SortOrderId", DbSchema.Media),
                cancellationToken,
                DbParameters.Value("Id", id),
                DbParameters.Value("SortOrderId", sortOrderId));
        }

        /// <summary>
        /// Deletes the playlist item.
        /// </summary>
        /// <param name="id">The unique playlist item identifier from the data source for the playlist item.</param>
        /// <param name="cancellationToken">A token to observe while waiting for the task to complete.</param>
        /// <returns>
        /// A task that represents the asynchronous operation.
        /// </returns>
        public Task DeleteItemAsync(int id, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            return _context.Database.ExecuteSqlCommandAsync(
                string.Format("{0}.DeletePlaylistItem @Id", DbSchema.Media),
                cancellationToken,
                new SqlParameter("Id", id));
        }

        /// <summary>
        /// Saves all changes.
        /// </summary>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>
        /// The task.
        /// </returns>
        private async Task SaveChangesAsync(CancellationToken cancellationToken)
        {
            if (AutoSaveChanges)
            {
                await _context.SaveChangesAsync(cancellationToken);
            }
        }

        /// <summary>
        /// Gets the playlists.
        /// </summary>
        /// <value>
        /// The playlists.
        /// </value>
        private DbSet<MediaPlaylist> Playlists => _context.Set<MediaPlaylist>();

        /// <summary>
        /// Gets the playlist items.
        /// </summary>
        /// <value>
        /// The playlist items.
        /// </value>
        private DbSet<MediaPlaylistItem> PlaylistItems => _context.Set<MediaPlaylistItem>();
    }
}
