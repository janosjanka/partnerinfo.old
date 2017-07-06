// Copyright (c) János Janka. All rights reserved.

using System;
using System.Collections.Generic;

namespace Partnerinfo.Project.EntityFramework
{
    public class ProjectAction
    {
        /// <summary>
        /// Action ID (Primary Key)
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Parent ID (Foreign Key)
        /// </summary>
        public int? ParentId { get; set; }

        /// <summary>
        /// Project ID (Foreign Key)
        /// </summary>
        public int ProjectId { get; set; }

        /// <summary>
        /// Project which owns this action
        /// </summary>
        public virtual ProjectEntity Project { get; set; }

        /// <summary>
        /// Action type
        /// </summary>
        public ActionType Type { get; set; }

        /// <summary>
        /// Sort Order
        /// </summary>
        public byte SortOrder { get; set; }

        /// <summary>
        /// Returns true if this action is enabled
        /// </summary>
        public bool Enabled { get; set; } = true;

        /// <summary>
        /// DateTime in UTC when this playlist was last modified
        /// </summary>
        public DateTime ModifiedDate { get; set; } = DateTime.UtcNow;

        /// <summary>
        /// Action name (optional)
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Options
        /// </summary>
        public string Options { get; set; }

        /// <summary>
        /// Navigation property for children
        /// </summary>
        public virtual ICollection<ProjectAction> Children { get; } = new List<ProjectAction>();
    }
}
