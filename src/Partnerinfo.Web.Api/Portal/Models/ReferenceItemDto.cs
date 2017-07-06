// Copyright (c) János Janka. All rights reserved.

using System.ComponentModel.DataAnnotations;

namespace Partnerinfo.Portal.Models
{
    public class ReferenceItemDto
    {
        /// <summary>
        /// Gets or sets the mime type for this <see cref="ReferenceItemDto" />.
        /// </summary>
        /// <value>
        /// The type for this <see cref="ReferenceItemDto" />.
        /// </value>
        [MaxLength(128)]
        public string Type { get; set; }

        /// <summary>
        /// Gets or sets the URI for this <see cref="ReferenceItemDto" />.
        /// </summary>
        /// <value>
        /// The URI for this <see cref="ReferenceItemDto" />.
        /// </value>
        [Required]
        [MaxLength(256)]
        [UrlValidator]
        public string Uri { get; set; }
    }
}