// Copyright (c) János Janka. All rights reserved.

using System;
using System.Collections.Generic;
using Partnerinfo.Security;

namespace Partnerinfo.Portal
{
    /// <summary>
    /// Represents a page of a <see cref="PortalItem" />.
    /// </summary>
    public class PageItem : SharedResourceItem
    {
        /// <summary>
        /// Gets the type of the ACE (Access Control Entry) for this <see cref="PageItem" />.
        /// </summary>
        public override AccessObjectType ObjectType => AccessObjectType.Page;

        /// <summary>
        /// Gets or sets the portal which owns this <see cref="PageItem" />.
        /// </summary>
        /// <value>
        /// The portal.
        /// </value>
        public ResourceItem Portal { get; set; }

        /// <summary>
        /// Gets or sets the master page for this <see cref="PageItem" />.
        /// </summary>
        /// <value>
        /// The master page.
        /// </value>
        public ResourceItem Master { get; set; }

        /// <summary>
        /// Gets or sets the date and time, in UTC, when this <see cref="PageItem" /> was last modified.
        /// </summary>
        /// <value>
        /// The date and time, in UTC, when this <see cref="PageItem" /> was last modified.
        /// </value>
        public DateTime ModifiedDate { get; set; }

        /// <summary>
        /// Gets or sets the description of this <see cref="PageItem" />.
        /// </summary>
        /// <value>
        /// The description.
        /// </value>
        public string Description { get; set; }

        /// <summary>
        /// Gets or sets the HTML content of this <see cref="PageItem" />.
        /// </summary>
        /// <value>
        /// The HTML content.
        /// </value>
        public string HtmlContent { get; set; }

        /// <summary>
        /// Gets or sets the CSS content of this <see cref="PageItem" />.
        /// </summary>
        /// <value>
        /// The CSS content.
        /// </value>
        public string StyleContent { get; set; }

        /// <summary>
        /// Gets the navigation property for <see cref="ReferenceItem" />s.
        /// </summary>
        /// <value>
        /// The navigation property for <see cref="ReferenceItem" />s.
        /// </value>
        public ICollection<ReferenceItem> References { get; } = new List<ReferenceItem>();

        /// <summary>
        /// Gets the navigation property for the child <see cref="PageItem" />s.
        /// </summary>
        /// <value>
        /// The navigation property for the child <see cref="PageItem" />s.
        /// </value>
        public ICollection<ResourceItem> Children { get; } = new List<ResourceItem>();
    }
}
