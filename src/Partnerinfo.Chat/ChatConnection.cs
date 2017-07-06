// Copyright (c) János Janka. All rights reserved.

namespace Partnerinfo.Chat
{
    public class ChatConnection
    {
        /// <summary>
        /// Room ID (Foreign Key)
        /// </summary>
        public string RoomId { get; set; }

        /// <summary>
        /// User name (Foreign Key)
        /// </summary>
        public string UserName { get; set; }
    }
}
