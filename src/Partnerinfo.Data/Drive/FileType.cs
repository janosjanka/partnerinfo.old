// Copyright (c) János Janka. All rights reserved.

using System.Runtime.Serialization;

namespace Partnerinfo.Drive
{
    /// <summary>
    /// Used to set the type of the document.
    /// </summary>
    public enum FileType : byte
    {
        /// <summary>
        /// Indicates that the document type is not being used.
        /// </summary>
        [EnumMember(Value = "unknown")]
        Unknown = 0,

        /// <summary>
        /// Indicates that the document is a folder.
        /// </summary>
        [EnumMember(Value = "folder")]
        Folder = 1,

        /// <summary>
        /// Indicates that the document is a file.
        /// </summary>
        [EnumMember(Value = "file")]
        File = 2
    }
}
