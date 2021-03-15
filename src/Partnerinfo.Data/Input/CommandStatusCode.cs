// Copyright (c) János Janka. All rights reserved.

namespace Partnerinfo.Input
{
    /// <summary>
    /// A state flag that can be used to indicate the exit state of an action flow.
    /// </summary>
    public enum CommandStatusCode : byte
    {
        /// <summary>
        /// Indicates that the command successfully performed but there is no a return value for the action.
        /// </summary>
        NoAction = 0,

        /// <summary>
        /// Indicates that the command successfully performed and there is a return value for the action.
        /// </summary>
        Success = 1,

        /// <summary>
        /// Indicates that the user doesn't have any access rights for a resource.
        /// </summary>
        Forbidden = 2
    }
}
