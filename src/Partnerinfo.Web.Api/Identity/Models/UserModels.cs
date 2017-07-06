// Copyright (c) János Janka. All rights reserved.

using System.ComponentModel.DataAnnotations;

namespace Partnerinfo.Identity.Models
{
    public sealed class UserQueryDto
    {
        /// <summary>
        /// Project name
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// The order in which projects are returned in a result set.
        /// </summary>
        public UserSortOrder OrderBy { get; set; } = UserSortOrder.None;

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
        public UserField Fields { get; set; } = UserField.None;
    }
}