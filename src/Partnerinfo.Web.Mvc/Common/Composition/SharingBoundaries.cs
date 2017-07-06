// Copyright (c) János Janka. All rights reserved.

namespace Partnerinfo.Composition
{
    /// <summary>
    /// Well-known sharing boundary names. The composition provider uses
    /// all of these when handling a web request.
    /// </summary>
    internal static class SharingBoundaries
    {
        /// <summary>
        /// The boundary within which a current HTTP request is accessible.
        /// </summary>
        public const string HttpRequest = "HttpRequest";
    }
}
