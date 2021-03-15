// Copyright (c) János Janka. All rights reserved.

using System;

namespace Partnerinfo.Logging
{
    [Flags]
    public enum RuleField : byte
    {
        /// <summary>
        /// No extra fields included in the result set.
        /// </summary>
        None = 0
    }
}
