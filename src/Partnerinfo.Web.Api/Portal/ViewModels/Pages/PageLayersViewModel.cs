// Copyright (c) Partnerinfo Ltd. All rights reserved.

using Partnerinfo.Portal.ViewModels.Portals;

namespace Partnerinfo.Portal.ViewModels.Pages
{
    public class PageLayersViewModel
    {
        /// <summary>
        /// Gets or sets the portal which owns the page(s).
        /// </summary>
        /// <value>
        /// The portal.
        /// </value>
        public PortalViewModel Portal { get; set; }

        /// <summary>
        /// Gets or sets the master <see cref="PageViewModel" />.
        /// </summary>
        /// <value>
        /// The master <see cref="PageViewModel" />.
        /// </value>
        public PageViewModel MasterPage { get; set; }

        /// <summary>
        /// Gets or sets the content <see cref="PageViewModel" />.
        /// </summary>
        /// <value>
        /// The content <see cref="PageViewModel" />.
        /// </value>
        public PageViewModel ContentPage { get; set; }
    }
}