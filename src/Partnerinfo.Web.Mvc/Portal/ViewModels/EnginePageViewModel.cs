// Copyright (c) János Janka. All rights reserved.

using System.Collections.Generic;
using Newtonsoft.Json;

namespace Partnerinfo.Portal.ViewModels
{
    public sealed class EnginePageViewModel
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
        /// Gets or sets the description of this <see cref="PageItem" />.
        /// </summary>
        /// <value>
        /// The description.
        /// </value>
        public string Description { get; set; }

        /// <summary>
        /// Gets or sets the HTML content of this <see cref="PageItem" />.
        /// </summary>
        /// <value>
        /// The HTML content.
        /// </value>
        [JsonIgnore]
        public string HtmlContent { get; set; }

        /// <summary>
        /// Gets or sets the CSS content of this <see cref="PageItem" />.
        /// </summary>
        /// <value>
        /// The CSS content.
        /// </value>
        [JsonIgnore]
        public string StyleContent { get; set; }

        /// <summary>
        /// Gets or sets a list of <see cref="ReferenceItem" />s to be rendered.
        /// </summary>
        /// <value>
        /// A list of references.
        /// </value>
        [JsonIgnore]
        public ICollection<ReferenceItem> References { get; set; }
    }
}