// Copyright (c) János Janka. All rights reserved.

using Hangfire;
using Hangfire.Dashboard;
using Hangfire.SqlServer;
using Owin;
using Partnerinfo.Composition;

namespace Partnerinfo
{
    public partial class Startup
    {
        public static void ConfigureHangFire(IAppBuilder app)
        {
            var options = new BackgroundJobServerOptions();
            var storage = new SqlServerStorage("PartnerDbContext");

            JobStorage.Current = storage;
            JobActivator.Current = new HangFireJobActivator(HttpCompositionProvider.Current);

            app.UseHangfireServer();
            app.UseHangfireDashboard("/admin/jobs", new DashboardOptions
            {
                AuthorizationFilters = new[]
                {
                    new AuthorizationFilter { Roles = "Admin" }
                }
            });

            // RegisterCommandListenerJob();
            // RegisterCommandCleanerJob();
        }

        //private static void RegisterCommandListenerJob()
        //{
        //    RecurringJob.AddOrUpdate<CommandListenerJob>(c => c.Execute(), Cron.Minutely);
        //}

        //private static void RegisterCommandCleanerJob()
        //{
        //    RecurringJob.AddOrUpdate<CommandCleanerJob>(c => c.Execute(), Cron.Daily);
        //}
    }
}