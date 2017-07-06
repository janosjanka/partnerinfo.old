// Copyright (c) János Janka. All rights reserved.

using System;

namespace Partnerinfo.Project
{
    public class BusinessTagItem : UniqueItem
    {
        /// <summary>
        /// Gets or sets the project which owns this <see cref="BusinessTagItem" />.
        /// </summary>
        /// <value>
        /// The project which owns this <see cref="BusinessTagItem" />.
        /// </value>
        public UniqueItem Project { get; set; }

        /// <summary>
        /// Gets or sets the date and time, in UTC, when this <see cref="BusinessTagItem" /> was created.
        /// </summary>
        /// <value>
        /// The date and time, in UTC, when this <see cref="BusinessTagItem" /> was created.
        /// </value>
        public DateTime CreatedDate { get; set; } = DateTime.UtcNow;

        /// <summary>
        /// Gets or sets the color for this <see cref="BusinessTagItem" />
        /// </summary>
        /// <value>
        /// The color.
        /// </value>
        public string Color { get; set; }

        /// <summary>
        /// Gets or sets the number of items associated with this <see cref="BusinessTagItem" />.
        /// </summary>
        /// <value>
        /// The number of items associated with this <see cref="BusinessTagItem" />.
        /// </value>
        public int ItemCount { get; set; }
    }
}
