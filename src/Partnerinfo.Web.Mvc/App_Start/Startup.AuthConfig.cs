// Copyright (c) János Janka. All rights reserved.

using System.Web.Helpers;
using System.Web.Routing;
using Microsoft.Web.WebPages.OAuth;
using Owin;
using Partnerinfo.Identity.OAuth;
using Partnerinfo.Properties;

namespace Partnerinfo
{
    public partial class Startup
    {
        public static void ConfigureAuth(IAppBuilder app)
        {
            // Sets the name of the cookie that is used by the anti-forgery system
            AntiForgeryConfig.CookieName = ResourceKeys.AntiForgeryTokenName;

            //// To let users of this site log in using their accounts from other sites such as Microsoft, Facebook, and Twitter,
            //// you must update this site. For more information visit http://go.microsoft.com/fwlink/?LinkID=252166

            OAuthWebSecurity.RegisterClient(
                new OAuthFacebookClient(Settings.Default.OAuthFacebookAppId, Settings.Default.OAuthFacebookAppSecret),
                "Facebook",
                new RouteValueDictionary { { "logo", "i facebook" } });

            OAuthWebSecurity.RegisterClient(
                new OAuthGoogleClient(Settings.Default.OAuthGoogleAppId, Settings.Default.OAuthGoogleConsumerSecret),
                "Google",
                new RouteValueDictionary { { "logo", "i google" } });

            OAuthWebSecurity.RegisterClient(
                new OAuthMicrosoftClient(Settings.Default.OAuthMicrosoftAppId, Settings.Default.OAuthMicrosoftAppSecret),
                "Microsoft",
                new RouteValueDictionary { { "logo", "i microsoft" } });
        }
    }
}
