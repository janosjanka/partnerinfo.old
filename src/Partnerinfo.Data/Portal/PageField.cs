// Copyright (c) János Janka. All rights reserved.

using System;

namespace Partnerinfo.Portal
{
    [Flags]
    public enum PageField
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
        /// The master is included in the result set. 
        /// </summary>
        Master = 1 << 1,

        /// <summary>
        /// The content is included in the result set.
        /// </summary>
        Content = 1 << 2,

        /// <summary>
        /// All of the fields are included in the result set.
        /// </summary>
        All = Portal | Master | Content
    }
}
