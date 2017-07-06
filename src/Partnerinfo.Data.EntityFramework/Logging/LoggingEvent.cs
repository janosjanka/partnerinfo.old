// Copyright (c) János Janka. All rights reserved.

using System;
using System.Collections.Generic;
using Partnerinfo.Project.EntityFramework;

namespace Partnerinfo.Logging.EntityFramework
{
    public class LoggingEvent
    {
        /// <summary>
        /// Event ID (Primary Key)
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Object Type ID
        /// </summary>
        public ObjectType ObjectType { get; set; }

        /// <summary>
        /// Object
        /// </summary>
        public int? ObjectId { get; set; }

        /// <summary>
        /// Contact ID (Foreign Key)
        /// </summary>
        public int? ContactId { get; set; }

        /// <summary>
        /// Contact
        /// </summary>
        public virtual ProjectContact Contact { get; set; }

        /// <summary>
        /// Contact state
        /// </summary>
        public ObjectState ContactState { get; set; }

        /// <summary>
        /// Project ID (Foreign Key)
        /// </summary>
        public int? ProjectId { get; set; }

        /// <summary>
        /// Project
        /// </summary>
        public virtual ProjectEntity Project { get; set; }

        /// <summary>
        /// Correlation ID (Foreign Key)
        /// </summary>
        public int? CorrelationId { get; set; }

        /// <summary>
        /// Correlation
        /// </summary>
        public virtual LoggingEvent Correlation { get; set; }

        /// <summary>
        /// DateTime in UTC when this Event was started
        /// </summary>
        public DateTime StartDate { get; set; } = DateTime.UtcNow;

        /// <summary>
        /// DateTime in UTC when this Event was finished
        /// </summary>
        public DateTime? FinishDate { get; set; }

        /// <summary>
        /// Browser type
        /// </summary>
        public BrowserBrand BrowserBrand { get; set; }

        /// <summary>
        /// Browser version
        /// </summary>
        public short BrowserVersion { get; set; }

        /// <summary>
        /// Mobile device type
        /// </summary>
        public MobileDevice MobileDevice { get; set; }

        /// <summary>
        /// Anonym ID which identifies an anonym user
        /// </summary>
        public Guid? AnonymId { get; set; }

        /// <summary>
        /// Client ID
        /// </summary>
        public string ClientId { get; set; }

        /// <summary>
        /// URI which is generated for the action link
        /// </summary>
        public string CustomUri { get; set; }

        /// <summary>
        /// HTTP Referrer URL
        /// </summary>
        public string ReferrerUrl { get; set; }

        /// <summary>
        /// The message
        /// </summary>
        public string Message { get; set; }

        /// <summary>
        /// Navigation property for users
        /// </summary>
        public virtual ICollection<LoggingEventSharing> Users { get; } = new List<LoggingEventSharing>();
    }
}
