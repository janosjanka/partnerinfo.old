// Copyright (c) János Janka. All rights reserved.

using System.Net;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Routing;
using Partnerinfo.Identity;
using Partnerinfo.Logging.Models;
using Partnerinfo.Logging.Results;
using Partnerinfo.Project;

namespace Partnerinfo.Logging.Controllers
{
    /// <summary>
    /// Defines API actions for event categories.
    /// </summary>
    [Authorize]
    [RoutePrefix("api/logging/categories")]
    public sealed class CategoriesController : ApiController
    {
        private const string GetByIdRouteName = "Logging.Categories.GetById";

        private readonly UserManager _userManager;
        private readonly ProjectManager _projectManager;
        private readonly CategoryManager _categoryManager;

        /// <summary>
        /// Initializes a new instance of the <see cref="CategoriesController" /> class.
        /// </summary>
        public CategoriesController(UserManager userManager, ProjectManager projectManager, CategoryManager categoryManager)
        {
            _userManager = userManager;
            _projectManager = projectManager;
            _categoryManager = categoryManager;
        }

        /// <summary>
        /// Represents an action that handles only HTTP GET requests.
        /// </summary>
        [Route("")]
        public async Task<IHttpActionResult> GetAllAsync([FromUri] CategoryQueryDto model, CancellationToken cancellationToken)
        {
            if (model == null)
            {
                model = new CategoryQueryDto();
            }
            var user = await _userManager.FindByIdAsync(ApiSecurity.CurrentUserId, cancellationToken);
            if (user == null)
            {
                return NotFound();
            }
            var project = default(ProjectItem);
            if (model.ProjectId != null)
            {
                project = await _projectManager.FindByIdAsync((int)model.ProjectId, cancellationToken);
                if (project == null)
                {
                    return NotFound();
                }
            }
            return Ok(await _categoryManager.FindAllAsync(user, project, model.OrderBy, model.Fields, cancellationToken));
        }

        /// <summary>
        /// Represents an action that handles only HTTP GET requests.
        /// </summary>
        [Route("{id}", Name = GetByIdRouteName)]
        public async Task<IHttpActionResult> GetByIdAsync(int id, CancellationToken cancellationToken)
        {
            var category = await _categoryManager.FindByIdAsync(id, CategoryField.None, cancellationToken);
            if (category == null)
            {
                return NotFound();
            }
            return new CategoryContentResult(category, this);
        }

        /// <summary>
        /// Represents an action that handles only HTTP POST requests.
        /// </summary>
        [Route("")]
        public async Task<IHttpActionResult> PostAsync([FromBody] CategoryItemDto model, CancellationToken cancellationToken)
        {
            if (model == null || !ModelState.IsValid)
            {
                return BadRequest();
            }
            ProjectItem project = null;
            if (model.ProjectId != null)
            {
                project = await _projectManager.FindByIdAsync((int)model.ProjectId, cancellationToken);
                if (project == null)
                {
                    return BadRequest("The specified project was not found.");
                }
            }
            var category = new CategoryItem { Name = model.Name, Color = model.Color };
            await _categoryManager.CreateAsync(new AccountItem { Id = ApiSecurity.CurrentUserId }, project, category, cancellationToken);
            return new CategoryContentResult(HttpStatusCode.Created, category, this);
        }

        /// <summary>
        /// Represents an action that handles only HTTP PUT requests.
        /// </summary>
        [Route("{id}")]
        public async Task<IHttpActionResult> PutAsync(int id, [FromBody] CategoryItemDto model, CancellationToken cancellationToken)
        {
            if (model == null || !ModelState.IsValid)
            {
                return BadRequest();
            }
            var category = new CategoryItem { Id = id, Name = model.Name, Color = model.Color };
            await _categoryManager.UpdateAsync(category, cancellationToken);
            return new CategoryContentResult(category, this);
        }

        /// <summary>
        /// Represents an action that handles only HTTP DELETE requests.
        /// </summary>
        [Route("{id}")]
        public async Task<IHttpActionResult> DeleteAsync(int id, CancellationToken cancellationToken)
        {
            var category = await _categoryManager.FindByIdAsync(id, CategoryField.None, cancellationToken);
            if (category != null)
            {
                await _categoryManager.DeleteAsync(category, cancellationToken);
            }
            return StatusCode(HttpStatusCode.NoContent);
        }
    }
}
