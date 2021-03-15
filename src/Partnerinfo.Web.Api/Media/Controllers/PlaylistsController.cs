// Copyright (c) János Janka. All rights reserved.

using System;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;
using System.Web.Routing;
using Partnerinfo.Filters;
using Partnerinfo.Media.EntityFramework;
using Partnerinfo.Media.Models;

namespace Partnerinfo.Media.Controllers
{
    /// <summary>
    /// Provides methods that respond to HTTP requests that are made to an ASP.NET Web API.
    /// </summary>
    [RoutePrefix("api/project/playlists")]
    public sealed class PlaylistsController : ApiController
    {
        private readonly IPersistenceServices _services;

        /// <summary>
        /// Initializes a new instance of the <see cref="PlaylistsController" /> class.
        /// </summary>
        /// <returns></returns>
        public PlaylistsController(IPersistenceServices services)
        {
            _services = services;
        }

        /// <summary>
        /// Authentication ticket which identifies a contact
        /// </summary>
        public AuthTicket ProjectIdentity
        {
            get
            {
                object ticket;
                return Request.Properties.TryGetValue(AuthorizeProjectAttribute.IdentityKey, out ticket) ? (AuthTicket)ticket : null;
            }
        }

        /// <summary>
        /// Represents an action that handles only HTTP GET requests.
        /// </summary>
        [Route("default/{contactId:int}")]
        public async Task<IHttpActionResult> GetDefaultByContactAsync(int contactId, CancellationToken cancellationToken)
        {
            var playlist = await _services.Media.FindDefaultByContactAsync(contactId, cancellationToken);
            if (playlist == null)
            {
                return NotFound();
            }
            return Ok(playlist);
        }

        /// <summary>
        /// Represents an action that handles only HTTP GET requests.
        /// </summary>
        [Route("{id:int}", Name = "Media.Playlists.GetById")]
        public async Task<IHttpActionResult> GetByIdAsync(int id, CancellationToken cancellationToken)
        {
            var playlist = await _services.Media.FindByIdAsync(id, cancellationToken);
            if (playlist == null)
            {
                return NotFound();
            }
            return Ok(playlist);
        }

        /// <summary>
        /// Represents an action that handles only HTTP GET requests.
        /// </summary>
        [Route("{uri}")]
        public async Task<IHttpActionResult> GetByUriAsync(string uri, CancellationToken cancellationToken)
        {
            var playlist = await _services.Media.FindByUriAsync(uri, cancellationToken);
            if (playlist == null)
            {
                return NotFound();
            }
            return Ok(playlist);
        }

        /// <summary>
        /// Represents an action that handles only HTTP GET requests.
        /// </summary>
        [Route("")]
        [AuthorizeProject]
        public async Task<IHttpActionResult> GetAllAsync([FromUri] PaylistsQueryDto model, CancellationToken cancellationToken)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest();
            }
            if (model == null)
            {
                model = new PaylistsQueryDto();
            }
            return Ok(await _services.Media.FindAllAsync(
                ProjectIdentity.Id,
                model.Name,
                model.Page,
                model.Count,
                cancellationToken));
        }

        /// <summary>
        /// Represents an action that handles only HTTP POST requests.
        /// </summary>
        [Route("{id}/default")]
        [AuthorizeProject]
        public async Task<IHttpActionResult> PostDefaultAsync(int id, CancellationToken cancellationToken)
        {
            var playlist = await _services.Media.FindByIdAsync(id, cancellationToken);
            if (playlist == null)
            {
                return NotFound();
            }
            await _services.Media.SetDefaultAsync(playlist, cancellationToken);
            await _services.SaveAsync(cancellationToken);
            return Ok();
        }

        /// <summary>
        /// Represents an action that handles only HTTP POST requests.
        /// </summary>
        [Route("")]
        [AuthorizeProject]
        public async Task<IHttpActionResult> PostAsync([FromBody] MediaPlaylist model, CancellationToken cancellationToken)
        {
            if (model == null)
            {
                return BadRequest();
            }
            if (!ModelState.IsValid)
            {
                ModelState.Remove("model.ContactId");
                ModelState.Remove("model.Uri");
            }
            if (!ModelState.IsValid)
            {
                return BadRequest();
            }

            var uri = string.Join("-", ProjectIdentity.Id.ToString("x"), UriUtility.Normalize(model.Name));
            var oldPlaylist = await _services.Media.FindByUriAsync(uri, cancellationToken);
            if (oldPlaylist != null)
            {
                uri = string.Join("-", ProjectIdentity.Id.ToString("x"), Guid.NewGuid().ToString("n"));
            }
            model = new MediaPlaylist
            {
                ContactId = ProjectIdentity.Id,
                Uri = uri,
                Name = model.Name,
                EditMode = model.EditMode,
                DefaultList = model.DefaultList,
                Published = true
            };
            await _services.Media.CreateAsync(model, cancellationToken);
            await _services.SaveAsync(cancellationToken);
            return CreatedAtRoute("Media.Playlists.GetById", new RouteValueDictionary { { "id", model.Id } }, model);
        }

        /// <summary>
        /// Represents an action that handles only HTTP POST requests.
        /// </summary>
        [Route("{id}")]
        [AuthorizeProject]
        public async Task<IHttpActionResult> PutAsync(int id, [FromBody] MediaPlaylist model, CancellationToken cancellationToken)
        {
            if (model == null || !ModelState.IsValid)
            {
                return BadRequest();
            }
            var playlist = await _services.Media.FindByIdAsync(id, cancellationToken);
            if (playlist == null)
            {
                return NotFound();
            }

            playlist.Name = model.Name;
            playlist.DefaultList = model.DefaultList;
            playlist.EditMode = model.EditMode;

            await _services.Media.UpdateAsync(playlist, cancellationToken);
            await _services.SaveAsync(cancellationToken);

            return Ok(model);
        }

        /// <summary>
        /// Represents an action that handles only HTTP DELETE requests.
        /// </summary>
        [Route("{id}")]
        [AuthorizeProject]
        public async Task<IHttpActionResult> DeleteAsync(int id, CancellationToken cancellationToken)
        {
            var playlist = await _services.Media.FindByIdAsync(id, cancellationToken);
            if (playlist != null || playlist.ContactId == ProjectIdentity.Id)
            {
                await _services.Media.DeleteAsync(id, cancellationToken);
                await _services.SaveAsync(cancellationToken);
            }
            return StatusCode(HttpStatusCode.NoContent);
        }
    }
}