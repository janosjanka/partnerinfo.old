// Copyright (c) János Janka. All rights reserved.

using System;
using System.Collections.Generic;

namespace Partnerinfo.Portal.EntityFramework
{
    public class PortalPage
    {
        /// <summary>
        /// Page ID (Primary Key)
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Portal ID (Foreign Key)
        /// </summary>
        public int PortalId { get; set; }

        /// <summary>
        /// Parent Page ID (Foreign Key)
        /// </summary>
        public int? ParentId { get; set; }

        /// <summary>
        /// Master Page ID (Foreign Key)
        /// </summary>
        public int? MasterId { get; set; }

        /// <summary>
        /// String ID
        /// </summary>
        public string Uri { get; set; }

        /// <summary>
        /// DateTime in UTC when this page was last modified
        /// </summary>
        public DateTime ModifiedDate { get; set; } = DateTime.UtcNow;

        /// <summary>
        /// Page name or title
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Meta description
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// Html content
        /// </summary>
        public string HtmlContent { get; set; }

        /// <summary>
        /// Style content
        /// </summary>
        public string StyleContent { get; set; }

        /// <summary>
        /// Gets or sets the serialized version of external resources.
        /// </summary>
        /// <value>
        /// The serialized version of external resources.
        /// </value>
        public string ReferenceList { get; set; }

        /// <summary>
        /// Portal
        /// </summary>
        public virtual PortalEntity Portal { get; set; }

        /// <summary>
        /// Master Page
        /// </summary>
        public virtual PortalPage Master { get; set; }

        /// <summary>
        /// Navigation property for child pages
        /// </summary>
        public virtual ICollection<PortalPage> Children { get; } = new List<PortalPage>();

        /// <summary>
        /// Navigation property for content pages
        /// </summary>
        public virtual ICollection<PortalPage> Contents { get; } = new List<PortalPage>();
    }
}
