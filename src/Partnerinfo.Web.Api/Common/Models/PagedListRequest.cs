// Copyright (c) János Janka. All rights reserved.

using System.ComponentModel.DataAnnotations;

namespace Partnerinfo.Models
{
    public class PagedListRequest
    {
        /// <summary>
        /// The index of the page of results to return. Use 1 to indicate the first page.
        /// </summary>
        public int Page { get; set; } = 1;

        /// <summary>
        /// The size of the page of results to return. The page index is non-zero-based.
        /// </summary>
        [Range(0, 100)]
        public int Count { get; set; } = 50;
    }
}