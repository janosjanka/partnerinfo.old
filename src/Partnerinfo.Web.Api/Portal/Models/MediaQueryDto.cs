// Copyright (c) János Janka. All rights reserved.

using System.Web.Http;

namespace Partnerinfo.Portal.Models
{
    public class MediaQueryDto
    {
        /// <summary>
        /// Gets or sets the portal URI to be found.
        /// </summary>
        /// <value>
        /// The portal URI.
        /// </value>
        [HttpBindRequired]
        public string PortalUri { get; set; }

        /// <summary>
        /// Gets or sets the media URI to be found.
        /// </summary>
        /// <value>
        /// The media URI.
        /// </value>
        public string MediaUri { get; set; }

        /// <summary>
        /// Gets or sets the name for the <see cref="MediaItem" /> to be found.
        /// </summary>
        /// <value>
        /// The name for the <see cref="MediaItem" /> to be found.
        /// </value>
        public string Name { get; set; }

        /// <summary>
        /// The order in which portals are returned in a result set.
        /// </summary>
        /// <value>
        /// The order by.
        /// </value>
        public MediaSortOrder OrderBy { get; set; } = MediaSortOrder.None;

        /// <summary>
        /// The related fields to include in the query results.
        /// </summary>
        /// <value>
        /// The fields.
        /// </value>
        public MediaField Fields { get; set; } = MediaField.None;
    }
}