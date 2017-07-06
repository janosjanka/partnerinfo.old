// Copyright (c) János Janka. All rights reserved.

using System;

namespace Partnerinfo.Portal.Models
{
    [Flags]
    public enum PortalQueryField
    {
        /// <summary>
        /// No extra fields included in the result set.
        /// </summary>
        None = PortalField.None,

        /// <summary>
        /// Owners are included in the result set. 
        /// </summary>
        Owners = PortalField.Owners,

        /// <summary>
        /// The project is included in the result set. 
        /// </summary>
        Project = PortalField.Project,

        /// <summary>
        /// The home page is included in the result set.
        /// </summary>
        HomePage = PortalField.HomePage,

        /// <summary>
        /// The master page is included in the result set.
        /// </summary>
        MasterPage = PortalField.MasterPage,

        /// <summary>
        /// The master page is included in the result set.
        /// </summary>
        Pages = 1 << 4
    }
}
