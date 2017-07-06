// Copyright (c) János Janka. All rights reserved.

using System.ComponentModel.DataAnnotations;

namespace Partnerinfo.Portal.Models
{
    public class CreatePortalDto
    {
        /// <summary>
        /// Gets or sets the project which owns the <see cref="PortalItem" />.
        /// </summary>
        /// <value>
        /// The project which owns the <see cref="PortalItem" />.
        /// </value>
        public UniqueItem Project { get; set; }

        /// <summary>
        /// Gets or sets the URI (Unique Resource Identifier) of <see cref="PortalItem" />.
        /// </summary>
        /// <value>
        /// The URI (Unique Resource Identifier) of the <see cref="PortalItem" />.
        /// </value>
        [Required]
        [MaxLength(64)]
        [UriPartValidator]
        public string Uri { get; set; }

        /// <summary>
        /// Gets or sets the name of the <see cref="PortalItem" />.
        /// </summary>
        /// <value>
        /// The name of the <see cref="PortalItem" />.
        /// </value>
        [Required]
        [MaxLength(64)]
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the description of the <see cref="PortalItem" />.
        /// </summary>
        /// <value>
        /// The description of the <see cref="PortalItem" />.
        /// </value>
        [MaxLength(256)]
        public string Description { get; set; }

        /// <summary>
        /// Gets or sets the Google tracking code of the <see cref="PortalItem" />.
        /// </summary>
        /// <value>
        /// The Google tracking code of the <see cref="PortalItem" />.
        /// </value>
        [MaxLength(128)]
        public string GATrackingId { get; set; }

        /// <summary>
        /// Gets or sets the name of the template to be created.
        /// </summary>
        /// <value>
        /// The name of the template to be created.
        /// </value>
        public string Template { get; set; }
    }
}