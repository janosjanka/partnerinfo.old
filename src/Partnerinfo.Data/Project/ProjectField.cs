// Copyright (c) János Janka. All rights reserved.

using System;

namespace Partnerinfo.Project
{
    [Flags]
    public enum ProjectField : byte
    {
        /// <summary>
        /// No extra fields included in the result set.
        /// </summary>
        None = 0,

        /// <summary>
        /// Owners are included in the result set. 
        /// </summary>
        Owners = 1 << 0,

        /// <summary>
        /// Statistics are included in the result set.
        /// </summary>
        Statistics = 1 << 1,

        /// <summary>
        /// All of the fields are included in the result set.
        /// </summary>
        All = Owners | Statistics
    }
}
