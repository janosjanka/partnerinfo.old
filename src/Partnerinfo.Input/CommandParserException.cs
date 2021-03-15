// Copyright (c) János Janka. All rights reserved.

using System;
using System.Runtime.Serialization;

namespace Partnerinfo.Input
{
    [Serializable]
    public class CommandParserException : Exception
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="CommandParserException" /> class.
        /// </summary>
        /// <param name="message">The message that describes the error.</param>
        public CommandParserException(string message)
            : base(message)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="CommandParserException" /> class.
        /// </summary>
        /// <param name="info">The <see cref="T:System.Runtime.Serialization.SerializationInfo" /> that holds the serialized object data about the exception being thrown.</param>
        /// <param name="context">The <see cref="T:System.Runtime.Serialization.StreamingContext" /> that contains contextual information about the source or destination.</param>
        protected CommandParserException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}
