// Copyright (c) János Janka. All rights reserved.

using System;

namespace Partnerinfo.Input.EntityFramework
{
    public class CommandEntity
    {
        /// <summary>
        /// Command ID (Primary Key)
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// The part of a URL which identifies the command using human-readable keywords
        /// </summary>
        public string Uri { get; set; }

        /// <summary>
        /// DateTime in UTC when this Command was created
        /// </summary>
        public DateTime CreatedDate { get; set; } = DateTime.UtcNow;

        /// <summary>
        /// Command line
        /// </summary>
        public string Line { get; set; }

        /// <summary>
        /// Command data
        /// </summary>
        public string Data { get; set; }
    }
}
