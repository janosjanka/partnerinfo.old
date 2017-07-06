// Copyright (c) János Janka. All rights reserved.

using System;

namespace Partnerinfo.Media.EntityFramework
{
    public class MediaPlaylistItem
    {
        /// <summary>
        /// PlaylistItem ID (Primary Key)
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Playlist ID (Foreign Key)
        /// </summary>
        public int PlaylistId { get; set; }
        
        /// <summary>
        /// Sort order ID
        /// </summary>
        public int SortOrderId { get; set; }

        /// <summary>
        /// Playlist item name
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Media type ( YouTube, Dailymotion, etc. )
        /// </summary>
        public MediaType MediaType { get; set; }

        /// <summary>
        /// Media ID which is given by an external provider
        /// </summary>
        public string MediaId { get; set; }

        /// <summary>
        /// Media duration
        /// </summary>
        public int Duration { get; set; }

        /// <summary>
        /// Publish date, in coordinated universal time (UTC)
        /// </summary>
        public DateTime PublishDate { get; set; } = DateTime.UtcNow;
    }
}
