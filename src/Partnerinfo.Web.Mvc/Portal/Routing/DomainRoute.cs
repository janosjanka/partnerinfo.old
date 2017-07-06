// Copyright (c) János Janka. All rights reserved.

using System;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using Partnerinfo.Properties;

namespace Partnerinfo.Portal.Routing
{
    public sealed class DomainRoute : Route
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DomainRoute"/> class.
        /// </summary>
        /// <param name="url">The URL pattern for the route.</param>
        /// <param name="routeHandler">The object that processes requests for the route.</param>
        public DomainRoute(string controller, string action)
            : base("{*pageUri}", new RouteValueDictionary { { "controller", controller }, { "action", action }, { "pageUri", UrlParameter.Optional } }, new MvcRouteHandler())
        {
        }

        /// <summary>
        /// Returns information about the requested route.
        /// </summary>
        /// <param name="httpContext">An object that encapsulates information about the HTTP request.</param>
        /// <returns>
        /// An object that contains the values from the route definition.
        /// </returns>
        public override RouteData GetRouteData(HttpContextBase httpContext)
        {
            RouteData route = null;
            string hostName = GetHostName(httpContext);

            if (!string.Equals(hostName, Settings.Default.AppHost, StringComparison.OrdinalIgnoreCase))
            {
                route = base.GetRouteData(httpContext);
                route.Values["domain"] = hostName;
            }

            return route;
        }

        /// <summary>
        /// Gets the name of the host.
        /// </summary>
        /// <param name="httpContext">The HTTP context.</param>
        /// <returns>
        /// A string that represents the HTTP host.
        /// </returns>
        private static string GetHostName(HttpContextBase httpContext)
        {
            string hostName = httpContext.Request.Headers["HOST"];

            if (hostName == null && httpContext.Request.Url != null)
            {
                hostName = httpContext.Request.Url.Host;
            }

            if (hostName != null && hostName.StartsWith("www.", StringComparison.OrdinalIgnoreCase))
            {
                return hostName.Substring(4);
            }

            return hostName;
        }
    }
}