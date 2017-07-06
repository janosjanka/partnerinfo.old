// Copyright (c) János Janka. All rights reserved.

using System.ComponentModel.DataAnnotations;

namespace Partnerinfo.Portal.Models
{
    /// <summary>
    /// Represents a view for copying a <see cref="PageItem" />.
    /// </summary>
    public class CopyPageDto
    {
        /// <summary>
        /// Gets or sets the part of a URL which identifies this <see cref="CopyPageDto" /> using human-readable keywords.
        /// </summary>
        /// <value>
        /// The part of a URL.
        /// </value>
        [MaxLength(64)]
        [UriPartValidator]
        public string Uri { get; set; }
    }
}