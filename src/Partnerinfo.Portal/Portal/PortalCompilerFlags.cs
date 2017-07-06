// Copyright (c) János Janka. All rights reserved.

using System;

namespace Partnerinfo.Portal
{
    [Flags]
    public enum PortalCompilerFlags
    {
        /// <summary>
        /// The none
        /// </summary>
        None = 0,

        /// <summary>
        /// The merge master and content pages
        /// </summary>
        MergeMasterAndContent = 1 << 1,

        /// <summary>
        /// The interpolate HTML content
        /// </summary>
        InterpolateHtmlContent = 1 << 2
    }
}
