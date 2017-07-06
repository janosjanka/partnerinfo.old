// Copyright (c) János Janka. All rights reserved.

using System.Diagnostics;
using System.Web.Http;
using System.Web.Http.Controllers;
using System.Web.Http.ModelBinding;
using Partnerinfo.Project.Actions;

namespace Partnerinfo.Project.ModelBinders
{
    public sealed class ActionLinkModelBinder : IModelBinder
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
            Debug.Assert(bindingContext.ModelType == typeof(ActionLink));

            var service = GlobalConfiguration.Configuration.DependencyResolver.GetService(typeof(IActionLinkService)) as IActionLinkService;
            string paramUri = bindingContext.ValueProvider.GetValue("paramUri")?.AttemptedValue;
            if (paramUri != null)
            {
                string customUri = bindingContext.ValueProvider.GetValue("customUri")?.AttemptedValue;
                bindingContext.Model = service.UrlTokenDecode(paramUri, customUri);
                return true;
            }
            return false;
        }
    }
}