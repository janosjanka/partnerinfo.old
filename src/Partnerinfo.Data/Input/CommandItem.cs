// Copyright (c) János Janka. All rights reserved.

using System;

namespace Partnerinfo.Input
{
    public class CommandItem : ResourceItem
    {
        /// <summary>
        /// DateTime in UTC when this Command was created
        /// </summary>
        public DateTime CreatedDate { get; set; }

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
