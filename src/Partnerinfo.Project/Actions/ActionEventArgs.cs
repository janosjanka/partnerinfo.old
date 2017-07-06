// Copyright (c) János Janka. All rights reserved.

using Partnerinfo.Logging;

namespace Partnerinfo.Project.Actions
{
    /// <summary>
    /// Represents an immutable action event argument.
    /// </summary>
    public sealed class ActionEventArgs
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ActionEventArgs" /> class.
        /// </summary>
        public ActionEventArgs()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ActionEventArgs" /> class.
        /// </summary>
        /// <param name="targetType">Type of the target.</param>
        /// <param name="targetId">The unique item identifier from the data source for the item.</param>
        public ActionEventArgs(ObjectType targetType, int targetId)
        {
            TargetType = targetType;
            TargetId = targetId;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ActionEventArgs" /> class.
        /// </summary>
        /// <param name="targetType">Type of the target.</param>
        /// <param name="targetId">The unique item identifier from the data source for the item.</param>
        /// <param name="contactId">The unique item identifier from the data source for the item.</param>
        public ActionEventArgs(ObjectType targetType, int targetId, int? contactId)
        {
            TargetType = targetType;
            TargetId = targetId;
            ContactId = contactId;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ActionEventArgs" /> class.
        /// </summary>
        /// <param name="targetType">Type of the target.</param>
        /// <param name="targetId">The unique item identifier from the data source for the item.</param>
        /// <param name="contactId">The unique item identifier from the data source for the item.</param>
        /// <param name="salt">The salt.</param>
        public ActionEventArgs(ObjectType targetType, int targetId, int? contactId, string salt)
        {
            TargetType = targetType;
            TargetId = targetId;
            ContactId = contactId;
            Salt = salt;
        }

        /// <summary>
        /// Gets the type of the target object.
        /// </summary>
        public ObjectType TargetType { get; set; }

        /// <summary>
        /// Gets the unique identifier of the target object.
        /// </summary>
        public int TargetId { get; set; }

        /// <summary>
        /// Gets the unique identifier of the user.
        /// </summary>
        public int? ContactId { get; set; }

        /// <summary>
        /// Gets the user-defined string identifier.
        /// </summary>
        public string Salt { get; set; }
    }
}
