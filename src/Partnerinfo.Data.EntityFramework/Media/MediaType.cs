// Copyright (c) János Janka. All rights reserved.

using System.Runtime.Serialization;

namespace Partnerinfo.Media.EntityFramework
{
    /// <summary>
    /// Used to set the media type.
    /// </summary>
    public enum MediaType : byte
    {
        /// <summary>
        /// Indicates that the media type is not being used.
        /// </summary>
        [EnumMember(Value = "unknown")]
        Unknown = 0,

        /// <summary>
        /// Indicates that the media type is a YouTube video.
        /// </summary>
        [EnumMember(Value = "youtube")]
        YouTube = 10,

        /// <summary>
        /// Indicates that the media type is a Dailymotion video.
        /// </summary>
        [EnumMember]
        Dailymotion = 20
    }
}
