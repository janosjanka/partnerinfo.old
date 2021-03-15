// Copyright (c) János Janka. All rights reserved.

namespace Partnerinfo.Security
{
    /// <summary>
    /// Represents a visibility for an ACE (Access Control Entry).
    /// </summary>
    public enum AccessVisibility : byte
    {
        /// <summary>
        /// Indicates that the visibility is not being used.
        /// </summary>
        Unknown = 0,

        /// <summary>
        /// Indicates a public resource on the Internet. 
        /// </summary>
        Public = 10,

        /// <summary>
        /// Indicates a public resource which is only readable by a link and not indexed by search engines.
        /// </summary>
        AnyoneWithLink = 20,

        /// <summary>
        /// Indicates a protected resource which is only readable by project users (contacts).
        /// </summary>
        Impersonated = 30,

        /// <summary>
        /// Indicates a private resource which is only readable by system users.
        /// </summary>
        Private = 40
    }
}
