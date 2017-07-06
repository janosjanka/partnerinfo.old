// Copyright (c) János Janka. All rights reserved.

using System.Net;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Routing;
using Partnerinfo.Project.Mail;
using Partnerinfo.Project.Models;
using Partnerinfo.Security;

namespace Partnerinfo.Project.Controllers
{
    [Authorize]
    [RoutePrefix("api/project/mails")]
    public sealed class MailMessagesController : ApiController
    {
        private readonly ProjectManager _projectManager;
        private readonly IMailClientService _mailService;

        /// <summary>
        /// Initializes a new instance of the <see cref="MailMessagesController" /> class.
        /// </summary>
        public MailMessagesController(ProjectManager projectManager, IMailClientService mailService)
        {
            _projectManager = projectManager;
            _mailService = mailService;
        }

        /// <summary>
        /// Represents an action that handles only HTTP GET requests.
        /// </summary>
        [Route("~/api/projects/{projectid}/mails")]
        public async Task<IHttpActionResult> GetAllAsync([FromUri] MailMessageQueryDto model, CancellationToken cancellationToken)
        {
            if (model == null || !ModelState.IsValid)
            {
                return BadRequest();
            }
            var project = await _projectManager.FindByIdAsync(model.ProjectId, cancellationToken);
            await ApiSecurity.AuthorizeAsync(project, AccessPermission.CanView, cancellationToken);

            return Ok(await _projectManager.GetMailMessagesAsync(
                project,
                model.Subject,
                model.OrderBy,
                model.Page,
                model.Limit,
                model.Fields,
                cancellationToken));
        }

        /// <summary>
        /// Represents an action that handles only HTTP GET requests.
        /// </summary>
        [Route("{id}")]
        public Task<IHttpActionResult> GetByIdAsync(int id, CancellationToken cancellationToken)
        {
            return GetByIdAsync(id, MailMessageField.None, cancellationToken);
        }

        /// <summary>
        /// Represents an action that handles only HTTP GET requests.
        /// </summary>
        [Route("{id}", Name = "Project.Mails.GetById")]
        public async Task<IHttpActionResult> GetByIdAsync(int id, MailMessageField fields, CancellationToken cancellationToken)
        {
            var mailMessage = await _projectManager.GetMailMessageByIdAsync(id, fields | MailMessageField.Project, cancellationToken);
            if (mailMessage == null)
            {
                return NotFound();
            }
            await ApiSecurity.AuthorizeAsync(AccessObjectType.Project, mailMessage.Project.Id, AccessPermission.CanView, cancellationToken);
            return Ok(mailMessage);
        }

        /// <summary>
        /// Represents an action that handles only HTTP POST requests.
        /// </summary>
        [Route("~/api/projects/{projectid}/mails")]
        public async Task<IHttpActionResult> PostAsync(int projectId, [FromBody] MailMessageItemDto model, CancellationToken cancellationToken)
        {
            if (model == null || !ModelState.IsValid)
            {
                return BadRequest();
            }
            var project = await _projectManager.FindByIdAsync(projectId, cancellationToken);
            await ApiSecurity.AuthorizeAsync(project, AccessPermission.CanEdit, cancellationToken);

            var mailMessage = new MailMessageItem { Subject = model.Subject, Body = model.Body };
            var validationResult = await _projectManager.AddMailMessageAsync(project, mailMessage, cancellationToken);
            if (!validationResult.Succeeded)
            {
                return this.ValidationContent(validationResult);
            }
            return CreatedAtRoute("Project.Mails.GetById", new RouteValueDictionary { { "id", mailMessage.Id } }, mailMessage);
        }

        /// <summary>
        /// Represents an action that handles only HTTP PUT requests.
        /// </summary>
        [Route("{id}")]
        public async Task<IHttpActionResult> PutAsync(int id, [FromBody] MailMessageItemDto model, CancellationToken cancellationToken)
        {
            if (model == null || !ModelState.IsValid)
            {
                return BadRequest();
            }
            var mailMessage = await _projectManager.GetMailMessageByIdAsync(id, MailMessageField.Project, cancellationToken);
            if (mailMessage == null)
            {
                return NotFound();
            }
            var project = await _projectManager.FindByIdAsync(mailMessage.Project.Id, cancellationToken);
            await ApiSecurity.AuthorizeAsync(project, AccessPermission.CanEdit, cancellationToken);

            mailMessage.Subject = model.Subject;
            mailMessage.Body = model.Body;
            var validationResult = await _projectManager.ReplaceMailMessageAsync(project, mailMessage, mailMessage, cancellationToken);
            if (!validationResult.Succeeded)
            {
                return this.ValidationContent(validationResult);
            }
            return Ok(mailMessage);
        }

        /// <summary>
        /// Represents an action that handles only HTTP POST requests.
        /// </summary>
        [Route("{id}/send"), HttpPost]
        public async Task<IHttpActionResult> SendAsync(int id, [FromBody] MailMessageHeaderDto model, CancellationToken cancellationToken)
        {
            if (model == null || !ModelState.IsValid)
            {
                return BadRequest();
            }
            var mailMessage = await _projectManager.GetMailMessageByIdAsync(id, MailMessageField.Project | MailMessageField.Body, cancellationToken);
            if (mailMessage == null)
            {
                return NotFound();
            }
            var project = await _projectManager.FindByIdAsync(mailMessage.Project.Id, cancellationToken);
            await ApiSecurity.AuthorizeAsync(project, AccessPermission.CanManage, cancellationToken);

            var validationResult = await _mailService.SendAsync(project, model.ToMailMessageHeader(), mailMessage, cancellationToken);
            return this.ValidationContent(validationResult);
        }

        /// <summary>
        /// Represents an action that handles only HTTP DELETE requests.
        /// </summary>
        [Route("{id}")]
        public async Task<IHttpActionResult> DeleteAsync(int id, CancellationToken cancellationToken)
        {
            var mailMessage = await _projectManager.GetMailMessageByIdAsync(id, MailMessageField.Project, cancellationToken);
            if (mailMessage == null)
            {
                return NotFound();
            }
            var project = await _projectManager.FindByIdAsync(mailMessage.Project.Id, cancellationToken);
            await ApiSecurity.AuthorizeAsync(project, AccessPermission.CanEdit, cancellationToken);

            await _projectManager.RemoveMailMessageAsync(project, mailMessage, cancellationToken);
            return StatusCode(HttpStatusCode.NoContent);
        }
    }
}