// Copyright (c) János Janka. All rights reserved.

using System;
using System.Web;
using Microsoft.Web.Infrastructure.DynamicModuleHelper;

[assembly: System.Web.PreApplicationStartMethod(typeof(Partnerinfo.Composition.HttpCompositionScopeModule), "Register")]

namespace Partnerinfo.Composition
{
    /// <summary>
    /// Provides lifetime management for the <see cref="HttpCompositionProvider" /> type.
    /// This module is automatically injected into the ASP.NET request processing
    /// pipeline at startup and should not be called by user code.
    /// </summary>
    public class HttpCompositionScopeModule : IHttpModule
    {
        private static bool s_isInitialized;

        /// <summary>
        /// Initializes a module and prepares it to handle requests.
        /// </summary>
        /// <param name="context">An <see cref="T:System.Web.HttpApplication" /> that provides access to the methods, properties, and events common to all application objects within an ASP.NET application</param>
        public void Init(HttpApplication context)
        {
            context.EndRequest += OnEndRequest;
        }

        /// <summary>
        /// Disposes of the resources (other than memory) used by the module that implements <see cref="T:System.Web.IHttpModule" />.
        /// </summary>
        public void Dispose()
        {
        }

        /// <summary>
        /// Disposes the composition scope.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="EventArgs" /> instance containing the event data.</param>
        private static void OnEndRequest(object sender, EventArgs e)
        {
            HttpCompositionProvider.CurrentInitializedScope?.Dispose();
        }

        /// <summary>
        /// Register the module. This method is automatically called
        /// at startup and should not be called by user code.
        /// </summary>
        public static void Register()
        {
            if (!s_isInitialized)
            {
                s_isInitialized = true;
                DynamicModuleUtility.RegisterModule(typeof(HttpCompositionScopeModule));
            }
        }
    }
}