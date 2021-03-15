// Copyright (c) János Janka. All rights reserved.

namespace Partnerinfo.Logging
{
    public class MessageResult : EventResultBase
    {
        /// <summary>
        /// Contact
        /// </summary>
        public AccountItem Contact { get; set; }

        /// <summary>
        /// Client ID
        /// </summary>
        public string ClientId { get; set; }

        /// <summary>
        /// Custom URI
        /// </summary>
        public string CustomUri { get; set; }

        /// <summary>
        /// Event message
        /// </summary>
        public string Message { get; set; }
    }
}
