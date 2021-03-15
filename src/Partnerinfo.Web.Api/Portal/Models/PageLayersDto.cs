// Copyright (c) János Janka. All rights reserved.

namespace Partnerinfo.Portal.Models
{
    public class PageLayersDto
    {
        /// <summary>
        /// Gets or sets the portal which owns the page(s).
        /// </summary>
        /// <value>
        /// The portal.
        /// </value>
        public PortalItemDto Portal { get; set; }

        /// <summary>
        /// Gets or sets the master <see cref="PageItemDto" />.
        /// </summary>
        /// <value>
        /// The master <see cref="PageItemDto" />.
        /// </value>
        public PageItemDto MasterPage { get; set; }

        /// <summary>
        /// Gets or sets the content <see cref="PageItemDto" />.
        /// </summary>
        /// <value>
        /// The content <see cref="PageItemDto" />.
        /// </value>
        public PageItemDto ContentPage { get; set; }
    }
}