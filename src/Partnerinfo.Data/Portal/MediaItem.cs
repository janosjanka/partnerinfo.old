// Copyright (c) János Janka. All rights reserved.

using System;

namespace Partnerinfo.Portal
{
    public class MediaItem : ResourceItem
    {
        /// <summary>
        /// Gets or sets the portal which owns this <see cref="MediaItem" />.
        /// </summary>
        /// <value>
        /// The portal which owns this <see cref="MediaItem" />.
        /// </value>
        public ResourceItem Portal { get; set; }

        /// <summary>
        /// Gets or sets the mime type for this <see cref="MediaItem" />.
        /// </summary>
        /// <value>
        /// The type for this <see cref="MediaItem" />.
        /// </value>
        public string Type { get; set; }

        /// <summary>
        /// Gets or sets the date and time, in UTC, when this <see cref="MediaItem" /> was last modified.
        /// </summary>
        /// <value>
        /// The date and time, in UTC, when this <see cref="MediaItem" /> was last modified.
        /// </value>
        public DateTime ModifiedDate { get; set; }
    }
}
