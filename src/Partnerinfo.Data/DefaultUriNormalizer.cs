// Copyright (c) János Janka. All rights reserved.

using System.Web;

namespace Partnerinfo
{
    public sealed class DefaultUriNormalizer : IUriNormalizer
    {
        public static readonly DefaultUriNormalizer Default = new DefaultUriNormalizer();

        /// <summary>
        /// Returns a normalized representation of the specified <paramref name="uri" />.
        /// </summary>
        /// <param name="uri">The uri to normalize.</param>
        /// <returns>
        /// A normalized representation of the specified <paramref name="uri" />.
        /// </returns>
        /// <exception cref="System.ArgumentNullException"></exception>
        public string Normalize(string uri) => UriUtility.Normalize(uri);
    }
}
