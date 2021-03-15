// Copyright (c) János Janka. All rights reserved.

using System;
using System.Collections.Generic;

namespace Partnerinfo.Portal.EntityFramework
{
    public class PortalMedia
    {
        /// <summary>
        /// Gets or sets the primary key for this <see cref="PortalMedia" />.
        /// </summary>
        /// <value>
        /// The primary key.
        /// </value>
        public int Id { get; set; }

        /// <summary>
        /// Gets or sets the parent which owns this <see cref="PortalMedia" />.
        /// </summary>
        /// <value>
        /// The parent which owns this <see cref="PortalMedia" />.
        /// </value>
        public int? ParentId { get; set; }

        /// <summary>
        /// Gets or sets the portal which owns this <see cref="PortalMedia" />.
        /// </summary>
        /// <value>
        /// The portal which owns this <see cref="PortalMedia" />.
        /// </value>
        public int PortalId { get; set; }

        /// <summary>
        /// Gets or sets the URI for this <see cref="PortalMedia" />.
        /// </summary>
        /// <value>
        /// The URI for this <see cref="PortalMedia" />.
        /// </value>
        public string Uri { get; set; }

        /// <summary>
        /// Gets or sets the mime type for this <see cref="PortalMedia" />.
        /// </summary>
        /// <value>
        /// The type for this <see cref="PortalMedia" />.
        /// </value>
        public string Type { get; set; }

        /// <summary>
        /// Gets or sets the path for this <see cref="PortalMedia" />.
        /// </summary>
        /// <value>
        /// The path for this <see cref="PortalMedia" />.
        /// </value>
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the date and time, in UTC, when this <see cref="PortalMedia" /> was last modified.
        /// </summary>
        /// <value>
        /// The date and time, in UTC, when this <see cref="PortalMedia" /> was last modified.
        /// </value>
        public DateTime ModifiedDate { get; set; } = DateTime.UtcNow;

        /// <summary>
        /// Gets the navigation property for child <see cref="PortalMedia" /> items.
        /// </summary>
        /// <value>
        /// The navigation property for child <see cref="PortalMedia" /> items.
        /// </value>
        public virtual ICollection<PortalMedia> Children { get; } = new List<PortalMedia>();
    }
}
