// Copyright (c) János Janka. All rights reserved.

using System;

namespace Partnerinfo.Portal.Models
{
    public class MediaItemDto
    {
        /// <summary>
        /// Gets or sets the primary key for this <see cref="MediaItemDto" />.
        /// </summary>
        /// <value>
        /// The primary key.
        /// </value>
        public int Id { get; set; }

        /// <summary>
        /// Gets or sets a human-readable name for the <see cref="MediaItemDto" />.
        /// </summary>
        /// <value>
        /// The human-readable name.
        /// </value>
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the URI (Unique Resource Identifier) for this <see cref="MediaItemDto" /> provided by the storage.
        /// </summary>
        /// <value>
        /// The URI (Unique Resource Identifier).
        /// </value>
        public string Uri { get; set; }

        /// <summary>
        /// Gets or sets the mime type for this <see cref="MediaItemDto" />.
        /// </summary>
        /// <value>
        /// The type for this <see cref="MediaItemDto" />.
        /// </value>
        public string Type { get; set; }

        /// <summary>
        /// Gets or sets the date and time, in UTC, when this <see cref="MediaItemDto" /> was last modified.
        /// </summary>
        /// <value>
        /// The date and time, in UTC, when this <see cref="MediaItemDto" /> was last modified.
        /// </value>
        public DateTime ModifiedDate { get; set; }

        /// <summary>
        /// Gets or sets the absolute URL to the <see cref="MediaItemDto" />.
        /// </summary>
        /// <value>
        /// The absolute URL.
        /// </value>
        public string Link { get; set; }
    }
}