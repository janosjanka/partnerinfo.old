// Copyright (c) János Janka. All rights reserved.

using Partnerinfo.Identity.EntityFramework;

namespace Partnerinfo.Logging.EntityFramework
{
    public class LoggingRule
    {
        /// <summary>
        /// Gets or sets the primary key of this <see cref="LoggingRule" />.
        /// </summary>
        /// <value>
        /// The primary key.
        /// </value>
        public int Id { get; set; }

        /// <summary>
        /// User ID (Foreign Key)
        /// </summary>
        public int UserId { get; set; }

        /// <summary>
        /// User
        /// </summary>
        public virtual IdentityUser User { get; set; }

        /// <summary>
        /// Options
        /// </summary>
        public string Options { get; set; }
    }
}
