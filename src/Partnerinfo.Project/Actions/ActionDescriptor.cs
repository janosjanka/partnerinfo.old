// Copyright (c) János Janka. All rights reserved.

namespace Partnerinfo.Project.Actions
{
    /// <summary>
    /// Represents an action with its metadata.
    /// </summary>
    internal sealed class ActionDescriptor
    {
        /// <summary>
        /// Gets the action activity.
        /// </summary>
        /// <value>
        /// The action activity.
        /// </value>
        public IActionActivity Activity { get; }

        /// <summary>
        /// Gets the action activity metadata that belongs to the action.
        /// </summary>
        /// <value>
        /// The action activity metadata.
        /// </value>
        public ActionActivityMetadata Metadata { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="ActionDescriptor" /> class.
        /// </summary>
        /// <param name="activity">The activity.</param>
        /// <param name="metadata">The metadata.</param>
        public ActionDescriptor(IActionActivity activity, ActionActivityMetadata metadata)
        {
            Activity = activity;
            Metadata = metadata;
        }
    }
}
