// Copyright (c) János Janka. All rights reserved.

using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Routing;
using Partnerinfo.Project.Models;
using Partnerinfo.Security;

namespace Partnerinfo.Project.Controllers
{
    [Authorize]
    [RoutePrefix("api/projects/{projectid}/contacts")]
    public sealed class ContactsController : ApiController
    {
        private readonly ProjectManager _projectManager;

        /// <summary>
        /// Initializes a new instance of the <see cref="ContactsController" /> class.
        /// </summary>
        /// <param name="projectManager">The project manager that the <see cref="ContactsController" /> operates against.</param>
        public ContactsController(ProjectManager projectManager)
        {
            _projectManager = projectManager;
        }

        /// <summary>
        /// Represents an action that handles only HTTP GET requests.
        /// </summary>
        [Route("")]
        public async Task<IHttpActionResult> GetAllAsync([FromUri] ContactQueryDto model, CancellationToken cancellationToken)
        {
            if (model == null || !ModelState.IsValid)
            {
                return BadRequest();
            }
            var project = await _projectManager.FindByIdAsync(model.ProjectId, cancellationToken);
            await ApiSecurity.AuthorizeAsync(project, AccessPermission.CanView, cancellationToken);
            return Ok(await _projectManager.GetContactsAsync(
                 project,
                 model.Name,
                 model.IncludeWithTags,
                 model.ExcludeWithTags,
                 model.OrderBy,
                 model.Page,
                 model.Limit,
                 model.Fields,
                 cancellationToken));
        }

        /// <summary>
        /// Represents an action that handles only HTTP GET requests.
        /// </summary>
        [Route("~/api/project/contacts/{id}")]
        public Task<IHttpActionResult> GetById(int id, CancellationToken cancellationToken)
        {
            return GetById(id, ContactField.None, cancellationToken);
        }

        /// <summary>
        /// Represents an action that handles only HTTP GET requests.
        /// </summary>
        [Route("~/api/project/contacts/{id}", Name = "Project.Contacts.GetById")]
        public async Task<IHttpActionResult> GetById(int id, ContactField fields, CancellationToken cancellationToken)
        {
            var contact = await _projectManager.GetContactByIdAsync(id, fields | ContactField.Project, cancellationToken);
            if (contact == null)
            {
                return NotFound();
            }

            await ApiSecurity.AuthorizeAsync(AccessObjectType.Project, contact.Project.Id, AccessPermission.CanView, cancellationToken);
            return Ok(contact);
        }

        /// <summary>
        /// Represents an action that handles only HTTP POST requests.
        /// </summary>
        [Route("")]
        public async Task<IHttpActionResult> PostAsync(int projectId, [FromBody] ContactItemDto model, CancellationToken cancellationToken)
        {
            if (model == null || !ModelState.IsValid)
            {
                return BadRequest();
            }
            var project = await _projectManager.FindByIdAsync(projectId, cancellationToken);
            await ApiSecurity.AuthorizeAsync(project, AccessPermission.CanEdit, cancellationToken);

            var sponsor = default(ContactItem);
            if (model.SponsorId != null)
            {
                sponsor = await _projectManager.GetContactByIdAsync((int)model.SponsorId, cancellationToken);
            }
            var contact = new ContactItem
            {
                Sponsor = sponsor,
                FacebookId = model.FacebookId,
                Email = model.Email,
                FirstName = model.FirstName,
                LastName = model.LastName,
                NickName = model.NickName,
                Gender = model.Gender,
                Birthday = model.Birthday,
                Phones = model.Phones,
                Comment = model.Comment
            };
            var validationResult = await _projectManager.AddContactAsync(project, contact, cancellationToken);
            if (!validationResult.Succeeded)
            {
                return this.ValidationContent(validationResult);
            }
            return CreatedAtRoute("Project.Contacts.GetById", new RouteValueDictionary { { "id", contact.Id } }, contact);
        }

        /// <summary>
        /// Represents an action that handles only HTTP POST requests.
        /// </summary>
        [Route("upload")]
        public async Task<IHttpActionResult> PostFileAsync(int projectId, CancellationToken cancellationToken)
        {
            var project = await _projectManager.FindByIdAsync(projectId, cancellationToken);
            await ApiSecurity.AuthorizeAsync(project, AccessPermission.CanEdit, cancellationToken);

            using (var content = await Request.Content.ReadAsFileStreamAsync(cancellationToken))
            {
                var contactManager = new ContactManager(_projectManager);
                switch (content.FileExtension.ToLower())
                {
                    case ".xls":
                        await contactManager.ImportFromXlsAsync(project, content.FileStream, cancellationToken);
                        break;
                    case ".xlsx":
                        await contactManager.ImportFromXlsxAsync(project, content.FileStream, cancellationToken);
                        break;
                    default:
                        return StatusCode(HttpStatusCode.UnsupportedMediaType);
                }
            }
            return Ok();
        }

        /// <summary>
        /// Represents an action that handles only HTTP PUT requests.
        /// </summary>
        [Route("~/api/project/contacts/{id}")]
        public async Task<IHttpActionResult> PutAsync(int id, [FromBody] ContactItemDto model, CancellationToken cancellationToken)
        {
            if (model == null || !ModelState.IsValid)
            {
                return BadRequest();
            }
            var contact = await _projectManager.GetContactByIdAsync(id, ContactField.Project, cancellationToken);
            if (contact == null)
            {
                return NotFound();
            }
            var project = await _projectManager.FindByIdAsync(contact.Project.Id, cancellationToken);
            await ApiSecurity.AuthorizeAsync(project, AccessPermission.CanEdit, cancellationToken);

            var sponsor = default(ContactItem);
            if (model.SponsorId != null)
            {
                sponsor = await _projectManager.GetContactByIdAsync((int)model.SponsorId, cancellationToken);
            }

            contact.Sponsor = sponsor;
            contact.FacebookId = model.FacebookId;
            contact.Email = model.Email;
            contact.FirstName = model.FirstName;
            contact.LastName = model.LastName;
            contact.NickName = model.NickName;
            contact.Gender = model.Gender;
            contact.Birthday = model.Birthday;
            contact.Phones = model.Phones;
            contact.Comment = model.Comment;

            await _projectManager.ReplaceContactAsync(project, contact, contact, cancellationToken);
            return Ok(contact);
        }

        /// <summary>
        /// Represents an action that handles only HTTP DELETE requests.
        /// </summary>
        [Route("")]
        public async Task<IHttpActionResult> DeleteAllAsync(int projectId, [FromBody] IEnumerable<int> model, CancellationToken cancellationToken)
        {
            var project = await _projectManager.FindByIdAsync(projectId, cancellationToken);
            await ApiSecurity.AuthorizeAsync(project, AccessPermission.CanEdit, cancellationToken);

            var contacts = await _projectManager.GetContactsAsync(project, model, ContactSortOrder.None, ContactField.None, cancellationToken);
            await _projectManager.RemoveContactsAsync(project, contacts, cancellationToken);
            return StatusCode(HttpStatusCode.NoContent);
        }

        /// <summary>
        /// Represents an action that handles only HTTP DELETE requests.
        /// </summary>
        [Route("~/api/project/contacts/{id}")]
        public async Task<IHttpActionResult> DeleteAsync(int id, CancellationToken cancellationToken)
        {
            var contact = await _projectManager.GetContactByIdAsync(id, ContactField.Project, cancellationToken);
            if (contact != null)
            {
                var project = await _projectManager.FindByIdAsync(contact.Project.Id, cancellationToken);
                await ApiSecurity.AuthorizeAsync(project, AccessPermission.CanEdit, cancellationToken);
                await _projectManager.RemoveContactAsync(project, contact, cancellationToken);
            }
            return StatusCode(HttpStatusCode.NoContent);
        }

        /// <summary>
        /// Represents an action that handles only HTTP POST requests.
        /// </summary>
        [Route("~/api/project/contacts/business-tags"), HttpPost]
        public async Task<IHttpActionResult> SetTagsAsync([FromBody] ContactTagsDto model, CancellationToken cancellationToken)
        {
            if (model == null || !ModelState.IsValid)
            {
                return BadRequest();
            }
            await _projectManager.SetBusinessTagsAsync(model.Ids, model.TagsToAdd ?? Enumerable.Empty<int>(), model.TagsToRemove ?? Enumerable.Empty<int>(), cancellationToken);
            return Ok();
        }
    }
}