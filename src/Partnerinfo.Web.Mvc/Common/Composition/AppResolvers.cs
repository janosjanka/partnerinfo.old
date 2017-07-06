// Copyright (c) János Janka. All rights reserved.

using System;

namespace Partnerinfo.Composition
{
    public class ServiceResolver : IServiceResolver
    {
        internal static readonly ServiceResolver Default = new ServiceResolver();

        /// <summary>
        /// Resolves singly registered services that support arbitrary object creation.
        /// </summary>
        /// <param name="serviceType">The type of the requested service or object.</param>
        /// <returns>
        /// The requested service or object.
        /// </returns>
        public object Resolve(Type serviceType)
        {
            return CompositionHelper.GetService(serviceType);
        }
    }
}