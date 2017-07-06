// Copyright (c) János Janka. All rights reserved.

using Newtonsoft.Json;

namespace Partnerinfo.Chat
{
    /// <summary>
    /// Represents a light-weight, cachable version of a user profile.
    /// </summary>
    public class ChatUserInfo
    {
        /// <summary>
        /// Room ID (Foreign Key)
        /// </summary>
        [JsonIgnore]
        public string RoomId { get; set; }

        /// <summary>
        /// Contact ID (Foreign Key)
        /// </summary>
        [JsonIgnore]
        public int? ContactId { get; set; }

        /// <summary>
        /// Client ID
        /// </summary>
        [JsonIgnore]
        public string ClientId { get; set; }

        /// <summary>
        /// IP Address
        /// </summary>
        [JsonIgnore]
        public string IPAddress { get; set; }

        /// <summary>
        /// User name
        /// </summary>
        public string UserName { get; set; }

        /// <summary>
        /// Nick name
        /// </summary>
        public string NickName { get; set; }

        /// <summary>
        /// True if this message is from an admin
        /// </summary>
        public bool Admin { get; set; }
    }
}
