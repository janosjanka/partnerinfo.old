// Copyright (c) János Janka. All rights reserved.

using System.Runtime.Serialization;

namespace Partnerinfo.Logging
{
    public enum MobileDevice : byte
    {
        /// <summary>
        /// Indicates that the mobile device type is not being used.
        /// </summary>
        [EnumMember(Value = "unknown")]
        Unknown = 0,

        /// <summary>
        /// Indicates that the mobile device type is IPhone.
        /// </summary>
        [EnumMember(Value = "iphone")]
        IPhone = 1,

        /// <summary>
        /// Indicates that the mobile device type is IPod.
        /// </summary>
        [EnumMember(Value = "ipod")]
        IPod = 2,

        /// <summary>
        /// Indicates that the mobile device type is IPad.
        /// </summary>
        [EnumMember(Value = "ipad")]
        IPad = 3,

        /// <summary>
        /// Indicates that the mobile device type is IPad.
        /// </summary>
        [EnumMember(Value = "android")]
        Android = 4,

        /// <summary>
        /// Indicates that the mobile device type is IPad.
        /// </summary>
        [EnumMember(Value = "windowsphone")]
        WindowsPhone = 5,

        /// <summary>
        /// Other phone device
        /// </summary>
        [EnumMember(Value = "other")]
        Other = 255
    }
}
