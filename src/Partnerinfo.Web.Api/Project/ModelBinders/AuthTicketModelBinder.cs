// Copyright (c) János Janka. All rights reserved.

using System.Diagnostics;
using System.Net.Http.Headers;
using System.Web.Http.Controllers;
using System.Web.Http.ModelBinding;
using System.Web.Http.ValueProviders;

namespace Partnerinfo.Project.ModelBinders
{
    public sealed class AuthTicketModelBinder : IModelBinder
    {
        /// <summary>
        /// Binds the model to a value by using the specified controller context and binding context.
        /// </summary>
        /// <param name="actionContext">The action context.</param>
        /// <param name="bindingContext">The binding context.</param>
        /// <returns>
        /// The bound value.
        /// </returns>
        public bool BindModel(HttpActionContext actionContext, ModelBindingContext bindingContext)
        {
            Debug.Assert(bindingContext.ModelType == typeof(AuthTicket));

            var ticket =
                GetTicketFromModel(bindingContext.ValueProvider, ResourceKeys.ContactTokenParamName) ??
                GetTicketFromHeader(actionContext.ControllerContext.Request.Headers.Authorization);

            if (ticket != null)
            {
                bindingContext.Model = ticket;
                return true;
            }

            return false;
        }

        /// <summary>
        /// Gets an authentication ticket from the model
        /// </summary>
        private static AuthTicket GetTicketFromModel(IValueProvider provider, string key)
        {
            if (provider == null)
            {
                return null;
            }
            var provResult = provider.GetValue(key);
            if (provResult != null)
            {
                if (string.IsNullOrEmpty(provResult.AttemptedValue))
                {
                    return null;
                }
                return AuthUtility.Unprotect(provResult.AttemptedValue);
            }
            return null;
        }

        /// <summary>
        /// Gets an authentication ticket from the given HTTP header
        /// </summary>
        private static AuthTicket GetTicketFromHeader(AuthenticationHeaderValue header)
        {
            if (header == null
                || string.IsNullOrEmpty(header.Scheme)
                || string.IsNullOrEmpty(header.Parameter)
                || "Bearer".Equals(header.Scheme, System.StringComparison.OrdinalIgnoreCase) == false)
            {
                return null;
            }
            return AuthUtility.Unprotect(header.Parameter);
        }
    }
}