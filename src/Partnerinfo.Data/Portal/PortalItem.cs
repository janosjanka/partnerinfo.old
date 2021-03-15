// Copyright (c) János Janka. All rights reserved.

using System;
using Partnerinfo.Security;

namespace Partnerinfo.Portal
{
    public class PortalItem : SharedResourceItem
    {
        /// <summary>
        /// Gets the type of the ACE (Access Control Entry) for this <see cref="PortalItem" />.
        /// </summary>
        public override AccessObjectType ObjectType => AccessObjectType.Portal;

        /// <summary>
        /// Gets or sets the project.
        /// </summary>
        /// <value>
        /// The project.
        /// </value>
        public UniqueItem Project { get; set; }

        /// <summary>
        /// Gets or sets the home page.
        /// </summary>
        /// <value>
        /// The home page.
        /// </value>
        public ResourceItem HomePage { get; set; }

        /// <summary>
        /// Gets or sets the master page.
        /// </summary>
        /// <value>
        /// The master page.
        /// </value>
        public ResourceItem MasterPage { get; set; }

        /// <summary>
        /// Gets or sets the domain of this <see cref="PortalItem" />.
        /// </summary>
        /// <value>
        /// The domain.
        /// </value>
        public string Domain { get; set; }

        /// <summary>
        /// Gets or sets the description for this <see cref="PortalItem" />.
        /// </summary>
        /// <value>
        /// The description for this <see cref="PortalItem" />.
        /// </value>
        public string Description { get; set; }

        /// <summary>
        /// Gets or sets the Google tracking code for this <see cref="PortalItem" />.
        /// </summary>
        /// <value>
        /// The Google tracking code for this <see cref="PortalItem" />.
        /// </value>
        public string GATrackingId { get; set; }

        /// <summary>
        /// Gets or sets <see cref="DateTime" /> in UTC when this <see cref="PortalItem" /> was created.
        /// </summary>
        /// <value>
        /// The created date.
        /// </value>
        public DateTime CreatedDate { get; set; }

        /// <summary>
        /// Gets or sets <see cref="DateTime" /> in UTC when this <see cref="PortalItem" /> was last modified.
        /// </summary>
        /// <value>
        /// The modified date.
        /// </value>
        public DateTime ModifiedDate { get; set; }
    }
}
