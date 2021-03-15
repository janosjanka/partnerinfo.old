// Copyright (c) János Janka. All rights reserved.

using System;
using System.Collections.Generic;

namespace Partnerinfo.Drive
{
    public class FileItem
    {
        /// <summary>
        /// Gets or sets the primary key for this <see cref="FileItem" />.
        /// </summary>
        /// <value>
        /// The primary key.
        /// </value>
        public int Id { get; set; }

        /// <summary>
        /// Gets or sets the parent identifier.
        /// </summary>
        /// <value>
        /// The parent identifier.
        /// </value>
        public int? ParentId { get; set; }

        /// <summary>
        /// Gets or sets the owner of the document.
        /// </summary>
        /// <value>
        /// The owner identifier.
        /// </value>
        public int OwnerId { get; set; }

        /// <summary>
        /// Folder | File
        /// </summary>
        public FileType Type { get; set; }

        /// <summary>
        /// File name
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// File size
        /// </summary>
        public int Length { get; set; }

        /// <summary>
        /// Human-readable identifier
        /// </summary>
        public string Slug { get; set; }

        /// <summary>
        /// Physical path
        /// </summary>
        public string PhysicalPath { get; set; }

        /// <summary>
        /// DateTime in UTC when this document was created
        /// </summary>
        public DateTime CreatedDate { get; set; } = DateTime.UtcNow;

        /// <summary>
        /// DateTime in UTC when this document was written
        /// </summary>
        public DateTime ModifiedDate { get; set; } = DateTime.UtcNow;

        /// <summary>
        /// Children
        /// </summary>
        /// <value>
        /// The children.
        /// </value>
        public virtual ICollection<FileItem> Children { get; } = new List<FileItem>();
    }
}
