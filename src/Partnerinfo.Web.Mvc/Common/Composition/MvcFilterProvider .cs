// Copyright (c) János Janka. All rights reserved.

using System.Collections.Generic;
using System.Composition;
using System.Web.Mvc;

namespace Partnerinfo.Composition
{
    internal class MvcFilterProvider : FilterAttributeFilterProvider
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="MvcFilterProvider" /> class.
        /// </summary>
        public MvcFilterProvider()
            : base(cacheAttributeInstances: false)
        {
        }

        /// <summary>
        /// Gets a collection of custom action attributes.
        /// </summary>
        /// <param name="controllerContext">The controller context.</param>
        /// <param name="actionDescriptor">The action descriptor.</param>
        /// <returns>
        /// A collection of custom action attributes.
        /// </returns>
        protected override IEnumerable<FilterAttribute> GetActionAttributes(ControllerContext controllerContext, ActionDescriptor actionDescriptor)
        {
            var attributes = base.GetActionAttributes(controllerContext, actionDescriptor);
            if (attributes != null)
            {
                ComposeAttributes(attributes);
            }
            return attributes;
        }

        /// <summary>
        /// Gets a collection of controller attributes.
        /// </summary>
        /// <param name="controllerContext">The controller context.</param>
        /// <param name="actionDescriptor">The action descriptor.</param>
        /// <returns>
        /// A collection of controller attributes.
        /// </returns>
        protected override IEnumerable<FilterAttribute> GetControllerAttributes(ControllerContext controllerContext, ActionDescriptor actionDescriptor)
        {
            var attributes = base.GetControllerAttributes(controllerContext, actionDescriptor);
            if (attributes != null)
            {
                ComposeAttributes(attributes);
            }
            return attributes;
        }

        /// <summary>
        /// Composes filter attributes.
        /// </summary>
        /// <param name="attributes">The attributes.</param>
        private void ComposeAttributes(IEnumerable<FilterAttribute> attributes)
        {
            foreach (var attribute in attributes)
            {
                HttpCompositionProvider.Current.SatisfyImports(attribute);
            }
        }
    }
}