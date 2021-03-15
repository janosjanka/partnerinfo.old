// Copyright (c) János Janka. All rights reserved.

using System;
using System.Net.Http.Headers;

namespace System.Net.Http
{
    public static class HttpResponseMessageExtensions
    {
        /// <summary>
        /// Sets whether an HTTP client is willing to accept a cached response.
        /// </summary>
        /// <param name="response">The response message.</param>
        /// <returns>A <see cref="HttpResponseMessage"/> instance that represents a response message.</returns>
        public static HttpResponseMessage SetNoCache(this HttpResponseMessage response)
        {
            if (response == null)
            {
                throw new ArgumentNullException("response");
            }

            CacheControlHeaderValue cc = response.Headers.CacheControl;
            if (cc == null)
            {
                cc = new CacheControlHeaderValue();
                response.Headers.CacheControl = cc;
            }

            // Google sample.
            cc.NoCache = true;
            cc.MaxAge = TimeSpan.Zero;
            cc.NoStore = true;
            cc.MustRevalidate = true;

            return response;
        }
    }
}
