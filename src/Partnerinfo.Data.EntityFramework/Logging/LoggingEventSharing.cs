// Copyright (c) János Janka. All rights reserved.

namespace Partnerinfo.Logging.EntityFramework
{
    public class LoggingEventSharing
    {
        /// <summary>
        /// User ID (Foreign Key)
        /// </summary>
        public int UserId { get; set; }

        /// <summary>
        /// Event ID (Foreign Key)
        /// </summary>
        public int EventId { get; set; }

        /// <summary>
        /// Category ID (Foreign Key)
        /// </summary>
        public int? CategoryId { get; set; }
    }
}
