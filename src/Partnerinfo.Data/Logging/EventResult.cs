// Copyright (c) János Janka. All rights reserved.

using System;

namespace Partnerinfo.Logging
{
    public class EventResult : EventResultBase
    {
        /// <summary>
        /// User
        /// </summary>
        [Newtonsoft.Json.JsonIgnore]
        public AccountItem User { get; set; }

        /// <summary>
        /// Event Category
        /// </summary>
        public UniqueItem Category { get; set; }

        /// <summary>
        /// Object Type
        /// </summary>
        public ObjectType ObjectType { get; set; }

        /// <summary>
        /// Object Type
        /// </summary>
        public UniqueItem Object { get; set; }

        /// <summary>
        /// Contact
        /// </summary>
        public AccountItem Contact { get; set; }

        /// <summary>
        /// Contact state
        /// </summary>
        public ObjectState ContactState { get; set; }

        /// <summary>
        /// Correlation
        /// </summary>
        public EventResultBase Correlation { get; set; }

        /// <summary>
        /// Project
        /// </summary>
        public UniqueItem Project { get; set; }

        /// <summary>
        /// DateTime in UTC format
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
        /// Client identifier
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
    }
}
