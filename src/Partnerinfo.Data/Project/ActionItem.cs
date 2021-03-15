// Copyright (c) János Janka. All rights reserved.

using System;
using System.Collections.Generic;
using Newtonsoft.Json.Linq;

namespace Partnerinfo.Project
{
    /// <summary>
    /// Represents an action in a project.
    /// </summary>
    public class ActionItem : UniqueItem
    {
        /// <summary>
        /// Gets or sets the project which owns this <see cref="ActionItem" />.
        /// </summary>
        /// <value>
        /// The project which owns this <see cref="ActionItem" />.
        /// </value>
        public UniqueItem Project { get; set; }

        /// <summary>
        /// Gets or sets the type of this <see cref="ActionItem" />.
        /// </summary>
        /// <value>
        /// The type.
        /// </value>
        public ActionType Type { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether this <see cref="ActionItem" /> is enabled.
        /// </summary>
        /// <value>
        ///   <c>true</c> if enabled; otherwise, <c>false</c>.
        /// </value>
        public bool Enabled { get; set; }

        /// <summary>
        /// Gets or sets a set of key/value pairs that can be used to configure this <see cref="ActionItem" />.
        /// </summary>
        /// <value>
        /// A set of key/value pairs.
        /// </value>
        public JObject Options { get; set; }

        /// <summary>
        /// Gets or sets the date and time, in UTC, when this <see cref="ActionItem" /> was last modified.
        /// </summary>
        /// <value>
        /// The date and time, in UTC, when this <see cref="ActionItem" /> was last modified.
        /// </value>
        public DateTime ModifiedDate { get; set; } = DateTime.UtcNow;

        /// <summary>
        /// Gets the navigation property for the child <see cref="ActionItem" />s.
        /// </summary>
        /// <value>
        /// The navigation property for the child <see cref="ActionItem" />s.
        /// </value>
        public ICollection<UniqueItem> Children { get; } = new List<UniqueItem>();
    }
}
