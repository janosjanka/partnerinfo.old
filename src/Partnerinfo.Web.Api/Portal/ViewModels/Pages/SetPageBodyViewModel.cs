// Copyright (c) Partnerinfo Ltd. All rights reserved.

using System.ComponentModel.DataAnnotations;

namespace Partnerinfo.Portal.ViewModels.Pages
{
    /// <summary>
    /// Represents a view of content of a <see cref="PageItem" />.
    /// </summary>
    public class SetPageBodyViewModel
    {
        /// <summary>
        /// Gets or sets the HTML content of a <see cref="PageItem" />.
        /// </summary>
        /// <value>
        /// The HTML content.
        /// </value>
        [Required]
        [MaxLength(131072)]
        public string HtmlContent { get; set; }

        /// <summary>
        /// Gets or sets the CSS content of a <see cref="PageItem" />.
        /// </summary>
        /// <value>
        /// The CSS content.
        /// </value>
        [MaxLength(131072)]
        public string StyleContent { get; set; }
    }
}