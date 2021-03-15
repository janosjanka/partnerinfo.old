// Copyright (c) János Janka. All rights reserved.

using Partnerinfo.Identity.EntityFramework;

namespace Partnerinfo.Security.EntityFramework
{
    public class SecurityAccessRule
    {
        /// <summary>
        /// ACE ID (Primary Key)
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Source type
        /// </summary>
        public AccessObjectType ObjectType { get; set; }

        /// <summary>
        /// Gets or sets the object identifier.
        /// </summary>
        /// <value>
        /// The object identifier.
        /// </value>
        public int ObjectId { get; set; }

        /// <summary>
        /// Gets or sets the user identifier.
        /// </summary>
        /// <value>
        /// The user identifier.
        /// </value>
        public int? UserId { get; set; }

        /// <summary>
        /// Gets or sets the user.
        /// </summary>
        /// <value>
        /// The user.
        /// </value>
        public virtual IdentityUser User { get; set; }

        /// <summary>
        /// Permission
        /// </summary>
        public AccessPermission Permission { get; set; }

        /// <summary>
        /// Visibility
        /// </summary>
        public AccessVisibility Visibility { get; set; }
    }
}
