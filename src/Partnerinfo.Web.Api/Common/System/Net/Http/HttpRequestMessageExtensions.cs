// Copyright (c) János Janka. All rights reserved.

using Partnerinfo;
using System.Collections.Generic;
using System.Net.Http.Headers;

namespace System.Net.Http
{
    internal static class HttpRequestMessageExtensions
    {
        /// <summary>
        /// Creates an <see cref="System.Net.Http.HttpResponseMessage" /> wired up to the associated <see cref="System.Net.Http.HttpRequestMessage" />.
        /// </summary>
        /// <typeparam name="T">The type of the HTTP response message.</typeparam>
        /// <param name="request">The HTTP request message which led to this response message.</param>
        /// <param name="statusCode">The HTTP response status code.</param>
        /// <param name="value">The content of the HTTP response message.</param>
        /// <returns>
        /// An initialized <see cref="System.Net.Http.HttpResponseMessage" /> wired up to the associated <see cref="System.Net.Http.HttpRequestMessage" />.
        /// </returns>
        /// <exception cref="System.ArgumentNullException">request</exception>
        public static HttpResponseMessage CreateResponseWithNoCache<T>(this HttpRequestMessage request, HttpStatusCode statusCode, T value)
        {
            if (request == null)
            {
                throw new ArgumentNullException("request");
            }

            HttpResponseMessage response = request.CreateResponse(statusCode, value);
            response.Headers.Vary.Add("Accept-Encoding");
            response.Headers.CacheControl = new CacheControlHeaderValue
            {
                NoCache = true,
                MaxAge = TimeSpan.Zero,
                NoStore = true,
                MustRevalidate = true
            };

            return response;
        }

        /// <summary>
        /// Checks whether the current resource has changed since we got it.
        /// </summary>
        /// <param name="request">The HTTP request message which led to this response message.</param>
        /// <param name="lastModified">The last update time.</param>
        /// <returns>
        ///   <c>true</c> if [is modified since] [the specified request]; otherwise, <c>false</c>.
        /// </returns>
        public static bool IsModifiedSince(this HttpRequestMessage request, DateTime lastModified)
        {
            if (request == null)
            {
                throw new ArgumentNullException("request");
            }

            // Recode the time value without millisecs (JavaScript workaround).
            lastModified = new DateTime(lastModified.Year, lastModified.Month, lastModified.Day, lastModified.Hour, lastModified.Minute, lastModified.Second, 0).ToUniversalTime();
            return !(request.Headers.IfModifiedSince != null && DateTimeOffset.Equals(request.Headers.IfModifiedSince.Value, lastModified));
        }

        /// <summary>
        /// Creates an <see cref="System.Net.Http.HttpResponseMessage" /> wired up to the associated <see cref="System.Net.Http.HttpRequestMessage" />.
        /// </summary>
        /// <typeparam name="T">The type of the HTTP response message.</typeparam>
        /// <param name="request">The HTTP request message which led to this response message.</param>
        /// <param name="statusCode">The HTTP response status code.</param>
        /// <param name="value">The content of the HTTP response message.</param>
        /// <param name="timeout">The value of the Expires content header on an HTTP response.</param>
        /// <param name="cacheHeader">The value of the Cache-Control header for an HTTP response. The default cache is private.</param>
        /// <returns>
        /// An initialized <see cref="System.Net.Http.HttpResponseMessage" /> wired up to the associated <see cref="System.Net.Http.HttpRequestMessage" />.
        /// </returns>
        /// <exception cref="System.ArgumentNullException">request</exception>
        public static HttpResponseMessage CreateResponseWithCache<T>(this HttpRequestMessage request, HttpStatusCode statusCode, T value, TimeSpan timeout, CacheControlHeaderValue cacheHeader = null)
        {
            if (request == null)
            {
                throw new ArgumentNullException("request");
            }

            // Instruct proxy servers to cache two versions of the resource: one compressed, and one uncompressed.            
            HttpResponseMessage response = request.CreateResponse(statusCode, value);
            response.Headers.CacheControl = cacheHeader ?? new CacheControlHeaderValue { Private = true };
            response.Headers.Vary.Add("Accept-Encoding");
            response.Content.Headers.Expires = DateTime.UtcNow.Add(timeout);
            return response;
        }

        /// <summary>
        /// Creates an <see cref="System.Net.Http.HttpResponseMessage" /> wired up to the associated <see cref="System.Net.Http.HttpRequestMessage" />.
        /// </summary>
        /// <typeparam name="T">The type of the HTTP response message.</typeparam>
        /// <param name="request">The HTTP request message which led to this response message.</param>
        /// <param name="statusCode">The HTTP response status code.</param>
        /// <param name="lastModified">The last update time.</param>
        /// <param name="value">The content of the HTTP response message.</param>
        /// <param name="timeout">The value of the expires content header on an HTTP response.</param>
        /// <returns>
        /// An initialized <see cref="System.Net.Http.HttpResponseMessage" /> wired up to the associated <see cref="System.Net.Http.HttpRequestMessage" />.
        /// </returns>
        /// <exception cref="System.ArgumentNullException">request</exception>
        public static HttpResponseMessage CreateResponseIfModified<T>(this HttpRequestMessage request, HttpStatusCode statusCode, DateTime lastModified, T value, TimeSpan? timeout = null)
        {
            if (request == null)
            {
                throw new ArgumentNullException("request");
            }

            if (request.IsModifiedSince(lastModified))
            {
                HttpResponseMessage response = request.CreateResponse(statusCode, value);
                response.Headers.Vary.Add("Accept-Encoding");
                response.Content.Headers.LastModified = lastModified;

                if (timeout != null)
                {
                    // Set the value of the expires content header on the HTTP response.
                    response.Content.Headers.Expires = DateTime.UtcNow.Add(timeout.Value);
                }

                return response;
            }
            else
            {
                // Equivalent to HTTP status 304. System.Net.HttpStatusCode.NotModified indicates
                // that the client's cached copy is up to date. The contents of the resource
                // are not transferred.
                return request.CreateResponse(HttpStatusCode.NotModified);
            }
        }

        /// <summary>
        /// Creates an <see cref="System.Net.Http.HttpResponseMessage" /> wired up to the associated <see cref="System.Net.Http.HttpRequestMessage" />.
        /// </summary>
        /// <typeparam name="T">The type of the HTTP response message.</typeparam>
        /// <param name="request">The HTTP request message which led to this response message.</param>
        /// <param name="statusCode">The HTTP response status code.</param>
        /// <param name="lastModified">The last update time.</param>
        /// <param name="valueAccessor">A callback function that can be called when the content of the HTTP response changes.</param>
        /// <param name="timeout">The value of the expires content header on an HTTP response.</param>
        /// <returns>
        /// An initialized <see cref="System.Net.Http.HttpResponseMessage" /> wired up to the associated <see cref="System.Net.Http.HttpRequestMessage" />.
        /// </returns>
        /// <exception cref="System.ArgumentNullException">request</exception>
        public static HttpResponseMessage CreateResponseIfModified<T>(this HttpRequestMessage request, HttpStatusCode statusCode, DateTime lastModified, Func<T> valueAccessor, TimeSpan? timeout = null)
        {
            if (request == null)
            {
                throw new ArgumentNullException("request");
            }

            if (request.IsModifiedSince(lastModified))
            {
                HttpResponseMessage response = request.CreateResponse(statusCode, valueAccessor());
                response.Headers.Vary.Add("Accept-Encoding");
                response.Content.Headers.LastModified = lastModified;

                if (timeout != null)
                {
                    // Set the value of the expires content header on the HTTP response.
                    response.Content.Headers.Expires = DateTime.UtcNow.Add(timeout.Value);
                }

                return response;
            }
            else
            {
                // Equivalent to HTTP status 304. System.Net.HttpStatusCode.NotModified indicates
                // that the client's cached copy is up to date. The contents of the resource
                // are not transferred.
                return request.CreateResponse(HttpStatusCode.NotModified);
            }
        }

        /// <summary>
        /// Creates an <see cref="System.Net.Http.HttpResponseMessage" /> that represents an exception.
        /// </summary>
        /// <param name="request">The HTTP request.</param>
        /// <param name="statusCode">The status code of the response.</param>
        /// <param name="errorMessage">The error message.</param>
        /// <returns>
        /// The request must be associated with an <see cref="System.Net.Http.HttpConfiguration" /> instance.
        /// An <see cref="System.Net.Http.HttpResponseMessage" /> whose content is a serialized representation of an <see cref="PartnerinfoFaultMessage" /> instance.
        /// </returns>
        public static HttpResponseMessage CreateFaultResponse(this HttpRequestMessage request, HttpStatusCode statusCode, string errorMessage)
        {
            return request.CreateResponse(statusCode, new FaultMessage(errorMessage));
        }

        /// <summary>
        /// Creates an <see cref="System.Net.Http.HttpResponseMessage" /> that represents an exception.
        /// </summary>
        /// <param name="request">The HTTP request.</param>
        /// <param name="statusCode">The status code of the response.</param>
        /// <param name="details">The details.</param>
        /// <returns>
        /// The request must be associated with an <see cref="System.Net.Http.HttpConfiguration" /> instance.
        /// An <see cref="System.Net.Http.HttpResponseMessage" /> whose content is a serialized representation of an <see cref="PartnerinfoFaultMessage" /> instance.
        /// </returns>
        public static HttpResponseMessage CreateFaultResponse(this HttpRequestMessage request, HttpStatusCode statusCode, IEnumerable<FaultMember> details)
        {
            return request.CreateResponse(statusCode, new FaultMessage(details));
        }

        /// <summary>
        /// Creates an <see cref="System.Net.Http.HttpResponseMessage" /> that represents an exception.
        /// </summary>
        /// <param name="request">The HTTP request.</param>
        /// <param name="statusCode">The status code of the response.</param>
        /// <param name="errorMessage">The error message.</param>
        /// <param name="details">The details.</param>
        /// <returns>
        /// The request must be associated with an <see cref="System.Net.Http.HttpConfiguration" /> instance.
        /// An <see cref="System.Net.Http.HttpResponseMessage" /> whose content is a serialized representation of an <see cref="PartnerinfoFaultMessage" /> instance.
        /// </returns>
        public static HttpResponseMessage CreateFaultResponse(this HttpRequestMessage request, HttpStatusCode statusCode, string errorMessage, IEnumerable<FaultMember> details)
        {
            return request.CreateResponse(statusCode, new FaultMessage(errorMessage, details));
        }
    }
}