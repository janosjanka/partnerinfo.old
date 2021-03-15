// Copyright (c) János Janka. All rights reserved.

namespace Partnerinfo.Logging
{
    public interface IUrlProvider
    {
        /// <summary>
        /// Generates a valid URL from the given URL.
        /// </summary>
        /// <param name="string">The URL.</param>
        /// <returns>
        /// The generated URL.
        /// </returns>
        string Generate(string url);
    }
}
