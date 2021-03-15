// Copyright (c) János Janka. All rights reserved.

using System;
using System.Linq;

namespace Partnerinfo.Media.EntityFramework
{
    internal static class MediaQueries
    {
        /// <summary>
        /// Gets a collection of all the media categories in the database.
        /// </summary>
        public static readonly Func<PartnerDbContext, int, string, IQueryable<MediaPlaylistResult>> GetPlaylists =
            (PartnerDbContext context, int contactId, string name) =>
                from pls in context.MediaPlaylists
                where (pls.ContactId == contactId) && (name == null || pls.Name.Contains(name))
                orderby pls.DefaultList descending, pls.Name
                select new MediaPlaylistResult
                {
                    Id = pls.Id,
                    Uri = pls.Uri,
                    Name = pls.Name,
                    DefaultList = pls.DefaultList,
                    EditMode = pls.EditMode,
                    ModifiedDate = pls.ModifiedDate,
                    ItemCount = pls.Items.Count()
                };

        /// <summary>
        /// Gets a collection of all the media items in the database.
        /// </summary>
        public static readonly Func<PartnerDbContext, int, string, IQueryable<MediaPlaylistItemResult>> GetItemsByPlaylist =
            (PartnerDbContext context, int playlistId, string name) =>
                from mi in context.MediaPlaylistItems
                where mi.PlaylistId == playlistId && (name == null || mi.Name.Contains(name))
                orderby mi.SortOrderId
                select new MediaPlaylistItemResult
                {
                    Id = mi.Id,
                    Name = mi.Name,
                    SortOrderId = mi.SortOrderId,
                    MediaType = mi.MediaType,
                    MediaId = mi.MediaId,
                    Duration = mi.Duration,
                    PublishDate = mi.PublishDate
                };

        /// <summary>
        /// Gets a collection of all the media items in the database.
        /// </summary>
        public static readonly Func<PartnerDbContext, int, string, IQueryable<MediaPlaylistItemResult>> GetItemsByContact =
            (PartnerDbContext context, int contactId, string name) =>
                from mi in context.MediaPlaylistItems
                join mp in context.MediaPlaylists on mi.PlaylistId equals mp.Id
                where mp.ContactId == contactId && (name == null || mi.Name.Contains(name))
                orderby mi.SortOrderId
                select new MediaPlaylistItemResult
                {
                    Id = mi.Id,
                    Playlist = new ResourceItem
                    {
                        Id = mi.PlaylistId,
                        Uri = mp.Uri,
                        Name = mp.Name
                    },
                    Name = mi.Name,
                    SortOrderId = mi.SortOrderId,
                    MediaType = mi.MediaType,
                    MediaId = mi.MediaId,
                    Duration = mi.Duration,
                    PublishDate = mi.PublishDate
                };
    }
}
