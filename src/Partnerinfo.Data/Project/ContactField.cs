// Copyright (c) János Janka. All rights reserved.

using System;

namespace Partnerinfo.Project
{
    [Flags]
    public enum ContactField : byte
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
        /// The sponsor is included in the result set.
        /// </summary>
        Sponsor = 1 << 1,

        /// <summary>
        /// Business tags belong to the contact are included in the result set.
        /// </summary>
        BusinessTags = 1 << 2,

        /// <summary>
        /// All of the fields are included in the result set.
        /// </summary>
        All = Project | Sponsor | BusinessTags
    }
}
