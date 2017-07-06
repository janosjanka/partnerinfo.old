// Copyright (c) János Janka. All rights reserved.

namespace Partnerinfo
{
    /// <summary>
    /// Represents a validation error.
    /// </summary>
    public sealed class FaultMember
    {
        /// <summary>
        /// Gets or sets the name of the property.
        /// </summary>
        /// <value>
        /// The name of the property.
        /// </value>
        public string Name { get; }

        /// <summary>
        /// Gets or sets the error message.
        /// </summary>
        /// <value>
        /// The error message.
        /// </value>
        public string Message { get; }

        /// <summary>
        /// Prevents a default instance of the <see cref="FaultMember" /> class from being created.
        /// </summary>
        private FaultMember()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="FaultMember" /> class.
        /// </summary>
        /// <param name="name">Name of the property.</param>
        /// <param name="message">The error message.</param>
        public FaultMember(string name, string message)
        {
            Name = name;
            Message = message;
        }
    }
}