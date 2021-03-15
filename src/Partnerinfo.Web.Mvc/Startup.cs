// Copyright (c) János Janka. All rights reserved.

using System.Web.Http;
using System.Web.Optimization;
using Owin;

[assembly: Microsoft.Owin.OwinStartup(typeof(Partnerinfo.Startup))]

namespace Partnerinfo
{
    public partial class Startup
    {
        /// <summary>
        /// Application HTTP Configuration
        /// </summary>
        public static HttpConfiguration HttpConfiguration { get; private set; }

        /// <summary>
        /// Application Bundle Collection
        /// </summary>
        public static BundleCollection BundleCollection { get; private set; }

        public void Configuration(IAppBuilder app)
        {
            HttpConfiguration = GlobalConfiguration.Configuration;

            BundleCollection = BundleTable.Bundles;
#if DEBUG
            BundleTable.EnableOptimizations = false;
#else
            BundleTable.EnableOptimizations = true;
#endif

            ConfigureBundles(app);
            ConfigureDepi(app);
            ConfigureDb(app);
            ConfigureAuth(app);
            ConfigureApi(app);
            ConfigureMvc(app);
            ConfigureHangFire(app);
            ConfigureSignalR(app);
        }
    }
}
