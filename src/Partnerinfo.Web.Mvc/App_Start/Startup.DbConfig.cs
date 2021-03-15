// Copyright (c) János Janka. All rights reserved.

using Owin;
using WebMatrix.WebData;

namespace Partnerinfo
{
    public partial class Startup
    {
        public static void ConfigureDb(IAppBuilder app)
        {
            WebSecurity.InitializeDatabaseConnection("PartnerDbContext", "User", "Id", "Email", autoCreateTables: false);
        }
    }
}