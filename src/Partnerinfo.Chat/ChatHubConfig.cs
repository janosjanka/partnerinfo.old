// Copyright (c) János Janka. All rights reserved.

using System;
using Owin;
using Microsoft.AspNet.SignalR;
using Partnerinfo.Logging;
using Partnerinfo.Portal;
using Partnerinfo.Project;

namespace Partnerinfo.Chat
{
    public static class ChatHubConfig
    {
        public static void Configure(IAppBuilder app)
        {
            GlobalHost.DependencyResolver.Register(typeof(ChatManager), () =>
                new ChatManager(new ChatMemoryStore())
                {
                    LogManagerFactory = (Func<LogManager>)app.Properties["LogManagerFactory"],
                    ProjectManagerFactory = (Func<ProjectManager>)app.Properties["ProjectManagerFactory"],
                    PortalManagerFactory = (Func<PortalManager>)app.Properties["PortalManagerFactory"]
                });
        }
    }
}
