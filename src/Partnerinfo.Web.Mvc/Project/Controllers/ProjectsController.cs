// Copyright (c) János Janka. All rights reserved.

using System.Net;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Mvc;
using Partnerinfo.Security;

namespace Partnerinfo.Project.Controllers
{
    [Authorize]
    public sealed class ProjectsController : Controller
    {
        private readonly SecurityManager _securityManager;
        private readonly ProjectManager _projectManager;

        /// <summary>
        /// Initializes a new instance of the <see cref="ProjectsController" /> class.
        /// </summary>
        public ProjectsController(SecurityManager securityManager, ProjectManager projectManager)
        {
            _securityManager = securityManager;
            _projectManager = projectManager;
        }

        /// <summary>
        /// Creates a ViewResult object that renders a view to the response.
        /// </summary>
        /// <returns>
        /// Returns the view that matches the action result name.
        /// </returns>
        public async Task<System.Web.Mvc.ActionResult> Index(int? id, CancellationToken cancellationToken)
        {
            if (id == null)
            {
                return View();
            }
            var project = await _projectManager.FindByIdAsync((int)id, cancellationToken);
            if (project == null)
            {
                return HttpNotFound();
            }
            var securityResult = await _securityManager.CheckAccessAsync(project, User.Identity.Name, AccessPermission.CanView, cancellationToken);
            if (!securityResult.AccessGranted)
            {
                return new HttpStatusCodeResult(HttpStatusCode.Forbidden);
            }
            return View(project);
        }
    }
}