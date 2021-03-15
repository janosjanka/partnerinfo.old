// Copyright (c) János Janka. All rights reserved.

using System.ComponentModel.DataAnnotations;

namespace Partnerinfo.Project.Models
{
    public sealed class ProjectQueryDto
    {
        /// <summary>
        /// Project name
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// The order in which projects are returned in a result set.
        /// </summary>
        public ProjectSortOrder OrderBy { get; set; } = ProjectSortOrder.None;

        /// <summary>
        /// The index of the page of results to return. Use 1 to indicate the first page.
        /// </summary>
        public int Page { get; set; } = 1;

        /// <summary>
        /// The size of the page of results to return. <see cref="Page"/> is non-zero-based.
        /// </summary>
        [Range(1, 50)]
        public int Limit { get; set; } = 50;

        /// <summary>
        /// The related fields to include in the query results.
        /// </summary>
        public ProjectField Fields { get; set; } = ProjectField.None;
    }

    public sealed class ProjectItemDto
    {
        /// <summary>
        /// Project name
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Sender
        /// </summary>
        public MailAddressItem Sender { get; set; }
    }

    public sealed class ProjectResultDto
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
        /// Sender
        /// </summary>
        public MailAddressItem Sender { get; set; }
    }
}