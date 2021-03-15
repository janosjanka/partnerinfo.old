// Copyright (c) János Janka. All rights reserved.

using System;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Mime;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Http;
using Partnerinfo.Identity;
using Partnerinfo.Logging.Models;
using Partnerinfo.Project;

namespace Partnerinfo.Logging.Controllers
{
    /// <summary>
    /// Provides methods that respond to HTTP requests that are made to an ASP.NET Web API.
    /// </summary>
    [Authorize]
    [RoutePrefix("api/logging/events")]
    public sealed class EventsController : ApiController
    {
        private readonly UserManager _userManager;
        private readonly ProjectManager _projectManager;
        private readonly EventManager _eventManager;
        private readonly CategoryManager _categoryManager;

        /// <summary>
        /// Initializes a new instance of the <see cref="EventsController" /> class.
        /// </summary>
        public EventsController(UserManager userManager, ProjectManager projectManager, EventManager eventManager, CategoryManager categoryManager)
        {
            _userManager = userManager;
            _projectManager = projectManager;
            _eventManager = eventManager;
            _categoryManager = categoryManager;
        }

        /// <summary>
        /// Represents an action that handles only HTTP GET requests.
        /// </summary>
        [Route("")]
        public async Task<IHttpActionResult> GetAllAsync([FromUri] EventCompQueryDto model, CancellationToken cancellationToken)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest();
            }
            var user = await _userManager.FindByIdAsync(ApiSecurity.CurrentUserId, cancellationToken);
            if (user == null)
            {
                return NotFound();
            }
            if (model == null)
            {
                model = new EventCompQueryDto();
            }
            var result = new EventCompResultDto();
            if (model.Events == null)
            {
                model.Events = new EventQueryDto();
            }
            result.Events = await _eventManager.FindAllAsync(
                user.Id,
                model.Events.CategoryId,
                model.Events.DateFrom,
                model.Events.DateTo,
                model.Events.ObjectType,
                model.Events.ObjectId,
                model.Events.ContactId,
                model.Events.ContactState,
                model.Events.ProjectId,
                model.Events.CustomUri,
                model.Events.Emails,
                model.Events.Clients,
                model.Events.Page,
                model.Events.Limit,
                cancellationToken);
            if (model.Fields.HasFlag(EventCompQueryField.Categories))
            {
                if (model.Categories == null)
                {
                    model.Categories = new CategoryQueryDto();
                }
                var project = default(ProjectItem);
                if (model.Categories.ProjectId != null)
                {
                    project = await _projectManager.FindByIdAsync((int)model.Categories.ProjectId, cancellationToken);
                    if (project == null)
                    {
                        return NotFound();
                    }
                }
                result.Categories = await _categoryManager.FindAllAsync(user, project, model.Categories.OrderBy, model.Categories.Fields, cancellationToken);
            }
            return Ok(result);
        }

        /// <summary>
        /// Represents an action that handles only HTTP GET requests.
        /// </summary>
        [Route("download")]
        public async Task<HttpResponseMessage> GetAllAsExcelAsync([FromUri] EventQueryDto model, CancellationToken cancellationToken)
        {
            if (!ModelState.IsValid)
            {
                return Request.CreateResponse(HttpStatusCode.BadRequest);
            }
            if (model == null)
            {
                model = new Models.EventQueryDto();
            }
            var stream = new MemoryStream();
            await _eventManager.ReportGenerator.GenerateAsync(stream, (await _eventManager.FindAllAsync(
                ApiSecurity.CurrentUserId,
                model.CategoryId,
                model.DateFrom,
                model.DateTo,
                model.ObjectType,
                model.ObjectId,
                model.ContactId,
                model.ContactState,
                model.ProjectId,
                model.CustomUri,
                model.Emails,
                model.Clients,
                1, 100000,
                cancellationToken)).Data,
                cancellationToken);
            stream.Position = 0;
            var response = new HttpResponseMessage(HttpStatusCode.OK);
            response.Headers.CacheControl = new CacheControlHeaderValue { Private = true };
            response.Content = new StreamContent(stream);
            response.Content.Headers.Expires = DateTime.UtcNow.AddMinutes(1);
            response.Content.Headers.ContentType = new MediaTypeHeaderValue(MediaTypes.Excel2007);
            response.Content.Headers.ContentLength = stream.Length;
            response.Content.Headers.ContentDisposition = new ContentDispositionHeaderValue(DispositionTypeNames.Attachment)
            {
                FileName = string.Format("events-{0}.xlsx", DateTime.UtcNow.ToString("yyyyMMdd-HHmmss")),
                Size = stream.Length
            };
            return response;
        }

        /// <summary>
        /// Represents an action that handles only HTTP GET requests.
        /// </summary>
        [Route("unread/count")]
        public async Task<IHttpActionResult> GetNumberOfUnreadEventsAsync(CancellationToken cancellationToken)
        {
            return Ok(await _eventManager.GetUnreadCountAsync(ApiSecurity.CurrentUserId, cancellationToken));
        }

        /// <summary>
        /// Represents an action that handles only HTTP POST requests.
        /// </summary>
        [Route("signal/read"), AllowAnonymous]
        public async Task<IHttpActionResult> PostSignalReadAsync(CancellationToken cancellationToken)
        {
            //ApiApiSecurity.Require(Services.Database.User, Services.Database.User, );
            await _eventManager.SetLastReadDateAsync(ApiSecurity.CurrentUserId, DateTime.UtcNow, cancellationToken);
            return Ok();
        }

        /// <summary>
        /// Represents an action that handles only HTTP POST requests.
        /// </summary>
        [Route("signal/finish"), AllowAnonymous]
        public async Task<IHttpActionResult> PostSignalFinishAsync([FromBody] int id, CancellationToken cancellationToken)
        {
            await _eventManager.SetEventFinishDateAsync(id, DateTime.UtcNow, cancellationToken);
            return Ok();
        }

        //
        // Batch Operations
        //

        /// <summary>
        /// Represents an action that handles only HTTP POST requests.
        /// </summary>
        [Route("category"), HttpPost]
        public async Task<IHttpActionResult> SetAllCategoriesAsync([FromBody] EventCategoryBulkUpdateDto model, CancellationToken cancellationToken)
        {
            if (model == null || !ModelState.IsValid)
            {
                return BadRequest();
            }
            if (model.Ids != null)
            {
                await _eventManager.BulkSetCategoriesAsync(ApiSecurity.CurrentUserId, model.Ids, model.CategoryId, cancellationToken);
            }
            if (model.Filter != null)
            {
                var ids = await _eventManager.GetIdsAsync(
                    ApiSecurity.CurrentUserId,
                    model.Filter.CategoryId,
                    model.Filter.DateFrom,
                    model.Filter.DateTo,
                    model.Filter.ObjectType,
                    model.Filter.ObjectId,
                    model.Filter.ContactId,
                    model.Filter.ContactState,
                    model.Filter.ProjectId,
                    model.Filter.CustomUri,
                    model.Filter.Emails,
                    model.Filter.Clients,
                    cancellationToken);
                if (ids.Count > 0)
                {
                    await _eventManager.BulkSetCategoriesAsync(ApiSecurity.CurrentUserId, ids, model.CategoryId, cancellationToken);
                }
            }
            return Ok();
        }

        /// <summary>
        /// Represents an action that handles only HTTP DELETE requests.
        /// </summary>
        [Route("")]
        public async Task<IHttpActionResult> DeleteAllAsync([FromBody] EventBulkUpdateDto model, CancellationToken cancellationToken)
        {
            if (model == null || !ModelState.IsValid)
            {
                return BadRequest();
            }
            if (model.Ids != null)
            {
                await _eventManager.BulkDeleteAsync(ApiSecurity.CurrentUserId, model.Ids, cancellationToken);
            }
            if (model.Filter != null)
            {
                var ids = await _eventManager.GetIdsAsync(
                    ApiSecurity.CurrentUserId,
                    model.Filter.CategoryId,
                    model.Filter.DateFrom,
                    model.Filter.DateTo,
                    model.Filter.ObjectType,
                    model.Filter.ObjectId,
                    model.Filter.ContactId,
                    model.Filter.ContactState,
                    model.Filter.ProjectId,
                    model.Filter.CustomUri,
                    model.Filter.Emails,
                    model.Filter.Clients,
                    cancellationToken);
                if (ids.Count > 0)
                {
                    await _eventManager.BulkDeleteAsync(ApiSecurity.CurrentUserId, ids, cancellationToken);
                }
            }
            return Ok();
        }
    }
}