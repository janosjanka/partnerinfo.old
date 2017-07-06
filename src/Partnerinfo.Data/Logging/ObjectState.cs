// Copyright (c) János Janka. All rights reserved.

namespace Partnerinfo.Logging
{
    public enum ObjectState : byte
    {
        /// <summary>
        /// Data is unchanged
        /// </summary>
        Unchanged = 0,

        /// <summary>
        /// Data is added to the database
        /// </summary>
        Added = 1,

        /// <summary>
        /// Data is modified
        /// </summary>
        Modified = 2,

        /// <summary>
        /// Data is deleted
        /// </summary>
        Deleted = 3
    }
}
