// Copyright (c) János Janka. All rights reserved.

namespace Partnerinfo.Portal.ViewModels
{
    public sealed class EnginePortalViewModel
    {
        /// <summary>
        /// Gets or sets the unique resource identifier (URI) for the item provided by the storage.
        /// </summary>
        /// <value>
        /// The unique resource identifier for the item provided by the storage.
        /// </value>
        public string Uri { get; set; }

        /// <summary>
        /// Gets or sets the name for the item.
        /// </summary>
        /// <value>
        /// The name for the item.
        /// </value>
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the description of this <see cref="PortalItem" />.
        /// </summary>
        /// <value>
        /// The description.
        /// </value>
        public string Description { get; set; }

        /// <summary>
        /// Gets or sets the tracking code of Google Analytics.
        /// </summary>
        /// <value>
        /// The tracking code.
        /// </value>
        public string GATrackingId { get; set; }

        /// <summary>
        /// Gets or sets the <see cref="UniqueItem" />.
        /// </summary>
        /// <value>
        /// The <see cref="UniqueItem" />.
        /// </value>
        public UniqueItem Project { get; set; }
    }
}