// Copyright (c) János Janka. All rights reserved.

using System.ComponentModel.DataAnnotations;

namespace Partnerinfo.Portal.Models
{
    public class PortalQueryDto
    {
        /// <summary>
        /// Gets or sets the name for the <see cref="PortalItem" /> to be found.
        /// </summary>
        /// <value>
        /// The name for the <see cref="PortalItem" /> to be found.
        /// </value>
        public string Name { get; set; }

        /// <summary>
        /// The order in which portals are returned in a result set.
        /// </summary>
        /// <value>
        /// The order by.
        /// </value>
        public PortalSortOrder OrderBy { get; set; } = PortalSortOrder.None;

        /// <summary>
        /// The index of the page of results to return. Use 1 to indicate the first page.
        /// </summary>
        /// <value>
        /// The page.
        /// </value>
        public int Page { get; set; } = 1;

        /// <summary>
        /// The size of the page of results to return. <see cref="Page" /> is non-zero-based.
        /// </summary>
        /// <value>
        /// The limit.
        /// </value>
        [Range(1, 50)]
        public int Limit { get; set; } = 50;

        /// <summary>
        /// The related fields to include in the query results.
        /// </summary>
        /// <value>
        /// The fields.
        /// </value>
        public PortalField Fields { get; set; } = PortalField.None;
    }
}