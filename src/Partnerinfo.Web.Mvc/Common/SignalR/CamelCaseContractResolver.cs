// Copyright (c) János Janka. All rights reserved.

using System;
using System.Reflection;
using Microsoft.AspNet.SignalR.Infrastructure;
using Newtonsoft.Json.Serialization;

namespace Partnerinfo.SignalR
{
    public class CamelCaseContractResolver : IContractResolver
    {
        private static readonly Assembly s_msAssembly = typeof(Connection).Assembly;
        private static readonly IContractResolver s_defaultContractSerializer = new DefaultContractResolver();
        private static readonly IContractResolver s_camelCaseContractResolver = new CamelCasePropertyNamesContractResolver();

        /// <summary>
        /// Resolves the contract for a given type.
        /// </summary>
        /// <param name="type">The type to resolve a contract for.</param>
        /// <returns>
        /// The contract for a given type.
        /// </returns>
        public JsonContract ResolveContract(Type type)
        {
            if (type.Assembly.Equals(s_msAssembly))
            {
                return s_defaultContractSerializer.ResolveContract(type);
            }
            return s_camelCaseContractResolver.ResolveContract(type);
        }
    }
}