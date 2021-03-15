// Copyright (c) János Janka. All rights reserved.

using System;

namespace Partnerinfo.Logging
{
    public class EventItem : UniqueItem
    {
        /// <summary>
        /// Object Type ID
        /// </summary>
        public ObjectType ObjectType { get; set; }

        /// <summary>
        /// Object
        /// </summary>
        public int? ObjectId { get; set; }

        /// <summary>
        /// Contact
        /// </summary>
        public AccountItem Contact { get; set; }

        /// <summary>
        /// Contact state
        /// </summary>
        public ObjectState ContactState { get; set; }

        /// <summary>
        /// Project
        /// </summary>
        public UniqueItem Project { get; set; }

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
    }
}
