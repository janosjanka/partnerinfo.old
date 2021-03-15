// Copyright (c) János Janka. All rights reserved.

namespace Partnerinfo.Logging
{
    public enum RuleActionCode : byte
    {
        /// <summary>
        /// 
        /// </summary>
        Uknown = 0,

        /// <summary>
        /// Indicates a delete operation
        /// </summary>
        Remove = 1,

        /// <summary>
        /// Indicates a categorization operation
        /// </summary>
        Categorize = 2
    }
}
