// Copyright (c) János Janka. All rights reserved.

using System.Web.Mvc;

namespace Partnerinfo.Identity.Models
{
    internal class OAuthLoginResult : ActionResult
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="OAuthLoginResult" /> class.
        /// </summary>
        public OAuthLoginResult(string provider, string returnUrl)
        {
            Provider = provider;
            ReturnUrl = returnUrl;
        }

        public string Provider { get; private set; }

        public string ReturnUrl { get; private set; }

        /// <summary>
        /// Enables processing of the result of an action method by a custom type that inherits from the <see cref="T:System.Web.Mvc.ActionResult" /> class.
        /// </summary>
        /// <param name="context">The context in which the result is executed. The context information includes the controller, HTTP content, request context, and route data.</param>
        public override void ExecuteResult(ControllerContext context)
        {
            Microsoft.Web.WebPages.OAuth.OAuthWebSecurity.RequestAuthentication(Provider, ReturnUrl);
        }
    }
}