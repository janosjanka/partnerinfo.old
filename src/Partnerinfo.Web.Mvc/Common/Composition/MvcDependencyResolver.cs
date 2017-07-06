// Copyright (c) János Janka. All rights reserved.

using System;
using System.Collections.Generic;
using System.Web.Mvc;

namespace Partnerinfo.Composition
{
    /// <summary>
    /// Defines the methods that simplify service location and dependency resolution.
    /// </summary>
    internal class MvcDependencyResolver : IDependencyResolver
    {
        /// <summary>
        /// Resolves singly registered services that support arbitrary object creation.
        /// </summary>
        /// <param name="serviceType">The type of the requested service or object.</param>
        /// <returns>
        /// The requested service or object.
        /// </returns>
        public object GetService(Type serviceType)
        {
            return CompositionHelper.GetService(serviceType);
        }

        /// <summary>
        /// Resolves multiply registered services.
        /// </summary>
        /// <param name="serviceType">The type of the requested services.</param>
        /// <returns>
        /// The requested services.
        /// </returns>
        public IEnumerable<object> GetServices(Type serviceType)
        {
            return CompositionHelper.GetServices(serviceType);
        }
    }
}