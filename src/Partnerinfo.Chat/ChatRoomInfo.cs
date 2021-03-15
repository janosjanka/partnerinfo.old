// Copyright (c) János Janka. All rights reserved.

using System.Collections.Generic;

namespace Partnerinfo.Chat
{
    /// <summary>
    /// Represents a light-weight, cachable chat room type.
    /// </summary>
    public class ChatRoomInfo
    {
        /// <summary>
        /// Room ID (Primary Key)
        /// </summary>
        public string Id { get; set; }

        /// <summary>
        /// Page ID
        /// </summary>
        public int PageId { get; set; }

        /// <summary>
        /// Portal ID
        /// </summary>
        public int PortalId { get; set; }

        /// <summary>
        /// Project ID (Foreign Key)
        /// </summary>
        public int? ProjectId { get; set; }

        /// <summary>
        /// Customer Service User
        /// </summary>
        public ChatUserInfo Admin { get; set; }

        /// <summary>
        /// Identity users who have rights for sending messages
        /// </summary>
        public IDictionary<string, AccountItem> Users { get; } = new Dictionary<string, AccountItem>();
    }
}
