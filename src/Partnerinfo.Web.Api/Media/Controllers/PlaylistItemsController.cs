// Copyright (c) János Janka. All rights reserved.

using System.Collections.Generic;
using System.Net;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
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
    [RoutePrefix("api/project/playlist-items")]
    public sealed class PlaylistItemsController : ApiController
    {
        private readonly IPersistenceServices _services;

        /// <summary>
        /// Initializes a new instance of the <see cref="PlaylistItemsController" /> class.
        /// </summary>
        /// <returns></returns>
        public PlaylistItemsController(IPersistenceServices services)
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
        [AllowAnonymous]
        [Route("~/api/project/playlists/{playlistId:int}/items")]
        public async Task<IHttpActionResult> GetAllByPlaylistAsync([FromUri] ItemsByPlaylistQueryDto model, CancellationToken cancellationToken)
        {
            if (model == null || !ModelState.IsValid)
            {
                return BadRequest();
            }

            return Ok(await _services.Media.GetItemsByPlaylistAsync(
                model.PlaylistId,
                model.Name,
                model.Page,
                model.Count,
                cancellationToken));
        }

        /// <summary>
        /// Represents an action that handles only HTTP GET requests.
        /// </summary>
        [AllowAnonymous]
        [Route("~/api/project/contacts/{contactId:int}/playlists/items")]
        public async Task<IHttpActionResult> GetAllByContactAsync([FromUri] ItemsByContactQueryDto model, CancellationToken cancellationToken)
        {
            if (model == null || !ModelState.IsValid)
            {
                return BadRequest();
            }

            return Ok(await _services.Media.GetItemsByContactAsync(
                model.ContactId,
                model.Name,
                model.Page,
                model.Count,
                cancellationToken));
        }

        /// <summary>
        /// Represents an action that handles only HTTP POST requests.
        /// </summary>
        [Route("{playlistId:int}")]
        [AuthorizeProject]
        public async Task<HttpResponseMessage> PostAsync(int playlistId, [FromBody] PlaylistItemDto model, CancellationToken cancellationToken)
        {
            var playlist = await _services.Media.FindByIdAsync(playlistId, cancellationToken);
            if (playlist == null)
            {
                return Request.CreateFaultResponse(HttpStatusCode.NotFound, "Playlist was not found.");
            }
            if (ProjectIdentity == null || ProjectIdentity.Id != playlist.ContactId)
            {
                return Request.CreateFaultResponse(HttpStatusCode.Forbidden, "Playlist has already been taken.");
            }
            if (ModelState.IsValid)
            {
                var entity = new MediaPlaylistItem
                {
                    PlaylistId = playlistId,
                    Name = model.Name,
                    MediaType = model.MediaType,
                    MediaId = model.MediaId,
                    Duration = model.Duration,
                    PublishDate = model.PublishDate
                };
                await _services.Media.AddItemAsync(entity, cancellationToken);
                await _services.SaveAsync(cancellationToken);
                var result = new MediaPlaylistItemResult
                {
                    Id = entity.Id,
                    SortOrderId = entity.SortOrderId,
                    Name = entity.Name,
                    MediaType = entity.MediaType,
                    MediaId = entity.MediaId,
                    Duration = entity.Duration,
                    PublishDate = entity.PublishDate
                };
                return Request.CreateResponse(HttpStatusCode.Created, result);
            }
            return Request.CreateResponse(HttpStatusCode.BadRequest);
        }

        /// <summary>
        /// Represents an action that handles only HTTP POST requests.
        /// </summary>
        [Route("~/api/project/playlists/{playlistId:int}/items")]
        [AuthorizeProject]
        public async Task<HttpResponseMessage> PostAllAsync(int playlistId, [FromBody] IList<PlaylistItemDto> model, CancellationToken cancellationToken)
        {
            if (model == null)
            {
                return Request.CreateResponse(HttpStatusCode.BadRequest);
            }
            var playlist = await _services.Media.FindByIdAsync(playlistId, cancellationToken);
            if (playlist == null)
            {
                return Request.CreateFaultResponse(HttpStatusCode.NotFound, "Playlist was not found.");
            }
            if (ProjectIdentity == null || ProjectIdentity.Id != playlist.ContactId)
            {
                return Request.CreateFaultResponse(HttpStatusCode.Forbidden, "Playlist has already been taken.");
            }
            var store = _services.Media;
            foreach (var playlistItem in model.Reverse())
            {
                await store.AddItemAsync(new MediaPlaylistItem
                {
                    PlaylistId = playlistId,
                    Name = playlistItem.Name,
                    MediaType = playlistItem.MediaType,
                    MediaId = playlistItem.MediaId,
                    Duration = playlistItem.Duration,
                    PublishDate = playlistItem.PublishDate
                },
                cancellationToken);
            }
            await _services.SaveAsync(cancellationToken);
            return Request.CreateResponse(HttpStatusCode.Created);
        }

        /// <summary>
        /// Represents an action that handles only HTTP MOVE requests.
        /// </summary>
        [Route("{id}/{sortOrderId}"), AcceptVerbs("MOVE")]
        [AuthorizeProject]
        public async Task<HttpResponseMessage> MoveAsync(int id, int sortOrderId, CancellationToken cancellationToken)
        {
            await _services.Media.MoveItemAsync(id, sortOrderId, cancellationToken);
            await _services.SaveAsync(cancellationToken);
            return Request.CreateResponse(HttpStatusCode.OK);
        }

        /// <summary>
        /// Represents an action that handles only HTTP DELETE requests.
        /// </summary>
        [Route("{id}")]
        [AuthorizeProject]
        public async Task<HttpResponseMessage> DeleteAsync(int id, CancellationToken cancellationToken)
        {
            await _services.Media.DeleteItemAsync(id, cancellationToken);
            await _services.SaveAsync(cancellationToken);
            return Request.CreateResponse(HttpStatusCode.NoContent);
        }
    }
}