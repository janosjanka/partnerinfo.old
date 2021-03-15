// Copyright (c) János Janka. All rights reserved.

using System.Web.Mvc;

namespace Partnerinfo.Controllers
{
    /// <summary>
    /// Provides methods that respond to HTTP requests that are made to an ASP.NET MVC Web site.
    /// </summary>
    public sealed class HomeController : Controller
    {
        /// <summary>
        /// Creates a ViewResult object that renders a view to the response.
        /// </summary>
        public ActionResult Index() => View();
    }
}