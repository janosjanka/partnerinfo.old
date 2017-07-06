// Copyright (c) János Janka. All rights reserved.

using Owin;

namespace Partnerinfo
{
    public partial class Startup
    {
        public static void ConfigureApi(IAppBuilder app)
        {
            ApiConfig.Configure(app, HttpConfiguration);
        }
    }
}