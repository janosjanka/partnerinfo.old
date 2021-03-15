// Copyright (c) János Janka. All rights reserved.

using System;

namespace Partnerinfo.Logging
{
    public class UrlProvider : IUrlProvider
    {
        private const int MaxLen = 256;
        public static readonly IUrlProvider Default = new UrlProvider();

        /// <summary>
        /// Generates a valid URL from the given URL.
        /// </summary>
        /// <param name="string">The URL.</param>
        /// <returns>
        /// The generated URL.
        /// </returns>
        public string Generate(string url)
        {
            if (url == null)
            {
                return null;
            }

            Uri uri;
            if (Uri.TryCreate(url, UriKind.RelativeOrAbsolute, out uri))
            {
                url = uri.ToString();
                if (url.Length > MaxLen)
                {
                    url = uri.GetLeftPart(UriPartial.Path);
                }
                if (url.Length > MaxLen)
                {
                    url = uri.GetLeftPart(UriPartial.Authority);
                }
                if (url.Length <= MaxLen)
                {
                    return url;
                }
            }
            return null;
        }
    }
}
