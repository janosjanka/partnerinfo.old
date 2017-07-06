// Copyright (c) János Janka. All rights reserved.

using System.Web.Http;
using Owin;

namespace Partnerinfo
{
    public static class ApiConfig
    {
        public static HttpConfiguration HttpConfiguration;

        public static void Configure(IAppBuilder app, HttpConfiguration httpConfig)
        {
            HttpConfiguration = httpConfig;

#if DEBUG
            httpConfig.EnableSystemDiagnosticsTracing();
#endif

            RegisterFilters(httpConfig);
            RegisterFormatters(httpConfig);
            // RegisterMappers(httpConfig);

            // This style of routing is similar to ASP.NET MVC, and may be appropriate for an RPC-style API.
            // For a RESTful API, you should avoid using verbs in the URIs, because a URI should identify a resource, not an action.

            httpConfig.MapHttpAttributeRoutes();
            //httpConfig.Routes.MapHttpRoute("ApiDefault", "api/{controller}/{id}", new { controller = "Users", id = RouteParameter.Optional });
            httpConfig.EnsureInitialized();
        }

        /// <summary>
        /// Registers API filters.
        /// </summary>
        /// <param name="config">Configuration of System.Web.Http.HttpServer instances.</param>
        private static void RegisterFilters(HttpConfiguration config)
        {
            config.Filters.Add(new Filters.ExceptionAttribute());
            config.Filters.Add(new Filters.ValidationAttribute());
            // config.Filters.Add(new UserAttribute());
        }

        /// <summary>
        /// Registers API message formatters.
        /// </summary>
        /// <param name="config">Configuration of System.Web.Http.HttpServer instances.</param>
        private static void RegisterFormatters(HttpConfiguration config)
        {
            config.Formatters.Remove(config.Formatters.XmlFormatter);
            config.Formatters.JsonFormatter.SerializerSettings = JsonNetUtility.Settings;
        }

        //        /// <summary>
        //        /// Registers API filters.
        //        /// </summary>
        //        /// <param name="config">Configuration of System.Web.Http.HttpServer instances.</param>
        //        private static void RegisterMappers(HttpConfiguration config)
        //        {
        //            Mapper.Initialize(c =>
        //            {
        //                c.AddProfile<Portal.ViewModels.AutoMapperProfile>();
        //            });

        //#if DEBUG
        //            Mapper.AssertConfigurationIsValid();
        //#endif
        //        }
    }
}