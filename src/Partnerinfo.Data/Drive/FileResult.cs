// Copyright (c) János Janka. All rights reserved.

using System;
using System.Collections.Generic;

namespace Partnerinfo.Drive
{
    public class FileResult
    {
        /// <summary>
        /// Gets or sets the unique identifier for the unique resource.
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Gets or sets the unique identifier of the parent for the unique resource.
        /// </summary>
        public int? ParentId { get; set; }

        /// <summary>
        /// Gets or sets the owner of the document.
        /// </summary>
        /// <value>
        /// The owner identifier.
        /// </value>
        public int OwnerId { get; set; }

        /// <summary>
        /// Gets or sets the human-readable name of the unique resource.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// File type
        /// </summary>
        public FileType Type { get; set; }

        /// <summary>
        /// The length of the document
        /// </summary>
        public int Length { get; set; }

        /// <summary>
        /// URI
        /// </summary>
        public string Slug { get; set; }

        /// <summary>
        /// Physical path
        /// </summary>
        public string PhysicalPath { get; set; }

        /// <summary>
        /// Absolute resource link
        /// </summary>
        public string PublicLink { get; set; }

        /// <summary>
        /// Absolute resource link
        /// </summary>
        public string PrivateLink { get; set; }

        /// <summary>
        /// DateTime in UTC when this Project was created
        /// </summary>
        public DateTime CreatedDate { get; set; }

        /// <summary>
        /// DateTime in UTC when this Project was last modified
        /// </summary>
        public DateTime ModifiedDate { get; set; }

        /// <summary>
        /// Gets or sets the owners (ACL) of the unique resource.
        /// </summary>
        public ICollection<AccountItem> Owners { get; set; }
    }
}
