// Copyright (c) János Janka. All rights reserved.

using System.Collections.Generic;

namespace Partnerinfo.Portal
{
    public class PortalTemplate
    {
        /// <summary>
        /// Gets the pages.
        /// </summary>
        /// <value>
        /// The pages.
        /// </value>
        public IList<PageItem> Pages { get; } = new List<PageItem>();
    }
}
