// Copyright (c) János Janka. All rights reserved.

using System;
using System.Collections.Generic;
using Partnerinfo.Logging.EntityFramework;

namespace Partnerinfo.Identity.EntityFramework
{
    public class IdentityUser
    {
        private MailAddressItem _email = MailAddressItem.None;

        /// <summary>
        /// User ID (Primary Key)
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Email
        /// </summary>
        public MailAddressItem Email
        {
            get { return _email; }
            set { _email = value ?? MailAddressItem.None; }
        }

        /// <summary>
        /// Gets or sets the first name of the user.
        /// </summary>
        public string FirstName { get; set; }

        /// <summary>
        /// Gets or sets the last name of the user.
        /// </summary>
        public string LastName { get; set; }

        /// <summary>
        /// Nick name
        /// </summary>
        public string NickName { get; set; }

        /// <summary>
        /// Gets or sets the gender of the user.
        /// </summary>
        public PersonGender Gender { get; set; }

        /// <summary>
        /// Gets or sets the birthday of the user.
        /// </summary>
        public DateTime? Birthday { get; set; }

        /// <summary>
        /// DateTime in UTC when this Project was created
        /// </summary>
        public DateTime CreatedDate { get; set; } = DateTime.UtcNow;

        /// <summary>
        /// DateTime in UTC when this Project was last modified
        /// </summary>
        public DateTime ModifiedDate { get; set; } = DateTime.UtcNow;

        /// <summary>
        /// DateTime in UTC when the last event is read
        /// </summary>
        public DateTime LastEventDate { get; set; } = DateTime.UtcNow;

        /// <summary>
        /// Gets or sets the IP address when the user was last authenticated.
        /// </summary>
        public string LastIPAddress { get; set; }

        /// <summary>
        /// Gets or sets the date and time when the user was last authenticated.
        /// </summary>
        public DateTime LastLoginDate { get; set; }

        /// <summary>
        /// Navigation property for events
        /// </summary>
        public virtual ICollection<LoggingEventSharing> Events { get; } = new List<LoggingEventSharing>();
    }
}
