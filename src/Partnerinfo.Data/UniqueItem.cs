// Copyright (c) János Janka. All rights reserved.

namespace Partnerinfo
{
    /// <summary>
    /// Represents a unique item in the system.
    /// </summary>
    public class UniqueItem
    {
        /// <summary>
        /// Gets or sets the primary key for this <see cref="UniqueItem" />.
        /// </summary>
        /// <value>
        /// The primary key.
        /// </value>
        public int Id { get; set; }

        /// <summary>
        /// Gets or sets the parent key for this <see cref="UniqueItem" />.
        /// </summary>
        /// <value>
        /// The parent key.
        /// </value>
        public UniqueItem Parent { get; set; }

        /// <summary>
        /// Gets or sets a human-readable name for the <see cref="UniqueItem" />.
        /// </summary>
        /// <value>
        /// The human-readable name.
        /// </value>
        public string Name { get; set; }

        /// <summary>
        /// Returns a <see cref="string" /> that represents this instance.
        /// </summary>
        /// <returns>
        /// A <see cref="string" /> that represents this instance.
        /// </returns>
        public override string ToString() => $"Id = {Id}, Name = '{Name}'";
    }
}
