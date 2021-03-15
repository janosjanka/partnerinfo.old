// Copyright (c) Partnerinfo Ltd. All rights reserved.

using System.ComponentModel.DataAnnotations;

namespace Partnerinfo.Portal.ViewModels.Pages
{
    /// <summary>
    /// Represents a view of a <see cref="PageItem" /> without content.
    /// </summary>
    public class SetPageHeadViewModel
    {
        /// <summary>
        /// Gets or sets the part of a URL which identifies this <see cref="SetPageHeadViewModel" /> using human-readable keywords.
        /// </summary>
        /// <value>
        /// The part of a URL.
        /// </value>
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
        /// Gets or sets the description of this <see cref="SetPageHeadViewModel" />.
        /// </summary>
        /// <value>
        /// The description.
        /// </value>
        [MaxLength(256)]
        public string Description { get; set; }
    }
}