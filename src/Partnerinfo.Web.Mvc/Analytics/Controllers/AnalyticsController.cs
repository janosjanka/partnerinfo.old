// Copyright (c) János Janka. All rights reserved.

using System.Web.Mvc;

namespace Partnerinfo.Analytics.Controllers
{
    /// <summary>
    /// Provides methods that respond to HTTP requests that are made to an ASP.NET MVC Web site.
    /// </summary>
    [Authorize]
    public sealed class AnalyticsController : Controller
    {
        /// <summary>
        /// Creates a ViewResult object that renders a view to the response.
        /// </summary>
        /// <returns>Returns the view that matches the action result name.</returns>
        public ActionResult Index() => View();
    }
}