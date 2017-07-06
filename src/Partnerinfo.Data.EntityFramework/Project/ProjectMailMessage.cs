// Copyright (c) János Janka. All rights reserved.

using System;

namespace Partnerinfo.Project.EntityFramework
{
    public class ProjectMailMessage
    {
        /// <summary>
        /// MailMessage ID (Primary Key)
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Project ID (Foreign Key)
        /// </summary>
        public int ProjectId { get; set; }

        /// <summary>
        /// Project which owns this mail message
        /// </summary>
        public virtual ProjectEntity Project { get; set; }

        /// <summary>
        /// Mail subject
        /// </summary>
        public string Subject { get; set; }

        /// <summary>
        /// Mail body
        /// </summary>
        public string Body { get; set; }

        /// <summary>
        /// DateTime in UTC when this mail message was last modified
        /// </summary>
        public DateTime ModifiedDate { get; set; } = DateTime.UtcNow;
    }
}
