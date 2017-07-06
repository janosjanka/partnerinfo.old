// Copyright (c) János Janka. All rights reserved.

namespace Partnerinfo
{
    /// <summary>
    /// Represents the gender of a person.
    /// </summary>
    public enum PersonGender : byte
    {
        /// <summary>
        /// Indicates no gender specification.
        /// </summary>
        Unknown = 0,

        /// <summary>
        /// Indicates a male.
        /// </summary>
        Male = 1,

        /// <summary>
        /// Indicates a female.
        /// </summary>
        Female = 2
    }
}
