// Copyright (c) János Janka. All rights reserved.

using System;
using System.ComponentModel.DataAnnotations;
using System.Web.Http;
using Partnerinfo.Media.EntityFramework;

namespace Partnerinfo.Media.Models
{
    public sealed class PaylistsQueryDto
    {
        /// <summary>
        /// Playlist name
        /// </summary>
        public string Name { get; set; }

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

    public sealed class ItemsByPlaylistQueryDto
    {
        /// <summary>
        /// Playlist ID (Foreign Key)
        /// </summary>
        [HttpBindRequired]
        public int PlaylistId { get; set; }

        /// <summary>
        /// Playlist item name
        /// </summary>
        public string Name { get; set; }

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

    public sealed class ItemsByContactQueryDto
    {
        /// <summary>
        /// Playlist ID (Foreign Key)
        /// </summary>
        [HttpBindRequired]
        public int ContactId { get; set; }

        /// <summary>
        /// Playlist item name
        /// </summary>
        public string Name { get; set; }

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

    public sealed class PlaylistItemDto
    {
        /// <summary>
        /// Playlist item name
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Media type
        /// </summary>
        public MediaType MediaType { get; set; }

        /// <summary>
        /// Media ID
        /// </summary>
        public string MediaId { get; set; }

        /// <summary>
        /// Media duration
        /// </summary>
        public int Duration { get; set; }

        /// <summary>
        /// Publish date
        /// </summary>
        public DateTime PublishDate { get; set; }
    }
}