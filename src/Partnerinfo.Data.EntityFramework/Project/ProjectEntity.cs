// Copyright (c) János Janka. All rights reserved.

using System;
using System.Collections.Generic;

namespace Partnerinfo.Project.EntityFramework
{
    public class ProjectEntity
    {
        /// <summary>
        /// Project ID (Primary Key)
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Project name
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Sender's email address
        /// </summary>
        public MailAddressItem Sender { get; set; } = MailAddressItem.None;

        /// <summary>
        /// DateTime in UTC when this Project was created
        /// </summary>
        public DateTime CreatedDate { get; set; } = DateTime.UtcNow;

        /// <summary>
        /// DateTime in UTC when this Project was last modified
        /// </summary>
        public DateTime ModifiedDate { get; set; } = DateTime.UtcNow;

        /// <summary>
        /// Navigation property for <see cref="ProjectAction" />s.
        /// </summary>
        public virtual ICollection<ProjectAction> Actions { get; } = new List<ProjectAction>();

        /// <summary>
        /// Navigation property for <see cref="ProjectBusinessTag" />s.
        /// </summary>
        public virtual ICollection<ProjectBusinessTag> BusinessTags { get; } = new List<ProjectBusinessTag>();

        /// <summary>
        /// Navigation property for contacts
        /// </summary>
        public virtual ICollection<ProjectContact> Contacts { get; } = new List<ProjectContact>();

        /// <summary>
        /// Navigation property for <see cref="ProjectMailMessage" />s.
        /// </summary>
        public virtual ICollection<ProjectMailMessage> MailMessages { get; } = new List<ProjectMailMessage>();
    }
}
