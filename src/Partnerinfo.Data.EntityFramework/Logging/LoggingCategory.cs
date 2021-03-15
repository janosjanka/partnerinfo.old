// Copyright (c) János Janka. All rights reserved.

using System;
using System.Collections.Generic;
using Partnerinfo.Identity.EntityFramework;
using Partnerinfo.Project.EntityFramework;

namespace Partnerinfo.Logging.EntityFramework
{
    public class LoggingCategory
    {
        /// <summary>
        /// Event Category ID (Primary Key)
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// User ID (Foreign Key)
        /// </summary>
        public int OwnerId { get; set; }

        /// <summary>
        /// User who owns this event category
        /// </summary>
        public virtual IdentityUser Owner { get; set; }

        /// <summary>
        /// Project ID (Foreign Key)
        /// </summary>
        public int? ProjectId { get; set; }

        /// <summary>
        /// Project
        /// </summary>
        public virtual ProjectEntity Project { get; set; }

        /// <summary>
        /// Category name
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Category color
        /// </summary>
        public int Color { get; set; }

        /// <summary>
        /// DateTime in UTC when this category was last modified
        /// </summary>
        public DateTime ModifiedDate { get; set; } = DateTime.UtcNow;

        /// <summary>
        /// Navigation property for events
        /// </summary>
        public virtual ICollection<LoggingEventSharing> Events { get; } = new List<LoggingEventSharing>();
    }
}
