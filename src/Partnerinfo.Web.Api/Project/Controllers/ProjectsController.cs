// Copyright (c) János Janka. All rights reserved.

using System.Net;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Description;
using System.Web.Routing;
using Partnerinfo.Identity;
using Partnerinfo.Project.Models;
using Partnerinfo.Security;

namespace Partnerinfo.Project.Controllers
{
    [Authorize]
    [RoutePrefix("api/projects")]
    public sealed class ProjectsController : ApiController
    {
        private readonly UserManager _userManager;
        private readonly ProjectManager _projectManager;

        public ProjectsController(UserManager userManager, ProjectManager projectManager)
        {
            _userManager = userManager;
            _projectManager = projectManager;
        }

        /// <summary>
        /// Represents an action that handles only HTTP GET requests.
        /// </summary>
        [Route("")]
        [ResponseType(typeof(ListResult<ProjectItem>))]
        public async Task<IHttpActionResult> GetAllAsync([FromUri] ProjectQueryDto model, CancellationToken cancellationToken)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest();
            }
            if (model == null)
            {
                model = new ProjectQueryDto();
            }
            return Ok(await _projectManager.FindAllAsync(
                ApiSecurity.CurrentUserId,
                model.Name,
                model.OrderBy,
                model.Page,
                model.Limit,
                model.Fields,
                cancellationToken));
        }

        /// <summary>
        /// Represents an action that handles only HTTP GET requests.
        /// </summary>
        [Route("{id}", Name = "Projects.GetById")]
        [ResponseType(typeof(ProjectItem))]
        public async Task<IHttpActionResult> GetByIdAsync(int id, CancellationToken cancellationToken)
        {
            var project = await _projectManager.FindByIdAsync(id, cancellationToken);
            await ApiSecurity.AuthorizeAsync(project, AccessPermission.CanView, cancellationToken);
            return Ok(project);
        }

        /// <summary>
        /// Represents an action that handles only HTTP POST requests.
        /// </summary>
        [Route("")]
        [ResponseType(typeof(ProjectResultDto))]
        public async Task<IHttpActionResult> PostAsync([FromBody] ProjectItemDto model, CancellationToken cancellationToken)
        {
            if (model == null)
            {
                return BadRequest();
            }
            var user = await _userManager.FindByEmailAsync(User.Identity.Name, cancellationToken);
            if (user == null)
            {
                return NotFound();
            }
            if (model.Sender == null || model.Sender.Address == null)
            {
                model.Sender = MailAddressItem.Create(ApiSecurity.CurrentUserName);
            }

            var project = new ProjectItem { Name = model.Name, Sender = model.Sender };
            var validationResult = await _projectManager.CreateAsync(project, new[] { user }, cancellationToken);
            if (!validationResult.Succeeded)
            {
                return this.ValidationContent(validationResult);
            }

            var contact = new ContactItem { Email = model.Sender };
            var businessTag = new BusinessTagItem { Name = "Feliratkozott", Color = "#00ff00" };
            await _projectManager.AddContactAsync(project, contact, cancellationToken);
            await _projectManager.AddBusinessTagAsync(project, businessTag, cancellationToken);
            await _projectManager.SetBusinessTagsAsync(new[] { contact.Id }, new[] { businessTag.Id }, new int[0], cancellationToken);

            return CreatedAtRoute("Projects.GetById", new RouteValueDictionary { { "id", project.Id } }, new ProjectResultDto
            {
                Id = project.Id,
                Name = project.Name,
                Sender = project.Sender
            });
        }

        /// <summary>
        /// Represents an action that handles only HTTP PUT requests.
        /// </summary>
        [Route("{id}")]
        [ResponseType(typeof(ProjectResultDto))]
        public async Task<IHttpActionResult> PutAsync(int id, [FromBody] ProjectItemDto model, CancellationToken cancellationToken)
        {
            if (model == null || !ModelState.IsValid)
            {
                return BadRequest();
            }
            var project = new ProjectItem { Id = id, Name = model.Name, Sender = model.Sender };
            await ApiSecurity.AuthorizeAsync(project, AccessPermission.CanEdit, cancellationToken);
            var validationResult = await _projectManager.UpdateAsync(project, cancellationToken);
            if (!validationResult.Succeeded)
            {
                return this.ValidationContent(validationResult);
            }
            return Ok(new ProjectResultDto
            {
                Id = project.Id,
                Name = project.Name,
                Sender = project.Sender
            });
        }

        /// <summary>
        /// Represents an action that handles only HTTP DELETE requests.
        /// </summary>
        [Route("{id}")]
        public async Task<IHttpActionResult> DeleteAsync(int id, CancellationToken cancellationToken)
        {
            var project = await _projectManager.FindByIdAsync(id, cancellationToken);
            await ApiSecurity.AuthorizeAsync(project, AccessPermission.IsOwner, cancellationToken);
            await _projectManager.DeleteAsync(project, cancellationToken);
            return StatusCode(HttpStatusCode.NoContent);
        }
    }
}
