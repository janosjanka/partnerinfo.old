// Copyright (c) János Janka. All rights reserved.

using System;

namespace Partnerinfo.Portal
{
    public static class MediaType
    {
        /// <summary>
        /// Represents a MIME type for JavaScript.
        /// </summary>
        public static readonly string Script = "text/javascript";

        /// <summary>
        /// Represents a MIME type for styles.
        /// </summary>
        public static readonly string Style = "text/css";

        /// <summary>
        /// Checks whether the specified MIME type represents a script.
        /// </summary>
        /// <param name="mimeType">The MIME type to check.</param>
        /// <returns>
        /// A value indicating whether the MIME type represents a script.
        /// </returns>
        public static bool IsScript(string mimeType)
        {
            return string.Equals(mimeType, Script, StringComparison.Ordinal);
        }

        /// <summary>
        /// Checks whether the specified MIME type represents a style.
        /// </summary>
        /// <param name="mimeType">The MIME type to check.</param>
        /// <returns>
        /// A value indicating whether the MIME type represents a style.
        /// </returns>
        public static bool IsStyle(string mimeType)
        {
            return string.Equals(mimeType, Style, StringComparison.Ordinal);
        }
    }
}
