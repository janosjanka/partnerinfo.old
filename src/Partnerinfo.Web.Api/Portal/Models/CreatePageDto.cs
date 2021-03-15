// Copyright (c) János Janka. All rights reserved.

using System.ComponentModel.DataAnnotations;

namespace Partnerinfo.Portal.Models
{
    /// <summary>
    /// Represents a view for creating a <see cref="PageItem" />.
    /// </summary>
    public class CreatePageDto
    {
        /// <summary>
        /// Gets or sets the part of a URL which identifies this <see cref="CreatePageDto" /> using human-readable keywords.
        /// </summary>
        /// <value>
        /// The part of a URL.
        /// </value>
        [Required]
        [MaxLength(64)]
        [UriPartValidator]
        public string Uri { get; set; }

        /// <summary>
        /// Gets or sets the name for the item.
        /// </summary>
        /// <value>
        /// The name for the item.
        /// </value>
        [Required]
        [MaxLength(64)]
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the description of this <see cref="CreatePageDto" />.
        /// </summary>
        /// <value>
        /// The description.
        /// </value>
        [MaxLength(256)]
        public string Description { get; set; }

        /// <summary>
        /// Gets or sets the HTML content of this <see cref="CreatePageDto" />.
        /// </summary>
        /// <value>
        /// The HTML content.
        /// </value>
        public string HtmlContent { get; set; }

        /// <summary>
        /// Gets or sets the CSS content of this <see cref="CreatePageDto" />.
        /// </summary>
        /// <value>
        /// The CSS content.
        /// </value>
        public string StyleContent { get; set; }
    }
}