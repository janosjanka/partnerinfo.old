// Copyright (c) János Janka. All rights reserved.

using System;

namespace Partnerinfo.Logging
{
    public class EventResultBase : UniqueItem
    {
        /// <summary>
        /// DateTime in UTC format
        /// </summary>
        public DateTime StartDate { get; set; }
    }
}
