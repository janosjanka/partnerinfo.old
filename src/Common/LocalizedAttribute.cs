// Copyright (c) János Janka. All rights reserved.

using System.Diagnostics;
using System.Globalization;
using System.Threading;
using System.Web.Routing;

namespace System.Web.Mvc
{
    internal class LocalizedAttribute : FilterAttribute, IActionFilter
    {
        private static readonly Configuration.LocalizationSettings s_settings = Configuration.LocalizationSettings.Instance;

        public void OnActionExecuting(ActionExecutingContext filterContext)
        {
            CultureInfo cultureInfo = null;

            string culture = (string)filterContext.RouteData.Values[s_settings.Name];
            if (culture != null)
            {
                if (!TrySetThreadCulture(culture, out cultureInfo))
                {
                    throw new HttpException(404, string.Format("Invalid culture: {0}", culture));
                }
            }
            else
            {
                culture = GetClientCulture(filterContext.HttpContext.Request);
                if (culture != null)
                {
                    TrySetThreadCulture(culture, out cultureInfo);
                }

                // The culture sent by the browser is less restrictive than ASP.NET MVC URL route data.
                // It means that we can replace the user-defined culture with the culture of the current thread.
                if (cultureInfo == null)
                {
                    cultureInfo = CultureInfo.DefaultThreadCurrentUICulture;
                }
            }

            SetClientCulture(filterContext.HttpContext.Response, cultureInfo.Name);
            SetCultureRoute(filterContext.RouteData, cultureInfo);
        }

        public void OnActionExecuted(ActionExecutedContext filterContext)
        {
        }

        /// <summary>
        /// Gets the two letter ISO language name from one of the HTTP collections.
        /// </summary>
        /// <param name="request">The request.</param>
        /// <returns>
        /// The two letter ISO language name or null.
        /// </returns>
        private static string GetClientCulture(HttpRequestBase request)
        {
            var culture = default(string);
            var cookie = request.Cookies[s_settings.Name];
            if (cookie != null)
            {
                culture = cookie.Value;
            }
            if (string.IsNullOrEmpty(culture) && request.UserLanguages?.Length > 0)
            {
                culture = request.UserLanguages[0];
            }
            return culture;
        }

        /// <summary>
        /// Sets the culture for the HTTP response.
        /// </summary>
        /// <param name="response">The HTTP response.</param>
        /// <param name="culture">The culture to set.</param>
        private static void SetClientCulture(HttpResponseBase response, string culture)
        {
            var cookie = new HttpCookie(s_settings.Name, culture);
            cookie.Domain = s_settings.Domain;
            cookie.Path = s_settings.Path;
            cookie.Expires = DateTime.UtcNow.Add(s_settings.Expiration);
            response.SetCookie(cookie);
        }

        /// <summary>
        /// Sets the specified culture route for the MVC controller.
        /// </summary>
        /// <param name="routeData">The route data.</param>
        /// <param name="cultureInfo">The culture to set.</param>
        private static void SetCultureRoute(RouteData routeData, CultureInfo cultureInfo)
        {
            if (s_settings.UseSpecificCulture)
            {
                routeData.Values[s_settings.Name] = cultureInfo.Name.ToLower();
            }
            else
            {
                routeData.Values[s_settings.Name] = cultureInfo.TwoLetterISOLanguageName.ToLower();
            }
        }

        /// <summary>
        /// Sets a specific culture for threads in the current application domain.
        /// </summary>
        /// <param name="culture">The two letter ISO language name.</param>
        /// <returns>
        /// The specific culture info object.
        /// </returns>
        private static bool TrySetThreadCulture(string culture, out CultureInfo cultureInfo)
        {
            Debug.Assert(culture != null);

            try
            {
                cultureInfo = CultureInfo.CreateSpecificCulture(culture);
            }
            catch (CultureNotFoundException)
            {
                cultureInfo = null;
                return false;
            }

            Thread.CurrentThread.CurrentCulture = cultureInfo;
            Thread.CurrentThread.CurrentUICulture = cultureInfo;

            CultureInfo.DefaultThreadCurrentCulture = cultureInfo;
            CultureInfo.DefaultThreadCurrentUICulture = cultureInfo;

            return true;
        }
    }
}