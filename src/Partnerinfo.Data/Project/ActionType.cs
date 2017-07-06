// Copyright (c) János Janka. All rights reserved.

namespace Partnerinfo.Project
{
    public enum ActionType : byte
    {
        /// <summary>
        /// Indicates that the action type is not being used.
        /// </summary>
        Unknown = 0,

        // Control Flow ----------------------------------------------------------

        /// <summary>
        /// A control flow that returns with a HTTP redirect URI
        /// </summary>
        Redirect = 1,

        /// <summary>
        /// A control flow that executes actions senquentially
        /// </summary>
        Sequence = 2,

        /// <summary>
        /// A control flow that executes actions on a scheduler thread
        /// </summary>
        Schedule = 3,

        /// <summary>
        /// Executes contained actions based on a specified condition
        /// </summary>
        Condition = 4,

        // Contact ---------------------------------------------------------------

        /// <summary>
        /// Authenticates a request
        /// </summary>
        Authenticate = 50,

        /// <summary>
        /// Inserts a new contact
        /// </summary>
        Register = 51,

        /// <summary>
        /// Deletes a contact permanently
        /// </summary>
        Unregister = 52,

        /// <summary>
        /// Represents a tagging action
        /// </summary>
        SetTags = 53,

        /// <summary>
        /// Represents a mail action
        /// </summary>
        SendMail = 54,

        // Reporting -------------------------------------------------------------

        /// <summary>
        /// Adds a new log entry to the table
        /// </summary>
        Log = 100
    }
}
