// Copyright (c) János Janka. All rights reserved.

using System;
using System.Net;
using System.Net.Http;
using System.Web.Http.Controllers;
using System.Web.Http.Filters;

namespace Partnerinfo.Filters
{
    public sealed class AuthorizeProjectAttribute : AuthorizationFilterAttribute
    {
        public static readonly string IdentityKey = "ProjectIdentity";

        /// <summary>
        /// Called when an action is being authorized.
        /// </summary>
        /// <remarks>You can use <see cref="AllowAnonymousAttribute" /> to cause authorization checks to be skipped for a particular action or controller.</remarks>
        /// <seealso cref="IsAuthorized(HttpActionContext)" />
        /// <param name="actionContext">The context.</param>
        /// <exception cref="ArgumentNullException">The context parameter is null.</exception>
        public override void OnAuthorization(HttpActionContext actionContext)
        {
            if (actionContext == null)
            {
                throw new ArgumentNullException(nameof(actionContext));
            }
            var ticket = GetAuthTicket(actionContext.Request);
            if (ticket == null)
            {
                HandleUnauthorizedRequest(actionContext);
            }
            else
            {
                actionContext.Request.Properties[IdentityKey] = ticket;
            }
        }

        /// <summary>
        /// Processes requests that fail authorization. This default implementation creates a new response with the
        /// Unauthorized status code. Override this method to provide your own handling for unauthorized requests.
        /// </summary>
        /// <param name="actionContext">The context.</param>
        private void HandleUnauthorizedRequest(HttpActionContext actionContext)
        {
            if (actionContext == null)
            {
                throw new ArgumentNullException(nameof(actionContext));
            }
            actionContext.Response = actionContext.ControllerContext.Request.CreateErrorResponse(HttpStatusCode.Unauthorized, "Unauthorized request.");
        }

        /// <summary>
        /// Gets the authentication ticket from the request.
        /// </summary>
        private AuthTicket GetAuthTicket(HttpRequestMessage request)
        {
            if (request == null)
            {
                throw new ArgumentNullException(nameof(request));
            }
            if (request.Headers == null || request.Headers.Authorization == null)
            {
                return null;
            }
            string value = request.Headers.Authorization.Parameter;
            if (value == null)
            {
                return null;
            }
            return AuthUtility.Unprotect(value);
        }
    }
}