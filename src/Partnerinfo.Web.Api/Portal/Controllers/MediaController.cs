// Copyright (c) János Janka. All rights reserved.

using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;
using System.Web.Http.Description;
using System.Web.Routing;
using Partnerinfo.Portal.Models;
using Partnerinfo.Security;

namespace Partnerinfo.Portal.Controllers
{
    [Authorize]
    [RoutePrefix("api/portal/media")]
    public sealed class MediaController : ApiController
    {
        private readonly PortalManager _portalManager;

        /// <summary>
        /// Initializes a new instance of the <see cref="MediaController" /> class.
        /// </summary>
        /// <param name="portalManager">The portal manager that the <see cref="MediaController" /> operates against.</param>
        public MediaController(PortalManager portalManager)
        {
            _portalManager = portalManager;
        }

        /// <summary>
        /// Represents an action that handles only HTTP GET requests.
        /// </summary>
        [Route("{portalUri}")]
        [Route("{portalUri}/{*mediaUri}", Name = "Portals.Media.GetByUri")]
        [ResponseType(typeof(ListResult<MediaItemDto>))]
        public async Task<IHttpActionResult> GetByUriAsync([FromUri] MediaQueryDto model, CancellationToken cancellationToken)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest();
            }
            if (model == null)
            {
                model = new MediaQueryDto();
            }

            var portal = await _portalManager.FindByUriAsync(model.PortalUri, cancellationToken);
            await ApiSecurity.AuthorizeAsync(portal, AccessPermission.CanView, cancellationToken);

            var parent = default(MediaItem);
            if (model.MediaUri != null)
            {
            }
            return Ok(ModelMapper.ToMediaListDto(
                await _portalManager.GetMediaAsync(portal, parent, model.Name, model.OrderBy, model.Fields, cancellationToken),
                uri => _portalManager.GetMediaLinkByUri(portal.Uri, uri)));
        }

        /// <summary>
        /// Represents an action that handles only HTTP POST requests.
        /// </summary>
        [Route("{portalUri}")]
        [ResponseType(typeof(MediaItemDto))]
        public Task<IHttpActionResult> PostAsync(string portalUri, CancellationToken cancellationToken)
        {
            return PostAsync(portalUri, null, cancellationToken);
        }

        /// <summary>
        /// Represents an action that handles only HTTP POST requests.
        /// </summary>
        [Route("{portalUri}/{*mediaUri}")]
        [ResponseType(typeof(MediaItemDto))]
        public async Task<IHttpActionResult> PostAsync(string portalUri, string mediaUri, CancellationToken cancellationToken)
        {
            var portal = await _portalManager.FindByUriAsync(portalUri, cancellationToken);
            await ApiSecurity.AuthorizeAsync(portal, AccessPermission.CanEdit, cancellationToken);

            var media = default(MediaItem);

            using (var content = await Request.Content.ReadAsFileStreamAsync(cancellationToken))
            {
                var validationResult = await _portalManager.AddMediaAsync(
                    portal,
                    null,
                    media = new MediaItem
                    {
                        Uri = content.FileName,
                        Type = MimeMapping.GetMimeMapping(content.FileName),
                        Name = content.FileName
                    },
                    content.FileStream,
                    cancellationToken);

                if (!validationResult.Succeeded)
                {
                    return this.ValidationContent(validationResult);
                }
            }

            return CreatedAtRoute(
                "Portals.Media.GetByUri",
                new RouteValueDictionary { ["portalUri"] = portal.Uri, ["mediaUri"] = media.Uri },
                ModelMapper.ToMediaDto(media, uri => _portalManager.GetMediaLinkByUri(portal.Uri, uri)));
        }

        /// <summary>
        /// Represents an action that handles only HTTP POST requests.
        /// </summary>
        [Route("{portalUri}/{*mediaUri}")]
        public async Task<IHttpActionResult> DeleteAsync(string portalUri, string mediaUri, CancellationToken cancellationToken)
        {
            var portal = await _portalManager.FindByUriAsync(portalUri, cancellationToken);
            await ApiSecurity.AuthorizeAsync(portal, AccessPermission.CanEdit, cancellationToken);

            var media = await _portalManager.GetMediaByUriAsync(portal, mediaUri, cancellationToken);
            if (media != null)
            {
                await _portalManager.RemoveMediaAsync(portal, media, cancellationToken);
            }
            return StatusCode(HttpStatusCode.NoContent);
        }
    }
}