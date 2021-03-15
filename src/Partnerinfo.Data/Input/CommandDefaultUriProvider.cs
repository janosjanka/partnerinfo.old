// Copyright (c) János Janka. All rights reserved.

using System.Web;

namespace Partnerinfo.Input
{
    public class CommandDefaultUriProvider : ICommandUriProvider
    {
        public static readonly CommandDefaultUriProvider Default = new CommandDefaultUriProvider();

        /// <summary>
        /// Generates a valid URL from the given URL.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns>
        /// The generated URI.
        /// </returns>
        public virtual string Generate(string value)
        {
            return UriUtility.EncodeRandomUriToken(32);
        }
    }
}
