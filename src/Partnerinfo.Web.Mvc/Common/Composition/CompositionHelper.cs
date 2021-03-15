// Copyright (c) János Janka. All rights reserved.

using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace Partnerinfo.Composition
{
    internal static class CompositionHelper
    {
        /// <summary>
        /// Resolves singly registered services that support arbitrary object creation.
        /// </summary>
        /// <param name="serviceType">The type of the requested service or object.</param>
        /// <returns>
        /// The requested service or object.
        /// </returns>
        public static object GetService(Type serviceType)
        {
            object export;

            if (!HttpCompositionProvider.Current.TryGetExport(serviceType, null, out export))
            {
                Debug.WriteLine($"{serviceType.Name} cannot be resolved.", "Managed Extensibility Framework");
            }

            return export;
        }

        /// <summary>
        /// Resolves multiply registered services.
        /// </summary>
        /// <param name="serviceType">The type of the requested services.</param>
        /// <returns>
        /// The requested services.
        /// </returns>
        public static IEnumerable<object> GetServices(Type serviceType)
        {
            return HttpCompositionProvider.Current.GetExports(serviceType);
        }
    }
}