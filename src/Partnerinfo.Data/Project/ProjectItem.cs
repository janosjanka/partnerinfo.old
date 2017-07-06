// Copyright (c) János Janka. All rights reserved.

using System;
using Partnerinfo.Security;

namespace Partnerinfo.Project
{
    public class ProjectItem : SharedResourceItem
    {
        /// <summary>
        /// Gets the type of the ACE (Access Control Entry) for this <see cref="ProjectItem" />.
        /// </summary>
        public override AccessObjectType ObjectType => AccessObjectType.Project;

        /// <summary>
        /// Gets or sets the sender for this <see cref="ProjectItem" />.
        /// </summary>
        /// <value>
        /// The sender.
        /// </value>
        public MailAddressItem Sender { get; set; }

        /// <summary>
        /// Gets or sets the date and time, in UTC, when this <see cref="ProjectItem" /> was created.
        /// </summary>
        /// <value>
        /// The date and time, in UTC, when this <see cref="ProjectItem" /> was created.
        /// </value>
        public DateTime CreatedDate { get; set; } = DateTime.UtcNow;

        /// <summary>
        /// Gets or sets the date and time, in UTC, when this <see cref="ProjectItem" /> was last modified.
        /// </summary>
        /// <value>
        /// The date and time, in UTC, when this <see cref="ProjectItem" /> was last modified.
        /// </value>
        public DateTime ModifiedDate { get; set; } = DateTime.UtcNow;

        /// <summary>
        /// Gets the number of contacts registered in this project.
        /// </summary>
        /// <value>
        /// The contact count.
        /// </value>
        public int ContactCount { get; set; }
    }
}
