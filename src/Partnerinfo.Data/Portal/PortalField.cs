// Copyright (c) János Janka. All rights reserved.

using System;

namespace Partnerinfo.Portal
{
    [Flags]
    public enum PortalField
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
        /// The project is included in the result set. 
        /// </summary>
        Project = 1 << 1,

        /// <summary>
        /// The home page is included in the result set.
        /// </summary>
        HomePage = 1 << 2,

        /// <summary>
        /// The master page is included in the result set.
        /// </summary>
        MasterPage = 1 << 3,

        /// <summary>
        /// All of the fields are included in the result set.
        /// </summary>
        All = Owners | Project | HomePage | MasterPage
    }
}
