﻿// Copyright (c) János Janka. All rights reserved.

namespace Partnerinfo.Identity
{
    public enum UserSortOrder
    {
        /// <summary>
        /// Items are returned in chronological order.
        /// </summary>
        None = 0,

        /// <summary>
        /// Items are returned in reverse chronological order.
        /// </summary>
        Recent = 1,

        /// <summary>
        /// Items are ordered alphabetically by name.
        /// </summary>
        Name = 2
    }
}