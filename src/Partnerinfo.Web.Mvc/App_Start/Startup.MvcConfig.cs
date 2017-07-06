// Copyright (c) János Janka. All rights reserved.

using System.Web.Mvc;
using System.Web.Routing;
using Owin;
using Partnerinfo.Filters;
using Partnerinfo.Portal.Routing;

namespace Partnerinfo
{
    public partial class Startup
    {
        public static void ConfigureMvc(IAppBuilder app)
        {
            // MS likes inserting silly things to the response header.
            MvcHandler.DisableMvcResponseHeader = true;

            RegisterRoutes(RouteTable.Routes);
            RegisterFilters(GlobalFilters.Filters);

            var locations = new[]
            {
                "~/Analytics/Views/{1}/{0}.cshtml",
                "~/Analytics/Views/Shared/{0}.cshtml",
                "~/Drive/Views/{1}/{0}.cshtml",
                "~/Drive/Views/Shared/{0}.cshtml",
                "~/Identity/Views/{1}/{0}.cshtml",
                "~/Identity/Views/Shared/{0}.cshtml",
                "~/Project/Views/{1}/{0}.cshtml",
                "~/Project/Views/Shared/{0}.cshtml",
                "~/Portal/Views/{1}/{0}.cshtml",
                "~/Portal/Views/Shared/{0}.cshtml",
                "~/Views/{1}/{0}.cshtml",
                "~/Views/Shared/{0}.cshtml"
            };

            ViewEngines.Engines.Clear();
            ViewEngines.Engines.Add(
                new RazorViewEngine
                {
                    FileExtensions = new[] { "cshtml" },
                    MasterLocationFormats = locations,
                    PartialViewLocationFormats = locations,
                    ViewLocationFormats = locations
                });
        }

        /// <summary>
        /// Registers all routes in an ASP.NET MVC application.
        /// </summary>
        private static void RegisterRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute(
                url: "{resource}/{*pathInfo}",
                constraints: new { resource = "^(a|api|filestore|hangfire)$" });

            routes.Add(
                name: "Domain",
                item: new DomainRoute("Engine", "Domain"));

            routes.MapRoute(
                name: "Account",
                url: "account/{action}",
                defaults: new { controller = "Account", action = "Index" });

            routes.MapRoute(
                name: "Admin",
                url: "admin/{controller}/{id}/{action}",
                defaults: new { action = "Index", id = UrlParameter.Optional });

            routes.MapRoute(
                name: "Portal",
                url: "{portalUri}/{*pageUri}",
                defaults: new { controller = "Engine", action = "Portal", pageUri = UrlParameter.Optional });

            routes.MapRoute(
               name: "Default",
               url: "",
               defaults: new { controller = "Home", action = "Index" });
        }

        /// <summary>
        /// Registers all global filters in an ASP.NET MVC application.
        /// </summary>
        private static void RegisterFilters(GlobalFilterCollection filters)
        {
            filters.Add(new LocalizedAttribute());
            filters.Add(new GlobalDataAttribute());
        }
    }
}