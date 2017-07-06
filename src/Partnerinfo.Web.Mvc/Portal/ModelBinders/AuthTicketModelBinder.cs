// Copyright (c) János Janka. All rights reserved.

using System.Diagnostics;
using System.Web;
using System.Web.Mvc;

namespace Partnerinfo.Portal.ModelBinders
{
    public sealed class AuthTicketModelBinder : IModelBinder
    {
        /// <summary>
        /// Binds the model to a value by using the specified controller context and binding context.
        /// </summary>
        /// <param name="controllerContext">The controller context.</param>
        /// <param name="bindingContext">The binding context.</param>
        /// <returns>
        /// The bound value.
        /// </returns>
        public object BindModel(ControllerContext controllerContext, ModelBindingContext bindingContext)
        {
            Debug.Assert(bindingContext.ModelType == typeof(AuthTicket));

            return GetTicketFromModel(bindingContext.ValueProvider, ResourceKeys.ContactTokenParamName)
                ?? GetTicketFromCookie(controllerContext.HttpContext.Request.Cookies, ResourceKeys.ContactTokenCookieName);
        }

        /// <summary>
        /// Gets the authentication ticket from the given value provider.
        /// </summary>
        /// <param name="provider">The value provider.</param>
        /// <param name="key">The key.</param>
        /// <returns>
        /// The authentication ticket.
        /// </returns>
        internal static AuthTicket GetTicketFromModel(IValueProvider provider, string key)
        {
            var result = provider.GetValue(key)?.AttemptedValue;
            return string.IsNullOrEmpty(result) ? null : AuthUtility.Unprotect(result);
        }

        /// <summary>
        /// Gets the authentication ticket from the given cookie collection.
        /// </summary>
        /// <param name="cookies">The cookie collection.</param>
        /// <param name="key">The key.</param>
        /// <returns>
        /// The authentication ticket.
        /// </returns>
        internal static AuthTicket GetTicketFromCookie(HttpCookieCollection cookies, string key)
        {
            var cookie = cookies[key]?.Value;
            return string.IsNullOrEmpty(cookie) ? null : AuthUtility.Unprotect(cookie);
        }
    }
}