// Copyright (c) János Janka. All rights reserved.

using System;

namespace Partnerinfo
{
    /// <summary>
    /// Represents a dependency injection container for application services.
    /// </summary>
    public interface IServiceResolver
    {
        /// <summary>
        /// Resolves singly registered services that support arbitrary object creation.
        /// </summary>
        /// <param name="serviceType">The service type.</param>
        /// <returns>
        /// The requested service or object.
        /// </returns>
        object Resolve(Type serviceType);
    }
}
