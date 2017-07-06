// Copyright (c) János Janka. All rights reserved.

namespace Partnerinfo.Security
{
    /// <summary>
    /// Used to set ACL role. The order of the roles is important.
    /// </summary>
    public enum AccessPermission : byte
    {
        /// <summary>
        /// Indicates that the role is not being used.
        /// </summary>
        Unknown = 0,

        /// <summary>
        /// Grants permissions to view a business object.
        /// </summary>
        CanView = 10,

        /// <summary>
        /// Grants permissions to view and edit a business object.
        /// </summary>
        CanEdit = 20,

        /// <summary>
        /// Grants permissions to view, edit, and manage a business object.
        /// </summary>
        CanManage = 25,

        /// <summary>
        /// Grants permissions to view, edit, manage, and delete a business object.
        /// </summary>
        IsOwner = 30
    }
}
