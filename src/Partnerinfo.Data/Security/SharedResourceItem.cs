// Copyright (c) János Janka. All rights reserved.

using System.Collections.Generic;

namespace Partnerinfo.Security
{
    public class SharedResourceItem : ResourceItem
    {
        /// <summary>
        /// Gets the type of the ace.
        /// </summary>
        /// <value>
        /// The type of the ace.
        /// </value>
        public virtual AccessObjectType ObjectType { get; }

        /// <summary>
        /// Gets a collection of users who are owners of this <see cref="SharedResourceItem" />.
        /// </summary>
        /// <value>
        /// A collection of users who are owners of this <see cref="SharedResourceItem" /> .
        /// </value>
        public ICollection<AccountItem> Owners { get; set; } = new List<AccountItem>();
    }
}
