// Copyright (c) János Janka. All rights reserved.

namespace Partnerinfo.Security
{
    public enum AccessObjectType : byte
    {
        /// <summary>
        /// Indicates that the ACL source type is not being used.
        /// </summary>
        Unknown = 0,

        /// <summary>
        /// Indicates that the ACL source object is a project.
        /// </summary>
        Project = 10,

        /// <summary>
        /// Indicates that the ACL source object is a document.
        /// </summary>
        File = 20,

        /// <summary>
        /// Indicates that the ACL source object is a portal.
        /// </summary>
        Portal = 30,

        /// <summary>
        /// Indicates that the ACL source object is a page.
        /// </summary>
        Page = 40
    }
}
