// Copyright (c) János Janka. All rights reserved.

using System.Collections.Generic;

namespace Partnerinfo.Project.EntityFramework
{
    public class ProjectBusinessTag
    {
        /// <summary>
        /// Tag ID (Primary Key)
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Project ID (Foreign Key)
        /// </summary>
        public int ProjectId { get; set; }

        /// <summary>
        /// Project which owns this tag
        /// </summary>
        public virtual ProjectEntity Project { get; set; }

        /// <summary>
        /// Tag name
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Color code
        /// </summary>
        public string Color { get; set; }

        /// <summary>
        /// Navigation property for contacts
        /// </summary>
        public virtual ICollection<ProjectContactTag> Contacts { get; } = new List<ProjectContactTag>();
    }
}
