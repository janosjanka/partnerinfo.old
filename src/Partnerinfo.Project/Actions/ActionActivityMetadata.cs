// Copyright (c) János Janka. All rights reserved.

namespace Partnerinfo.Project.Actions
{
    public sealed class ActionActivityMetadata
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ActionActivityMetadata" /> class.
        /// </summary>
        /// <param name="type">The type of the action that usually represents an operation.</param>
        public ActionActivityMetadata(ActionType type)
        {
            Type = type;
        }

        /// <summary>
        /// The type of the action that usually represents an operation.
        /// </summary>
        public ActionType Type { get; }
    }
}
