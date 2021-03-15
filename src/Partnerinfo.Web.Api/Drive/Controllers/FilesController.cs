// Copyright (c) János Janka. All rights reserved.

using System.Collections.Generic;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Routing;
using Partnerinfo.Drive.EntityFramework;
using Partnerinfo.Drive.Results;

namespace Partnerinfo.Drive.Controllers
{
    /// <summary>
    /// Provides methods that respond to HTTP requests that are made to an ASP.NET Web API.
    /// </summary>
    [Authorize]
    [RoutePrefix("api/drive/files")]
    public class FilesController : ApiController
    {
        private const string GetByIdRouteName = "Drive.Files.GetById";

        private readonly IPersistenceServices _services;

        /// <summary>
        /// Initializes a new instance of the <see cref="FilesController" /> class.
        /// </summary>
        /// <param name="services">An object that defines application-level services.</param>
        public FilesController(IPersistenceServices services)
        {
            _services = services;
        }

        /// <summary>
        /// Represents an action that handles only HTTP GET requests.
        /// </summary>
        [Route("")]
        public async Task<IHttpActionResult> GetAllAsync([FromUri] Models.FileListRequest model, CancellationToken cancellationToken)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest();
            }

            // Web API 2.0 attribute routing doesn't instatiate the model calling its parameterless constructor,
            // but it uses string expressions specified on the route attribute instead. I don't suggest to harcode
            // complex type initializers as string expressions.
            if (model == null)
            {
                model = new Models.FileListRequest();
            }

            return new FileListResult(await _services.Drive.FindAllAsync(
                ApiSecurity.CurrentUserId,
                model.ParentId,
                model.Name,
                model.Page,
                model.Count,
                cancellationToken),
                this);
        }

        /// <summary>
        /// Represents an action that handles only HTTP GET requests.
        /// </summary>
        [Route("{id:int}", Name = GetByIdRouteName)]
        public async Task<IHttpActionResult> GetByIdAsync(int id, CancellationToken cancellationToken)
        {
            //SecurityHelper.Authorize(Services.User, AccessSource.File, id);
            var file = await _services.Drive.FindByIdAsync(id, cancellationToken);
            if (file == null)
            {
                return NotFound();
            }

            return Ok(file);
        }

        /// <summary>
        /// Represents an action that handles only HTTP GET requests.
        /// </summary>
        [Route("{slug}")]
        public async Task<IHttpActionResult> GetBySlugAsync(string slug, CancellationToken cancellationToken)
        {
            var file = await _services.Drive.FindByUriAsync(slug, cancellationToken);
            if (file == null)
            {
                return NotFound();
            }

            //SecurityHelper.Authorize(Services.User, AccessSource.File, file.Id);
            return Ok(file);
        }

        /// <summary>
        /// Represents an action that handles only HTTP POST requests.
        /// </summary>
        [Route("")]
        public async Task<IHttpActionResult> PostAsync([FromBody] FileItem model, CancellationToken cancellationToken)
        {
            ModelState.Remove("model.Slug");
            if (model == null || !ModelState.IsValid)
            {
                return BadRequest();
            }

            var manager = new DriveManager(ApiSecurity.Manager, _services, ServerPaths.Map(ServerPaths.DriveFiles));
            model = await manager.CreateAsync(model, ApiSecurity.CurrentUserId, cancellationToken);
            return CreatedAtRoute(GetByIdRouteName, new RouteValueDictionary { { "id", model.Id } }, model);
        }

        /// <summary>
        /// Represents an action that handles only HTTP POST requests.
        /// </summary>
        [Route("{id}/name"), HttpPut]
        public async Task<IHttpActionResult> SetNameAsync(int id, [FromBody] string name, CancellationToken cancellationToken)
        {
            if (string.IsNullOrEmpty(name))
            {
                return BadRequest();
            }

            //SecurityHelper.Authorize(Services.User, AccessSource.File, id, AccessPermission.CanEdit);
            await _services.Drive.SetNameAsync(id, name, cancellationToken);
            await _services.SaveAsync(cancellationToken);
            return Ok();
        }

        /// <summary>
        /// Represents an action that handles only HTTP DELETE requests.
        /// </summary>
        [Route("{id}")]
        public async Task<IHttpActionResult> DeleteAsync(int id, CancellationToken cancellationToken)
        {
            //SecurityHelper.Authorize(Services.User, AccessSource.File, id, AccessPermission.IsOwner);
            await (new DriveManager(ApiSecurity.Manager, _services, ServerPaths.Map(ServerPaths.DriveFiles)).DeleteAsync(id, cancellationToken));
            return StatusCode(HttpStatusCode.NoContent);
        }

        /// <summary>
        /// Represents an action that handles only HTTP DELETE requests.
        /// </summary>
        /// <returns></returns>
        [Route("")]
        public async Task<IHttpActionResult> DeleteAllAsync([FromUri] IEnumerable<int> ids, CancellationToken cancellationToken)
        {
            if (ids != null)
            {
                await (new DriveManager(ApiSecurity.Manager, _services, ServerPaths.Map(ServerPaths.DriveFiles)).DeleteAsync(ids, cancellationToken));
            }
            return StatusCode(HttpStatusCode.NoContent);
        }
    }
}