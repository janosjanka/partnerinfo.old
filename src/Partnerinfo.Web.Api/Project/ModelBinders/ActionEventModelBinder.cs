// Copyright (c) János Janka. All rights reserved.

using System.Diagnostics;
using System.Web.Http.Controllers;
using System.Web.Http.ModelBinding;
using System.Web.Http.ValueProviders;
using Partnerinfo.Project.Actions;

namespace Partnerinfo.Project.ModelBinders
{
    public sealed class ActionEventModelBinder : IModelBinder
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
            Debug.Assert(bindingContext.ModelType == typeof(ActionEventArgs));

            string args = GetStringValue(bindingContext.ValueProvider, bindingContext.ModelName);
            if (args != null)
            {
                string salt = GetStringValue(bindingContext.ValueProvider, "salt");
                var model = ActionEventConverter.DecodeActionEvent(args, salt);
                if (model != null)
                {
                    bindingContext.Model = model;
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// Gets the value from the specified value provider.
        /// </summary>
        private static string GetStringValue(IValueProvider valueProvider, string name)
        {
            var result = valueProvider.GetValue(name);
            if (result != null)
            {
                return result.RawValue as string;
            }
            return null;
        }
    }
}