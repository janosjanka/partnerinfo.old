// Copyright (c) János Janka. All rights reserved.

using System;

namespace Partnerinfo.Media.EntityFramework
{
    /// <summary>
    /// Represents a media item DTO that holds all data that is required for the remote call.
    /// </summary>
    public sealed class MediaPlaylistItemResult
    {
        public int Id { get; set; }
        public UniqueItem Playlist { get; set; }
        public string Name { get; set; }
        public int SortOrderId { get; set; }
        public MediaType MediaType { get; set; }
        public string MediaId { get; set; }
        public int Duration { get; set; }
        public DateTime PublishDate { get; set; }
    }
}
