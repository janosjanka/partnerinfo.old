// Copyright (c) János Janka. All rights reserved.

using System.Runtime.Serialization;

namespace Partnerinfo.Logging
{
    public enum BrowserBrand : byte
    {
        /// <summary>
        /// Unknown browser
        /// </summary>
        [EnumMember(Value = "unknown")]
        Unknown = 0,

        /// <summary>
        /// Microsoft Internet Explorer (MSIE)
        /// </summary>
        [EnumMember(Value = "iexplorer")]
        IExplorer = 1,

        /// <summary>
        /// Mozilla FireFox
        /// </summary>
        [EnumMember(Value = "firefox")]
        FireFox = 2,

        /// <summary>
        /// Google Chrome
        /// </summary>
        [EnumMember(Value = "chrome")]
        Chrome = 3,

        /// <summary>
        /// Opera Software
        /// </summary>
        [EnumMember(Value = "opera")]
        Opera = 4,

        /// <summary>
        /// Apple Safari
        /// </summary>
        [EnumMember(Value = "safari")]
        Safari = 5
    }
}
