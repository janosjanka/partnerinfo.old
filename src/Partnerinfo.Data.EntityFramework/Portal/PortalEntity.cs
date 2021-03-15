// Copyright (c) János Janka. All rights reserved.

using System;
using System.Collections.Generic;
using Partnerinfo.Project.EntityFramework;

namespace Partnerinfo.Portal.EntityFramework
{
    public class PortalEntity
    {
        /// <summary>
        /// Gets or sets the primary key for this <see cref="PortalEntity" />.
        /// </summary>
        /// <value>
        /// The primary key.
        /// </value>
        public int Id { get; set; }

        /// <summary>
        /// Project ID (Foreign Key)
        /// </summary>
        public int? ProjectId { get; set; }

        /// <summary>
        /// Project
        /// </summary>
        public virtual ProjectEntity Project { get; set; }

        /// <summary>
        /// Home Page ID (Foreign Key)
        /// </summary>
        public int? HomePageId { get; set; }

        /// <summary>
        /// Home Page
        /// </summary>
        public virtual PortalPage HomePage { get; set; }

        /// <summary>
        /// Master Page ID (Foreign Key)
        /// </summary>
        public int? MasterPageId { get; set; }

        /// <summary>
        /// Master page
        /// </summary>
        public virtual PortalPage MasterPage { get; set; }

        /// <summary>
        /// Gets or sets the part of a URL which identifies this <see cref="PortalEntity" /> using human-readable keywords.
        /// </summary>
        /// <value>
        /// The part of a URL.
        /// </value>
        public string Uri { get; set; }

        /// <summary>
        /// Gets or sets the domain of this <see cref="PortalEntity" />.
        /// </summary>
        /// <value>
        /// The domain.
        /// </value>
        public string Domain { get; set; }

        /// <summary>
        /// Gets or sets the <see cref="DateTime" /> in UTC when this <see cref="PortalEntity" /> was created.
        /// </summary>
        /// <value>
        /// The <see cref="DateTime" />.
        /// </value>
        public DateTime CreatedDate { get; set; } = DateTime.UtcNow;

        /// <summary>
        /// Gets or sets the <see cref="DateTime" /> in UTC when this <see cref="PortalEntity" /> was last modified.
        /// </summary>
        /// <value>
        /// The <see cref="DateTime" />.
        /// </value>
        public DateTime ModifiedDate { get; set; } = DateTime.UtcNow;

        /// <summary>
        /// Gets or sets the name of this <see cref="PortalEntity" />.
        /// </summary>
        /// <value>
        /// The name.
        /// </value>
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the description of this <see cref="PortalEntity" />.
        /// </summary>
        /// <value>
        /// The description.
        /// </value>
        public string Description { get; set; }

        /// <summary>
        /// Gets or sets the tracking code of Google Analytics.
        /// </summary>
        /// <value>
        /// The tracking code.
        /// </value>
        public string GATrackingId { get; set; }

        /// <summary>
        /// Navigation property for media
        /// </summary>
        public virtual ICollection<PortalMedia> Media { get; } = new List<PortalMedia>();

        /// <summary>
        /// Navigation property for pages
        /// </summary>
        public virtual ICollection<PortalPage> Pages { get; } = new List<PortalPage>();
    }
}