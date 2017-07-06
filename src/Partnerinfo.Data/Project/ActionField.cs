// Copyright (c) János Janka. All rights reserved.

using System;

namespace Partnerinfo.Project
{
    [Flags]
    public enum ActionField : byte
    {
        /// <summary>
        /// No extra fields included in the result set.
        /// </summary>
        None = 0,

        /// <summary>
        /// The project is included in the result set. 
        /// </summary>
        Project = 1 << 0,

        /// <summary>
        /// All of the fields are included in the result set.
        /// </summary>
        All = Project
    }
}
