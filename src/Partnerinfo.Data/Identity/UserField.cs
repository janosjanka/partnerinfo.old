// Copyright (c) János Janka. All rights reserved.

using System;

namespace Partnerinfo.Identity
{
    [Flags]
    public enum UserField : byte
    {
        /// <summary>
        /// No extra fields included in the result set.
        /// </summary>
        None = 0
    }
}
