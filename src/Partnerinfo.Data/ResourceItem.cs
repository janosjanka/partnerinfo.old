// Copyright (c) János Janka. All rights reserved.

namespace Partnerinfo
{
    /// <summary>
    /// Represents a resource in the system.
    /// </summary>
    public class ResourceItem : UniqueItem
    {
        /// <summary>
        /// Gets or sets the URI (Unique Resource Identifier) for this <see cref="ResourceItem" /> provided by the storage.
        /// </summary>
        /// <value>
        /// The URI (Unique Resource Identifier).
        /// </value>
        public string Uri { get; set; }

        /// <summary>
        /// Returns a <see cref="string" /> that represents this instance.
        /// </summary>
        /// <returns>
        /// A <see cref="string" /> that represents this instance.
        /// </returns>
        public override string ToString() => $"Id = {Id}, Name = '{Name}', Uri = '{Uri}'";
    }
}
