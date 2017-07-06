// Copyright (c) János Janka. All rights reserved.

using System;
using Owin;
using Partnerinfo.Composition;
using Partnerinfo.Logging;
using Partnerinfo.Portal;
using Partnerinfo.Project;

namespace Partnerinfo
{
    public partial class Startup
    {
        public static void ConfigureDepi(IAppBuilder app)
        {
            HttpCompositionProvider.SetConfiguration(HttpConfiguration, new AppContainerConfiguration());

            app.Properties.Add("LogManagerFactory",
                (Func<LogManager>)(() => HttpCompositionProvider.Current.GetExport<LogManager>()));
            app.Properties.Add("ProjectManagerFactory",
                (Func<ProjectManager>)(() => HttpCompositionProvider.Current.GetExport<ProjectManager>()));
            app.Properties.Add("PortalManagerFactory",
                (Func<PortalManager>)(() => HttpCompositionProvider.Current.GetExport<PortalManager>()));
        }
    }
}