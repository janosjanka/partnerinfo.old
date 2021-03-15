// Copyright (c) János Janka. All rights reserved.

using System;

namespace Partnerinfo.Chat
{
    /// <summary>
    /// Represents a light-weight, cachable chat message.
    /// </summary>
    public class ChatMessageInfo
    {
        /// <summary>
        /// Gets or sets the from user for this chat message.
        /// </summary>
        /// <value>
        /// The from user for this chat message.
        /// </value>
        public ChatUserInfo From { get; set; }

        /// <summary>
        /// Gets or sets the to user for this chat message.
        /// </summary>
        /// <value>
        /// The to user for this chat message.
        /// </value>
        public ChatUserInfo To { get; set; }

        /// <summary>
        /// Gets or sets the message body.
        /// </summary>
        /// <value>
        /// The message body.
        /// </value>
        public string Message { get; set; }

        /// <summary>
        /// Gets or sets the datetime in UTC when this message was created.
        /// </summary>
        /// <value>
        /// The datetime in UTC when this message was created.
        /// </value>
        public DateTime CreatedDate { get; set; } = DateTime.UtcNow;
    }
}
