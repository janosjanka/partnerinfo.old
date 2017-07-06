// Copyright (c) János Janka. All rights reserved.

using System;

namespace Partnerinfo.Identity
{
    public class UserItem : AccountItem
    {
        /// <summary>
        /// DateTime in UTC when this Project was created
        /// </summary>
        public DateTime CreatedDate { get; set; }

        /// <summary>
        /// DateTime in UTC when this Project was last modified
        /// </summary>
        public DateTime ModifiedDate { get; set; }

        /// <summary>
        /// DateTime in UTC when the last event is read
        /// </summary>
        public DateTime LastEventDate { get; set; }

        /// <summary>
        /// Gets or sets the IP address when the user was last authenticated.
        /// </summary>
        public string LastIPAddress { get; set; }

        /// <summary>
        /// Gets or sets the date and time when the user was last authenticated.
        /// </summary>
        public DateTime LastLoginDate { get; set; }
    }
}
