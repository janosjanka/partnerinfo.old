// Copyright (c) János Janka. All rights reserved.

using System;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Routing;
using Partnerinfo.Project.Actions;
using Partnerinfo.Project.Models;
using Partnerinfo.Project.Results;
using Partnerinfo.Security;

namespace Partnerinfo.Project.Controllers
{
    [Authorize]
    [RoutePrefix("api/project/actions")]
    public sealed class ActionsController : ApiController
    {
        private readonly ProjectManager _projectManager;
        private readonly IActionLinkService _actionLinkService;

        /// <summary>
        /// Initializes a new instance of the <see cref="ActionsController" /> class.
        /// </summary>
        /// <param name="projectManager">The project manager that the <see cref="ActionsController" /> operates against.</param>
        public ActionsController(ProjectManager projectManager, IActionLinkService actionLinkService)
        {
            _projectManager = projectManager;
            _actionLinkService = actionLinkService;
        }

        /// <summary>
        /// Represents an action that handles only HTTP GET requests.
        /// </summary>
        [Route("~/api/projects/{projectid}/actions")]
        public async Task<IHttpActionResult> GetAllAsync([FromUri] ActionQueryDto model, CancellationToken cancellationToken)
        {
            if (model == null || !ModelState.IsValid)
            {
                return BadRequest();
            }

            await ApiSecurity.AuthorizeAsync(AccessObjectType.Project, model.ProjectId, AccessPermission.CanView, cancellationToken);
            return Ok(await _projectManager.GetActionsAsync(
                model.ProjectId,
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
        [Route("{id}", Name = "Project.Actions.GetById")]
        public async Task<IHttpActionResult> GetByIdAsync(int id, CancellationToken cancellationToken)
        {
            var action = await _projectManager.GetActionByIdAsync(id, cancellationToken);
            if (action == null)
            {
                return NotFound();
            }

            await ApiSecurity.AuthorizeAsync(AccessObjectType.Project, action.Project.Id, AccessPermission.CanView, cancellationToken);
            return new ActionContentResult(_actionLinkService, action, this);
        }

        /// <summary>
        /// Represents an action that handles only HTTP GET requests.
        /// </summary>
        [Route("~/api/project/actionlinks/{id}")]
        public async Task<IHttpActionResult> GetByLinkIdAsync(int id, CancellationToken cancellationToken)
        {
            var action = await _projectManager.GetActionByLinkIdAsync(id, cancellationToken);
            if (action == null)
            {
                return NotFound();
            }

            await ApiSecurity.AuthorizeAsync(AccessObjectType.Project, action.Project.Id, AccessPermission.CanView, cancellationToken);
            return new ActionContentResult(_actionLinkService, action, this);
        }

        /// <summary>
        /// Represents an action that handles only HTTP GET requests.
        /// </summary>
        [Route("")]
        public async Task<IHttpActionResult> GetByLinkAsync(string link, CancellationToken cancellationToken)
        {
            var actionLink = _actionLinkService.DecodeLink(link);
            var action = await _projectManager.GetActionByIdAsync(actionLink.ActionId, cancellationToken);
            var contact = actionLink.ContactId != null ? await _projectManager.GetContactByIdAsync((int)actionLink.ContactId, ContactField.None, cancellationToken) : null;

            await ApiSecurity.AuthorizeAsync(AccessObjectType.Project, action.Project.Id, AccessPermission.CanView, cancellationToken);
            return new ActionLinkContentResult(_actionLinkService, action, contact, actionLink.CustomUri, this);
        }

        /// <summary>
        /// Represents an action that handles only HTTP GET requests.
        /// </summary>
        [Route("{actionid}/link")]
        public async Task<IHttpActionResult> GetLinkByIdAsync([FromUri] ActionLink model, bool absolute = true, CancellationToken cancellationToken = default(CancellationToken))
        {
            if (model == null || !ModelState.IsValid)
            {
                return BadRequest();
            }
            var action = await _projectManager.GetActionByIdAsync(model.ActionId, cancellationToken);
            if (action == null)
            {
                return NotFound();
            }
            await ApiSecurity.AuthorizeAsync(AccessObjectType.Project, action.Project.Id, AccessPermission.CanView, cancellationToken);

            return Ok(_actionLinkService.CreateLink(model, absolute));
        }

        /// <summary>
        /// Represents an action that handles only HTTP GET requests.
        /// </summary>
        [Route("~/api/project/actionlinks/{actionid}/link")]
        public async Task<IHttpActionResult> GetLinkByLinkIdAsync([FromUri] ActionLink model, bool absolute = true, CancellationToken cancellationToken = default(CancellationToken))
        {
            if (model == null || !ModelState.IsValid)
            {
                return BadRequest();
            }
            var action = await _projectManager.GetActionByLinkIdAsync(model.ActionId, cancellationToken);
            if (action == null)
            {
                return NotFound();
            }
            await ApiSecurity.AuthorizeAsync(AccessObjectType.Project, action.Project.Id, AccessPermission.CanView, cancellationToken);

            model.ActionId = action.Id;
            return Ok(_actionLinkService.CreateLink(model, absolute));
        }

        /// <summary>
        /// Represents an action that handles only HTTP POST requests.
        /// </summary>
        [Route("~/api/projects/{projectid}/actions")]
        public Task<IHttpActionResult> PostAsync(int projectId, [FromBody] ActionItemDto model, CancellationToken cancellationToken)
        {
            return PostAsync(projectId, null, model, cancellationToken);
        }

        /// <summary>
        /// Represents an action that handles only HTTP POST requests.
        /// </summary>
        [Route("~/api/projects/{projectid}/actions/{parentid}")]
        public async Task<IHttpActionResult> PostAsync(int projectId, int? parentId, [FromBody] ActionItemDto model, CancellationToken cancellationToken)
        {
            if (model == null || !ModelState.IsValid)
            {
                return BadRequest();
            }
            var project = await _projectManager.FindByIdAsync(projectId, cancellationToken);
            await ApiSecurity.AuthorizeAsync(project, AccessPermission.CanEdit, cancellationToken);

            var parentAction = default(ActionItem);
            if (parentId != null)
            {
                parentAction = await _projectManager.GetActionByIdAsync((int)parentId, cancellationToken);
                if (parentAction == null || parentAction.Project.Id != project.Id)
                {
                    return BadRequest();
                }
            }
            var action = new ActionItem
            {
                Type = model.Type,
                Enabled = model.Enabled,
                Name = model.Name,
                Options = model.Options
            };
            var validationResult = await _projectManager.AddActionAsync(project, parentAction, action, cancellationToken);
            if (!validationResult.Succeeded)
            {
                return this.ValidationContent(validationResult);
            }
            var actionResult = await _projectManager.GetActionByIdAsync(action.Id, cancellationToken);
            return new ActionContentResult(HttpStatusCode.Created, _actionLinkService, actionResult, this);
        }

        /// <summary>
        /// Represents an action that handles only HTTP GET requests.
        /// </summary>
        [Route("{id}"), AcceptVerbs("COPY")]
        public async Task<IHttpActionResult> CopyAsync(int id, [FromBody] ActionSortOrderDto model, CancellationToken cancellationToken)
        {
            var action = await _projectManager.GetActionByIdAsync(id, cancellationToken);
            if (action == null)
            {
                return NotFound();
            }
            var referenceAction = default(ActionItem);
            if (model != null)
            {
                referenceAction = await _projectManager.GetActionByIdAsync((int)model.ReferenceId, cancellationToken);
                if (referenceAction == null)
                {
                    return NotFound();
                }
            }
            var project = await _projectManager.FindByIdAsync(action.Project.Id, cancellationToken);
            await ApiSecurity.AuthorizeAsync(project, AccessPermission.CanEdit, cancellationToken);

            var validationResult = await _projectManager.CopyActionBeforeAsync(project, action, referenceAction, cancellationToken);
            if (!validationResult.Succeeded)
            {
                return this.ValidationContent(validationResult);
            }
            action = await _projectManager.GetActionByIdAsync(action.Id, cancellationToken);
            return new ActionContentResult(_actionLinkService, action, this);
        }

        /// <summary>
        /// Represents an action that handles only HTTP GET requests.
        /// </summary>
        [Route("{id}"), AcceptVerbs("MOVE")]
        public async Task<IHttpActionResult> MoveAsync(int id, [FromBody] ActionSortOrderDto model, CancellationToken cancellationToken)
        {
            var action = await _projectManager.GetActionByIdAsync(id, cancellationToken);
            if (action == null)
            {
                return NotFound();
            }
            var referenceAction = default(ActionItem);
            if (model != null)
            {
                referenceAction = await _projectManager.GetActionByIdAsync((int)model.ReferenceId, cancellationToken);
                if (referenceAction == null)
                {
                    return NotFound();
                }
            }
            var project = await _projectManager.FindByIdAsync(action.Project.Id, cancellationToken);
            await ApiSecurity.AuthorizeAsync(project, AccessPermission.CanEdit, cancellationToken);

            var validationResult = await _projectManager.MoveActionBeforeAsync(project, action, referenceAction, cancellationToken);
            return this.ValidationContent(validationResult);
        }

        /// <summary>
        /// Represents an action that handles only HTTP PUT requests.
        /// </summary>
        [Route("{id}")]
        public async Task<IHttpActionResult> PutAsync(int id, [FromBody] ActionItemDto model, CancellationToken cancellationToken)
        {
            if (model == null || !ModelState.IsValid)
            {
                return BadRequest();
            }
            var action = await _projectManager.GetActionByIdAsync(id, cancellationToken);
            if (action == null)
            {
                return NotFound();
            }
            var project = await _projectManager.FindByIdAsync(action.Project.Id, cancellationToken);
            await ApiSecurity.AuthorizeAsync(project, AccessPermission.CanEdit, cancellationToken);

            action.Type = model.Type;
            action.Enabled = model.Enabled;
            action.Name = model.Name;
            action.Options = model.Options;
            action.ModifiedDate = DateTime.UtcNow;

            var validationResult = await _projectManager.ReplaceActionAsync(project, action, action, cancellationToken);
            if (!validationResult.Succeeded)
            {
                return this.ValidationContent(validationResult);
            }
            return new ActionContentResult(_actionLinkService, action, this);
        }

        /// <summary>
        /// Represents an action that handles only HTTP DELETE requests.
        /// </summary>
        [Route("{id}")]
        public async Task<IHttpActionResult> DeleteAsync(int id, CancellationToken cancellationToken)
        {
            var action = await _projectManager.GetActionByIdAsync(id, cancellationToken);
            if (action == null)
            {
                return NotFound();
            }
            var project = await _projectManager.FindByIdAsync(action.Project.Id, cancellationToken);
            await ApiSecurity.AuthorizeAsync(project, AccessPermission.CanEdit, cancellationToken);

            var validationResult = await _projectManager.RemoveActionAsync(project, action, cancellationToken);
            if (!validationResult.Succeeded)
            {
                return this.ValidationContent(validationResult);
            }
            return StatusCode(HttpStatusCode.NoContent);
        }
    }
}