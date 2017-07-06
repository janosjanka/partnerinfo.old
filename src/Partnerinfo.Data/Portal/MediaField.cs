// Copyright (c) János Janka. All rights reserved.

using System;

namespace Partnerinfo.Portal
{
    [Flags]
    public enum MediaField
    {
        /// <summary>
        /// No extra fields included in the result set.
        /// </summary>
        None = 0,

        /// <summary>
        /// The portal is included in the result set. 
        /// </summary>
        Portal = 1 << 0,

        /// <summary>
        /// All of the fields are included in the result set.
        /// </summary>
        All = Portal
    }
}
