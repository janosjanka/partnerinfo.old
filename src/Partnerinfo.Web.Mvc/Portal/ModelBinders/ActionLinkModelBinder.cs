// Copyright (c) János Janka. All rights reserved.

using System.Diagnostics;
using System.Web.Http;
using System.Web.Mvc;
using Partnerinfo.Project.Actions;

namespace Partnerinfo.Portal.ModelBinders
{
    public sealed class ActionLinkModelBinder : IModelBinder
    {
        private readonly IActionLinkService _service;

        /// <summary>
        /// Initializes a new instance of the <see cref="ActionLinkModelBinder" /> class.
        /// </summary>
        public ActionLinkModelBinder()
            : this(GlobalConfiguration.Configuration.DependencyResolver.GetService(typeof(IActionLinkService)) as IActionLinkService)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ActionLinkModelBinder" /> class.
        /// </summary>
        /// <param name="service">The project manager.</param>
        public ActionLinkModelBinder(IActionLinkService service)
        {
            _service = service;
        }

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
            Debug.Assert(bindingContext.ModelType == typeof(ActionLink));

            var actionParts = bindingContext.ValueProvider.GetValue(ResourceKeys.ActionParamName)?.AttemptedValue?.Split('/');

            if (actionParts == null || actionParts.Length == 0)
            {
                return null;
            }

            if (actionParts.Length > 1)
            {
                return _service.UrlTokenDecode(actionParts[0], actionParts[1]);
            }

            return _service.UrlTokenDecode(actionParts[0]);
        }
    }
}