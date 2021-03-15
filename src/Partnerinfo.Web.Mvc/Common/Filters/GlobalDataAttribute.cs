// Copyright (c) János Janka. All rights reserved.

using System;
using System.Collections.Generic;
using System.Web.Mvc;

namespace Partnerinfo.Filters
{
    /// <summary>
    /// Provides anonym user identification for an ASP.NET MVC action.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, Inherited = false, AllowMultiple = false)]
    public class GlobalDataAttribute : ActionFilterAttribute
    {
        /// <summary>
        /// Called by the ASP.NET MVC framework before the action method executes.
        /// </summary>
        /// <param name="filterContext">The filter context.</param>
        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            var appClass = new Queue<string>();
            var appTitle = new Queue<string>();

            if (filterContext.HttpContext.User?.Identity?.IsAuthenticated == true)
            {
                appClass.Enqueue("ui-logged-in");
            }
            else
            {
                appClass.Enqueue("ui-logged-out");
            }

            appTitle.Enqueue(Properties.Settings.Default.AppTitle);

            filterContext.Controller.ViewBag.AppClass = appClass;
            filterContext.Controller.ViewBag.AppTitle = appTitle;
            filterContext.Controller.ViewBag.AppLogo = Properties.Settings.Default.AppLogo;

            base.OnActionExecuting(filterContext);
        }
    }
}