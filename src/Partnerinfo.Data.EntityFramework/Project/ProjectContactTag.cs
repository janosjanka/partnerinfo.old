// Copyright (c) János Janka. All rights reserved.

using System;

namespace Partnerinfo.Project.EntityFramework
{
    public class ProjectContactTag
    {
        /// <summary>
        /// Business Tag ID (Foreign Key)
        /// </summary>
        public int BusinessTagId { get; set; }

        /// <summary>
        /// Contact ID (Foreign Key)
        /// </summary>
        public int ContactId { get; set; }

        /// <summary>
        /// DateTime in UTC when this Project was created
        /// </summary>
        public DateTime CreatedDate { get; set; } = DateTime.UtcNow;
    }
}
