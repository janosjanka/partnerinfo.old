// Copyright (c) János Janka. All rights reserved.

using Microsoft.AspNet.SignalR;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Owin;
using Partnerinfo.Chat;
using Partnerinfo.Composition;
using Partnerinfo.Logging;
using Partnerinfo.SignalR;

namespace Partnerinfo
{
    public partial class Startup
    {
        public static void ConfigureSignalR(IAppBuilder app)
        {
            // Create a new serialize without supporting camelCase member names.
            // Unfortunately, SignalR uses C# style property names instead of JS style names.
            var serializer = new JsonSerializer
            {
                MissingMemberHandling = MissingMemberHandling.Ignore,
                PreserveReferencesHandling = PreserveReferencesHandling.None,
                ObjectCreationHandling = ObjectCreationHandling.Replace,
                ReferenceLoopHandling = ReferenceLoopHandling.Ignore,
                NullValueHandling = NullValueHandling.Ignore,
                ContractResolver = new CamelCaseContractResolver()
            };

            serializer.Converters.Add(new StringEnumConverter { CamelCaseText = true });
            serializer.Converters.Add(new IsoDateTimeConverter());

            GlobalHost.HubPipeline.AddModule(new ErrorHubPipelineModule());
            GlobalHost.DependencyResolver.Register(typeof(JsonSerializer), () => serializer);
            GlobalHost.DependencyResolver.Register(typeof(LogManager), () => HttpCompositionProvider.Current.GetExport<LogManager>());

            NotificationHubConfig.Configure(app);
            ChatHubConfig.Configure(app);

            app.MapSignalR("/signalr", new HubConfiguration
            {
                EnableDetailedErrors = false,
                EnableJavaScriptProxies = false,
                EnableJSONP = false
            });
        }
    }
}