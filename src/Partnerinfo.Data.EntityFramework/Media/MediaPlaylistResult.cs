// Copyright (c) János Janka. All rights reserved.

using System;

namespace Partnerinfo.Media.EntityFramework
{
    /// <summary>
    /// Represents a media category DTO that holds all data that is required for the remote call.
    /// </summary>
    public sealed class MediaPlaylistResult
    {
        /// <summary>
        /// Gets or sets the primary key for this <see cref="MediaPlaylist" />.
        /// </summary>
        /// <value>
        /// The primary key.
        /// </value>
        public int Id { get; set; }

        /// <summary>
        /// Gets or sets the URI (Unique Resource Identifier) for this <see cref="MediaPlaylist" /> provided by the storage.
        /// </summary>
        /// <value>
        /// The URI (Unique Resource Identifier).
        /// </value>
        public string Uri { get; set; }

        /// <summary>
        /// Gets or sets a human-readable name for the <see cref="MediaPlaylist" />.
        /// </summary>
        /// <value>
        /// The human-readable name.
        /// </value>
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether this is the default playlist of the specified <see cref="ContactId" />.
        /// </summary>
        /// <value>
        /// <c>true</c> if this is the default playlist of the specified <see cref="ContactId" />; otherwise, <c>false</c>.
        /// </value>
        public bool DefaultList { get; set; }

        /// <summary>
        /// Gets or sets an edit mode that specifies how this playlist will be loaded at startup.
        /// </summary>
        /// <value>
        /// An edit mode that specifies how this playlist will be loaded at startup.
        /// </value>
        public PlaylistEditMode EditMode { get; set; } = PlaylistEditMode.Readonly;

        /// <summary>
        /// Gets or sets the date and time, in UTC, when this <see cref="MediaPlaylist" /> was last modified.
        /// </summary>
        /// <value>
        /// The date and time, in UTC, when this <see cref="MediaPlaylist" /> was last modified.
        /// </value>
        public DateTime ModifiedDate { get; set; } = DateTime.UtcNow;

        /// <summary>
        /// Gets or sets the total number of items were added to this playlist.
        /// </summary>
        /// <value>
        /// The total number of items.
        /// </value>
        public int ItemCount { get; set; }
    }
}
