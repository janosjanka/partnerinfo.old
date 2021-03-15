// Copyright (c) János Janka. All rights reserved.

namespace Partnerinfo.Security
{
    public class AccessRuleItem
    {
        /// <summary>
        /// Gets a value indicating whether this <see cref="AccessRuleItem" /> represents an anonymous user (anyone).
        /// </summary>
        /// <value>
        ///   <c>true</c> if anyone; otherwise, <c>false</c>.
        /// </value>
        public bool Anyone { get { return User == null; } }

        /// <summary>
        /// Gets or sets the user who owns the resource. A null value represents an anonymous user (anyone).
        /// </summary>
        /// <value>
        /// The user who owns the resource. A null value represents an anonymous user (anyone).
        /// </value>
        public AccountItem User { get; set; }

        /// <summary>
        /// Gets or sets the permission level for the item.
        /// </summary>
        /// <value>
        /// The permission level for the item.
        /// </value>
        public AccessPermission Permission { get; set; }

        /// <summary>
        /// Gets or sets the visibility level for the item.
        /// </summary>
        /// <value>
        /// The visibility level for the item.
        /// </value>
        public AccessVisibility Visibility { get; set; }
    }
}
