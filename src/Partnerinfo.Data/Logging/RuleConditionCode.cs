// Copyright (c) János Janka. All rights reserved.

namespace Partnerinfo.Logging
{
    public enum RuleConditionCode : byte
    {
        Uknown = 0,

        // Event  ----------------------------------------------------------------

        /// <summary>
        /// Checks whether the system time is greater than or equal to the given value.
        /// </summary>
        StartDateGreaterThan = 1,

        /// <summary>
        /// Checks whether the system time is less than or equal to the given value.
        /// </summary>
        StartDateLessThan = 2,

        /// <summary>
        /// Checks whether the client id contains the specified value.
        /// </summary>
        ClientIdContains = 3,

        /// <summary>
        /// Checks whether the client id contains the specified value.
        /// </summary>
        CustomUriContains = 4,

        /// <summary>
        /// Checks whether the client id contains the specified value.
        /// </summary>
        ReferrerUrlContains = 5,

        // Project ---------------------------------------------------------------

        /// <summary>
        /// Checks whether the project id is equal to the specified value.
        /// </summary>
        ProjectIdEquals = 50,

        // Contact ---------------------------------------------------------------

        /// <summary>
        /// Checks whether the mail contains the specified value.
        /// </summary>
        ContactStateEquals = 100,


        /// <summary>
        /// Checks whether the mail contains the specified value.
        /// </summary>
        ContactMailContains = 101
    }
}
