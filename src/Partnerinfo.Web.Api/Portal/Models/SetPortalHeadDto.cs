// Copyright (c) János Janka. All rights reserved.

using System.ComponentModel.DataAnnotations;

namespace Partnerinfo.Portal.Models
{
    public class SetPortalHeadDto
    {
        /// <summary>
        /// Gets or sets the part of a URL which identifies this <see cref="SetPortalHeadDto" /> using human-readable keywords.
        /// </summary>
        /// <value>
        /// The part of a URL.
        /// </value>
        [MaxLength(64)]
        [UriPartValidator]
        public string Uri { get; set; }

        /// <summary>
        /// Gets or sets the name for this <see cref="SetPortalHeadDto" />.
        /// </summary>
        /// <value>
        /// The name.
        /// </value>
        [Required]
        [MaxLength(64)]
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the description of this <see cref="SetPortalHeadDto" />.
        /// </summary>
        /// <value>
        /// The description.
        /// </value>
        [MaxLength(256)]
        public string Description { get; set; }

        /// <summary>
        /// Gets or sets the tracking code of Google Analytics.
        /// </summary>
        /// <value>
        /// The tracking code.
        /// </value>
        [MaxLength(128)]
        public string GATrackingId { get; set; }
    }
}