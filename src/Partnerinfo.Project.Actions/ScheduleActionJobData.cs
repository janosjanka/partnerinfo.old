// Copyright (c) János Janka. All rights reserved.

using System;
using Partnerinfo.Logging;

namespace Partnerinfo.Project.Actions
{
    public sealed class ScheduleActionJobData
    {
        /// <summary>
        /// Project ID
        /// </summary>
        public int ProjectId { get; set; }

        /// <summary>
        /// Action ID
        /// </summary>
        public int ActionId { get; set; }

        /// <summary>
        /// Access token
        /// </summary>
        public AuthTicket AuthTicket { get; set; }

        /// <summary>
        /// User ID which identifiers an anonymous user
        /// </summary>
        public Guid? AnonymId { get; set; }

        /// <summary>
        /// Contact
        /// </summary>
        public ContactItem Contact { get; set; }

        /// <summary>
        /// Contact state
        /// </summary>
        public ObjectState ContactState { get; set; }

        /// <summary>
        /// Other properties that can be injected to the pipeline
        /// </summary>
        public PropertyDictionary Properties { get; set; }

        /// <summary>
        /// Log event
        /// </summary>
        public EventItem Event { get; set; }
    }
}
