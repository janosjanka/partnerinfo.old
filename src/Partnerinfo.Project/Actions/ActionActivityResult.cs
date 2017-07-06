// Copyright (c) János Janka. All rights reserved.

using System;
using System.Collections.Generic;
using Partnerinfo.Logging;

namespace Partnerinfo.Project.Actions
{
    public sealed class ActionActivityResult
    {
        internal ActionActivityResult() { }
        internal ActionActivityResult(ActionActivityStatusCode statusCode)
        {
            StatusCode = statusCode;
        }

        /// <summary>
        /// Session ID which identifies an anonym user
        /// </summary>
        public Guid? AnonymId { get; set; }

        /// <summary>
        /// Event object
        /// </summary>
        public EventItem Event { get; set; }

        /// <summary>
        /// Access token
        /// </summary>
        public AuthTicket Ticket { get; set; }

        /// <summary>
        /// Contact
        /// </summary>
        public ContactItem Contact { get; set; }

        /// <summary>
        /// Contact state
        /// </summary>
        public ObjectState ContactState { get; set; }

        /// <summary>
        /// Status code
        /// </summary>
        public ActionActivityStatusCode StatusCode { get; set; }

        /// <summary>
        /// Error messages
        /// </summary>
        public IList<string> Errors { get; set; }

        /// <summary>
        /// Return URL
        /// </summary>
        public string ReturnUrl { get; set; }

        /// <summary>
        /// Returns true if the contact is authenticated
        /// </summary>
        public bool IsAuthenticated => Ticket != null;

        /// <summary>
        /// Returns true if the action tree contains at least one log action (required for backward-compatibility)
        /// </summary>
        internal bool HasLogAction { get; set; }
    }
}
