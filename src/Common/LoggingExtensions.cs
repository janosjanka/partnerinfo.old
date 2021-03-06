// Copyright (c) János Janka. All rights reserved.

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Web;
using Partnerinfo.Logging;

namespace Partnerinfo
{
    internal static class LoggingExtensions
    {
        private static readonly IDictionary<string, BrowserBrand> s_browsers = new ReadOnlyDictionary<string, BrowserBrand>(
            new Dictionary<string, BrowserBrand>(StringComparer.OrdinalIgnoreCase)
            {
                { "Chrome", BrowserBrand.Chrome },
                { "Firefox", BrowserBrand.FireFox },
                { "Opera", BrowserBrand.Opera },
                { "Safari", BrowserBrand.Safari },
                { "IEMobile", BrowserBrand.IExplorer },
                { "IE", BrowserBrand.IExplorer }
            });

        private static readonly IDictionary<string, MobileDevice> s_devices = new ReadOnlyDictionary<string, MobileDevice>(
            new Dictionary<string, MobileDevice>(StringComparer.OrdinalIgnoreCase)
            {
                { "IPhone", MobileDevice.IPhone },
                { "IPod", MobileDevice.IPod },
                { "IPad", MobileDevice.IPad },
                { "Android", MobileDevice.Android }
            });

        /// <summary>
        /// Creates an identity event from the request object.
        /// </summary>
        /// <param name="request">The <see cref="HttpRequestBase" /> object.</param>
        /// <param name="requestId">The request ID.</param>
        public static EventItem CreateEvent(this HttpRequestBase request)
        {
            if (request == null)
            {
                throw new ArgumentNullException(nameof(request));
            }

            BrowserBrand browserBrand = BrowserBrand.Unknown;
            short browserVersion = 0;
            MobileDevice mobileDevice = MobileDevice.Unknown;

            if (request.Browser != null)
            {
                s_browsers.TryGetValue(request.Browser.Browser, out browserBrand);
                browserVersion = (short)request.Browser.MajorVersion;
                s_devices.TryGetValue(request.Browser.MobileDeviceModel, out mobileDevice);
            }

            return new EventItem
            {
                AnonymId = GetAnonymId(request),
                BrowserBrand = browserBrand,
                BrowserVersion = browserVersion,
                MobileDevice = mobileDevice,
                ClientId = request.UserHostAddress,
                ReferrerUrl = request.UrlReferrer != null ? request.UrlReferrer.AbsoluteUri : null
            };
        }

        /// <summary>
        /// Gets the Anonym User ID from the request.
        /// </summary>
        /// <param name="request">The request object.</param>
        /// <returns></returns>
        public static Guid? GetAnonymId(this HttpRequestBase request)
        {
            if (request == null)
            {
                throw new ArgumentNullException(nameof(request));
            }

            Guid anonymId;

            // 1. A Session ID can be passed by a custom HTTP header
            string value = request.Headers.Get(ResourceKeys.AnonymIdCookieName);
            if (value != null && Guid.TryParse(value, out anonymId))
            {
                return anonymId;
            }

            // 2. We can also use a session cookie if a custom HTTP header does not exist
            var cookie = request.Cookies.Get(ResourceKeys.AnonymIdCookieName);
            if (cookie != null && Guid.TryParse(cookie.Value, out anonymId))
            {
                return anonymId;
            }

            return null;
        }

        /// <summary>
        /// Sets the given Session ID for the response.
        /// </summary>
        /// <param name="response">The response object.</param>
        /// <param name="anonymId">The Anonym User ID.</param>
        /// <param name="path">The virtual path to transmit with the cookie.</param>
        public static void SetAnonymId(this HttpResponseBase response, Guid anonymId, string path = "/")
        {
            if (response == null)
            {
                throw new ArgumentNullException(nameof(response));
            }

            string anonym = anonymId.ToString("N");

            // 1. Add a HTTP session header to the response
            response.Headers.Add(ResourceKeys.AnonymIdCookieName, anonym);

            // 2. Add a HTTP-only session cookie to the response
            response.Cookies.Set(new HttpCookie(ResourceKeys.AnonymIdCookieName)
            {
                Path = path,
                Value = anonym,
                HttpOnly = true
            });
        }
    }
}
