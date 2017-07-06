// Copyright (c) János Janka. All rights reserved.

using System.Collections.Generic;
using System.Composition;
using System.Composition.Hosting;
using System.Composition.Hosting.Core;
using System.Linq;
using System.Web;
using System.Web.Http;
using System.Web.Mvc;

namespace Partnerinfo.Composition
{
    /// <summary>
    /// Provides composition services to ASP.NET MVC by integrating DependencyResolver with
    /// the Managed Extensibility Framework (MEF). This class is self-configuring and will be
    /// enabled by simply being present in the application's Bin directory. Most applications
    /// should not need to access this class.
    /// </summary>    
    internal static class HttpCompositionProvider
    {
        private static CompositionHost s_container;
        private static ExportFactory<CompositionContext> s_factory;

        /// <summary>
        /// Gets the current initialized scope.
        /// </summary>
        /// <value>
        /// The current initialized scope.
        /// </value>
        internal static Export<CompositionContext> CurrentInitializedScope
        {
            get
            {
                return HttpContext.Current?.Items[typeof(HttpCompositionProvider)] as Export<CompositionContext>;
            }

            private set
            {
                var context = HttpContext.Current;
                if (context != null)
                {
                    context.Items[typeof(HttpCompositionProvider)] = value;
                }
            }
        }

        /// <summary>
        /// Gets the current composition context.
        /// </summary>
        /// <value>
        /// The current composition context.
        /// </value>
        public static CompositionContext Current
        {
            get
            {
                var current = CurrentInitializedScope;
                if (current == null)
                {
                    current = s_factory.CreateExport();
                    CurrentInitializedScope = current;
                }
                return current.Value;
            }
        }

        /// <summary>
        /// Used to override the default conventions for controller/part dependency injection.
        /// Cannot be used in conjunction with any other methods on this type. Most applications
        /// should not use this method.
        /// </summary>
        /// <param name="configuration">A configuration containing the controller types and other parts that
        /// should be used by the composition provider.</param>
        public static void SetConfiguration(HttpConfiguration httpConfiguration, ContainerConfiguration configuration)
        {
            // We add RSF with no conventions (overriding anything set as the default in configuration)
            s_container = configuration.CreateContainer();

            var factoryContract = new CompositionContract(
                typeof(ExportFactory<CompositionContext>),
                null,
                new Dictionary<string, object>
                {
                    { "SharingBoundaryNames", new[] { SharingBoundaries.HttpRequest } }
                });

            s_factory = (ExportFactory<CompositionContext>)s_container.GetExport(factoryContract);

            // Configure ASP.NET API
            httpConfiguration.DependencyResolver = new ApiDependencyResolver();
            httpConfiguration.Services.Add(typeof(System.Web.Http.Filters.IFilterProvider), new ApiFilterProvider());

            // Configure ASP.NET MVC
            DependencyResolver.SetResolver(new MvcDependencyResolver());
            FilterProviders.Providers.Remove(FilterProviders.Providers.OfType<FilterAttributeFilterProvider>().SingleOrDefault());
            FilterProviders.Providers.Add(new MvcFilterProvider());
            ModelBinderProviders.BinderProviders.Add(new MvcModelBinderProvider());
        }
    }
}
