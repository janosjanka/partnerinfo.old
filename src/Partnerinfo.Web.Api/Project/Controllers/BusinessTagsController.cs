// Copyright (c) János Janka. All rights reserved.

using System.Net;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Routing;
using Partnerinfo.Project.Models;
using Partnerinfo.Project.Results;
using Partnerinfo.Security;

namespace Partnerinfo.Project.Controllers
{
    [Authorize]
    [RoutePrefix("api/project/business-tags")]
    public sealed class BusinessTagsController : ApiController
    {
        private readonly ProjectManager _projectManager;

        /// <summary>
        /// Initializes a new instance of the <see cref="BusinessTagsController" /> class.
        /// </summary>
        /// <param name="projectManager">The project manager that the <see cref="BusinessTagsController" /> operates against.</param>
        public BusinessTagsController(ProjectManager projectManager)
        {
            _projectManager = projectManager;
        }

        /// <summary>
        /// Represents an action that handles only HTTP GET requests.
        /// </summary>
        [Route("~/api/projects/{projectid}/business-tags", Name = "Project.BusinessTags.GetAll")]
        public async Task<IHttpActionResult> GetAllAsync(int projectId, CancellationToken cancellationToken)
        {
            var project = await _projectManager.FindByIdAsync(projectId, cancellationToken);
            await ApiSecurity.AuthorizeAsync(project, AccessPermission.CanView, cancellationToken);
            return Ok(await _projectManager.GetBusinessTagsAsync(project, cancellationToken));
        }

        /// <summary>
        /// Represents an action that handles only HTTP GET requests.
        /// </summary>
        [Route("{id}", Name = "Project.BusinessTags.GetById")]
        public async Task<IHttpActionResult> GetByIdAsync(int id, CancellationToken cancellationToken)
        {
            var businessTag = await _projectManager.GetBusinessTagByIdAsync(id, cancellationToken);
            if (businessTag == null)
            {
                return NotFound();
            }
            return new BusinessTagContentResult(HttpStatusCode.OK, businessTag, this);
        }

        /// <summary>
        /// Represents an action that handles only HTTP GET requests.
        /// </summary>
        [Route("~/api/projects/{projectid}/business-tags", Name = "Project.BusinessTags.GetByName")]
        public async Task<IHttpActionResult> GetByNameAsync(int projectId, string name, CancellationToken cancellationToken)
        {
            var project = await _projectManager.FindByIdAsync(projectId, cancellationToken);
            await ApiSecurity.AuthorizeAsync(project, AccessPermission.CanView, cancellationToken);

            var businessTag = await _projectManager.GetBusinessTagByNameAsync(project, name, cancellationToken);
            if (businessTag == null)
            {
                return NotFound();
            }
            return new BusinessTagContentResult(HttpStatusCode.OK, businessTag, this);
        }

        /// <summary>
        /// Represents an action that handles only HTTP POST requests.
        /// </summary>
        [Route("~/api/projects/{projectid}/business-tags")]
        public async Task<IHttpActionResult> PostAsync(int projectId, [FromBody] BusinessTagItemDto model, CancellationToken cancellationToken)
        {
            if (model == null || !ModelState.IsValid)
            {
                return BadRequest();
            }
            var project = await _projectManager.FindByIdAsync(projectId, cancellationToken);
            await ApiSecurity.AuthorizeAsync(project, AccessPermission.CanEdit, cancellationToken);

            var businessTag = new BusinessTagItem { Name = model.Name, Color = model.Color };
            var validationResult = await _projectManager.AddBusinessTagAsync(project, businessTag, cancellationToken);
            if (!validationResult.Succeeded)
            {
                return this.ValidationContent(validationResult);
            }
            return new BusinessTagContentResult(HttpStatusCode.Created, businessTag, this);
        }

        /// <summary>
        /// Represents an action that handles only HTTP PUT requests.
        /// </summary>
        [Route("{id}")]
        public async Task<IHttpActionResult> PutAsync(int id, [FromBody] BusinessTagItemDto model, CancellationToken cancellationToken)
        {
            if (model == null || !ModelState.IsValid)
            {
                return BadRequest();
            }
            var businessTag = await _projectManager.GetBusinessTagByIdAsync(id, cancellationToken);
            if (businessTag == null)
            {
                return NotFound();
            }
            var project = await _projectManager.FindByIdAsync(businessTag.Project.Id, cancellationToken);
            await ApiSecurity.AuthorizeAsync(project, AccessPermission.CanEdit, cancellationToken);

            businessTag.Name = model.Name;
            businessTag.Color = model.Color;
            var validationResult = await _projectManager.ReplaceBusinessTagAsync(project, businessTag, businessTag, cancellationToken);
            if (!validationResult.Succeeded)
            {
                return this.ValidationContent(validationResult);
            }
            return new BusinessTagContentResult(HttpStatusCode.OK, businessTag, this);
        }

        /// <summary>
        /// Represents an action that handles only HTTP PUT requests.
        /// </summary>
        [Route("{id}")]
        public async Task<IHttpActionResult> DeleteAsync(int id, CancellationToken cancellationToken)
        {
            var businessTag = await _projectManager.GetBusinessTagByIdAsync(id, cancellationToken);
            if (businessTag == null)
            {
                return NotFound();
            }
            var project = await _projectManager.FindByIdAsync(businessTag.Project.Id, cancellationToken);
            await ApiSecurity.AuthorizeAsync(project, AccessPermission.CanEdit, cancellationToken);

            var validationResult = await _projectManager.RemoveBusinessTagAsync(project, businessTag, cancellationToken);
            if (!validationResult.Succeeded)
            {
                return this.ValidationContent(validationResult);
            }
            return new BusinessTagContentResult(HttpStatusCode.OK, businessTag, this);
        }

        /// <summary>
        /// Represents an action that handles only HTTP DELETE requests.
        /// </summary>
        [Route("")]
        public async Task<IHttpActionResult> DeleteAsync(int projectId, string name, CancellationToken cancellationToken)
        {
            var project = await _projectManager.FindByIdAsync(projectId, cancellationToken);
            await ApiSecurity.AuthorizeAsync(project, AccessPermission.CanEdit, cancellationToken);

            var businessTag = await _projectManager.GetBusinessTagByNameAsync(project, name, cancellationToken);
            if (businessTag == null)
            {
                return StatusCode(HttpStatusCode.NoContent);
            }
            var validationResult = await _projectManager.RemoveBusinessTagAsync(project, businessTag, cancellationToken);
            if (!validationResult.Succeeded)
            {
                return this.ValidationContent(validationResult);
            }
            return StatusCode(HttpStatusCode.NoContent);
        }
    }
}