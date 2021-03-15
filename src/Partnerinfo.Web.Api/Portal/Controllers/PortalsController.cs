// Copyright (c) János Janka. All rights reserved.

using System.Net;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Description;
using System.Web.Routing;
using Partnerinfo.Identity;
using Partnerinfo.Portal.Models;
using Partnerinfo.Project;
using Partnerinfo.Security;

namespace Partnerinfo.Portal.Controllers
{
    [Authorize]
    [RoutePrefix("api/portals")]
    public sealed class PortalsController : ApiController
    {
        private readonly UserManager _userManager;
        private readonly ProjectManager _projectManager;
        private readonly PortalManager _portalManager;

        /// <summary>
        /// Initializes a new instance of the <see cref="PortalsController" /> class.
        /// </summary>
        /// <param name="userManager">The user manager that the <see cref="PortalsController" /> operates against.</param>
        /// <param name="projectManager">The project manager that the <see cref="PortalsController" /> operates against.</param>
        /// <param name="portalManager">The project manager that the <see cref="PortalsController" /> operates against.</param>
        public PortalsController(UserManager userManager, ProjectManager projectManager, PortalManager portalManager)
        {
            _userManager = userManager;
            _projectManager = projectManager;
            _portalManager = portalManager;
        }

        /// <summary>
        /// Represents an action that handles only HTTP GET requests.
        /// </summary>
        [Route("")]
        [ResponseType(typeof(ListResult<PortalItemDto>))]
        public async Task<IHttpActionResult> GetAllAsync([FromUri] PortalQueryDto model, CancellationToken cancellationToken)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest();
            }
            if (model == null)
            {
                model = new PortalQueryDto();
            }
            return Ok(ModelMapper.ToPortalListDto(
                await _portalManager.FindAllAsync(
                    ApiSecurity.CurrentUserId,
                    model.Name,
                    model.OrderBy,
                    model.Page,
                    model.Limit,
                    model.Fields,
                    cancellationToken)));
        }

        /// <summary>
        /// Represents an action that handles only HTTP GET requests.
        /// </summary>
        [Route("{uri}", Name = "Portals.GetByUri")]
        [ResponseType(typeof(PortalItemDto))]
        public Task<IHttpActionResult> GetByUriAsync(string uri, CancellationToken cancellationToken)
        {
            return GetByUriAsync(uri, PortalQueryField.None, cancellationToken);
        }

        /// <summary>
        /// Represents an action that handles only HTTP GET requests.
        /// </summary>
        [Route("{uri}")]
        [ResponseType(typeof(PortalItemDto))]
        public async Task<IHttpActionResult> GetByUriAsync(string uri, PortalQueryField fields, CancellationToken cancellationToken)
        {
            var portal = await _portalManager.FindByUriAsync(uri, (PortalField)(fields & ~PortalQueryField.Pages), cancellationToken);
            await ApiSecurity.AuthorizeAsync(portal, AccessPermission.CanView, cancellationToken);
            return Ok(ModelMapper.ToPortalDto(portal, fields.HasFlag(PortalQueryField.Pages) ? await _portalManager.GetPagesAsync(portal, cancellationToken) : null));
        }

        /// <summary>
        /// Creates the specified portal, as an asynchronous HTTP POST operation.
        /// </summary>
        /// <param name="model">The model that contains portal information.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken" /> used to propagate notifications that the operation should be canceled.</param>
        /// <returns>
        /// The <see cref="Task" /> that represents the asynchronous operation, containing the <see cref="IHttpActionResult" /> of the update operation.
        /// </returns>
        [Route("")]
        [ResponseType(typeof(PortalItemDto))]
        public async Task<IHttpActionResult> PostAsync([FromBody] CreatePortalDto model, CancellationToken cancellationToken)
        {
            if (model == null || !ModelState.IsValid)
            {
                return BadRequest();
            }
            var user = await _userManager.FindByEmailAsync(User.Identity.Name, cancellationToken);
            if (user == null)
            {
                return NotFound();
            }
            var portal = new PortalItem
            {
                Uri = model.Uri,
                Name = model.Name,
                Description = model.Description,
                GATrackingId = model.GATrackingId
            };
            var validationResult = await _portalManager.CreateAsync(portal, new[]
            {
                new AccessRuleItem { User = null, Permission = AccessPermission.CanView, Visibility = AccessVisibility.Public },
                new AccessRuleItem { User = user, Permission = AccessPermission.IsOwner }
            },
            model.Template, cancellationToken);

            if (!validationResult.Succeeded)
            {
                return this.ValidationContent(validationResult);
            }
            if (model.Project != null)
            {
                await ApiSecurity.AuthorizeAsync(AccessObjectType.Project, model.Project.Id, AccessPermission.IsOwner, cancellationToken);
                await _portalManager.SetProjectAsync(portal, model.Project, cancellationToken);
            }
            return CreatedAtRoute("Portals.GetByUri", new RouteValueDictionary { { "uri", portal.Uri } }, ModelMapper.ToPortalDto(portal));
        }

        /// <summary>
        /// Updates the specified portal, as an asynchronous HTTP PUT operation.
        /// </summary>
        /// <param name="uri">The uri to search for.</param>
        /// <param name="model">The model to update.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken" /> used to propagate notifications that the operation should be canceled.</param>
        /// <returns>
        /// The <see cref="Task" /> that represents the asynchronous operation, containing the <see cref="IHttpActionResult" /> of the update operation.
        /// </returns>
        [Route("{uri}")]
        [ResponseType(typeof(PortalItemDto))]
        public async Task<IHttpActionResult> PutAsync(string uri, [FromBody] SetPortalHeadDto model, CancellationToken cancellationToken)
        {
            if (model == null || !ModelState.IsValid)
            {
                return BadRequest();
            }
            var portal = await _portalManager.FindByUriAsync(uri, cancellationToken);
            await ApiSecurity.AuthorizeAsync(portal, AccessPermission.CanEdit, cancellationToken);

            ValidationResult validationResult;

            if (model.Uri != null)
            {
                validationResult = await _portalManager.SetUriAsync(portal, model.Uri, cancellationToken);
                if (!validationResult.Succeeded)
                {
                    return this.ValidationContent(validationResult);
                }
            }

            portal.Name = model.Name;
            portal.Description = model.Description;
            portal.GATrackingId = model.GATrackingId;

            validationResult = await _portalManager.UpdateAsync(portal, cancellationToken);
            if (!validationResult.Succeeded)
            {
                return this.ValidationContent(validationResult);
            }

            return Ok(ModelMapper.ToPortalDto(portal));
        }

        /// <summary>
        /// Copies the specified portal, as an asynchronous HTTP COPY operation.
        /// </summary>
        /// <param name="uri">The uri to search for.</param>
        /// <param name="model">The portal to copy.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken" /> used to propagate notifications that the operation should be canceled.</param>
        /// <returns>
        /// The <see cref="Task" /> that represents the asynchronous operation, containing the <see cref="IHttpActionResult" /> of the update operation.
        /// </returns>
        [Route("{uri}")]
        [AcceptVerbs("COPY")]
        public async Task<IHttpActionResult> CopyAsync(string uri, [FromBody] CopyPortalDto model, CancellationToken cancellationToken)
        {
            if (model == null || !ModelState.IsValid)
            {
                return BadRequest();
            }
            var portal = await _portalManager.FindByUriAsync(uri, cancellationToken);
            await ApiSecurity.AuthorizeAsync(portal, AccessPermission.CanEdit, cancellationToken);

            var validationResult = await _portalManager.CopyAsync(portal, model.Uri, cancellationToken);
            if (!validationResult.Succeeded)
            {
                return this.ValidationContent(validationResult);
            }
            return Ok();
        }

        /// <summary>
        /// Deletes the specified portal, as an asynchronous HTTP DELETE operation.
        /// </summary>
        /// <param name="uri">The uri to search for.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken" /> used to propagate notifications that the operation should be canceled.</param>
        /// <returns>
        /// The <see cref="Task" /> that represents the asynchronous operation, containing the <see cref="IHttpActionResult" /> of the update operation.
        /// </returns>
        [Route("{uri}")]
        public async Task<IHttpActionResult> DeleteAsync(string uri, CancellationToken cancellationToken)
        {
            var portal = await _portalManager.FindByUriAsync(uri, cancellationToken);
            await ApiSecurity.AuthorizeAsync(portal, AccessPermission.IsOwner, cancellationToken);

            await _portalManager.DeleteAsync(portal, cancellationToken);
            return StatusCode(HttpStatusCode.NoContent);
        }

        /// <summary>
        /// Sets the home page for the specified portal, as an asynchronous HTTP POST operation.
        /// </summary>
        /// <param name="uri">The uri to search for.</param>
        /// <param name="homePageUri">The home page uri to set.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken" /> used to propagate notifications that the operation should be canceled.</param>
        /// <returns>
        /// The <see cref="Task" /> that represents the asynchronous operation, containing the <see cref="IHttpActionResult" /> of the update operation.
        /// </returns>
        [Route("{uri}/homepage")]
        public async Task<IHttpActionResult> PostHomePageByUriAsync(string uri, [FromBody] string homePageUri, CancellationToken cancellationToken)
        {
            var portal = await _portalManager.FindByUriAsync(uri, cancellationToken);
            await ApiSecurity.AuthorizeAsync(portal, AccessPermission.CanEdit, cancellationToken);

            ValidationResult validationResult;
            if (homePageUri == null)
            {
                validationResult = await _portalManager.SetHomePageAsync(portal, null, cancellationToken);
            }
            else
            {
                var masterPage = await _portalManager.GetPageByUriAsync(portal, homePageUri, cancellationToken);
                if (masterPage == null)
                {
                    return BadRequest(string.Format(PortalApiResources.PageNotFound, uri));
                }
                validationResult = await _portalManager.SetHomePageAsync(portal, masterPage, cancellationToken);
            }
            if (!validationResult.Succeeded)
            {
                return this.ValidationContent(validationResult);
            }
            return Ok();
        }

        /// <summary>
        /// Sets the master page for the specified portal, as an asynchronous HTTP POST operation.
        /// </summary>
        /// <param name="uri">The uri to search for.</param>
        /// <param name="masterPageUri">The master page uri to set.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken" /> used to propagate notifications that the operation should be canceled.</param>
        /// <returns>
        /// The <see cref="Task" /> that represents the asynchronous operation, containing the <see cref="IHttpActionResult" /> of the update operation.
        /// </returns>
        [Route("{uri}/masterpage")]
        public async Task<IHttpActionResult> PostMasterPageByUriAsync(string uri, [FromBody] string masterPageUri, CancellationToken cancellationToken)
        {
            var portal = await _portalManager.FindByUriAsync(uri, cancellationToken);
            await ApiSecurity.AuthorizeAsync(portal, AccessPermission.CanEdit, cancellationToken);

            ValidationResult validationResult;
            if (masterPageUri == null)
            {
                validationResult = await _portalManager.SetMasterPageAsync(portal, null, cancellationToken);
            }
            else
            {
                var masterPage = await _portalManager.GetPageByUriAsync(portal, masterPageUri, cancellationToken);
                if (masterPage == null)
                {
                    return BadRequest(string.Format(PortalApiResources.PageNotFound, uri));
                }
                validationResult = await _portalManager.SetMasterPageAsync(portal, masterPage, cancellationToken);
            }
            if (!validationResult.Succeeded)
            {
                return this.ValidationContent(validationResult);
            }
            return Ok();
        }

        /// <summary>
        /// Sets the project for the specified portal, as an asynchronous HTTP POST operation.
        /// </summary>
        /// <param name="uri">The uri to search for.</param>
        /// <param name="projectId">The project identifier to set.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken" /> used to propagate notifications that the operation should be canceled.</param>
        /// <returns>
        /// The <see cref="Task" /> that represents the asynchronous operation, containing the <see cref="IHttpActionResult" /> of the update operation.
        /// </returns>
        [Route("{uri}/project")]
        public async Task<IHttpActionResult> PostProjectByUriAsync(string uri, [FromBody] int? projectId, CancellationToken cancellationToken)
        {
            var portal = await _portalManager.FindByUriAsync(uri, cancellationToken);
            await ApiSecurity.AuthorizeAsync(portal, AccessPermission.CanEdit, cancellationToken);

            ProjectItem project = null;
            if (projectId != null)
            {
                project = await _projectManager.FindByIdAsync((int)projectId, cancellationToken);
                if (project == null)
                {
                    return BadRequest(string.Format(PortalApiResources.ProjectNotFound, projectId));
                }
                await ApiSecurity.AuthorizeAsync(project, AccessPermission.IsOwner, cancellationToken);
            }
            var validationResult = await _portalManager.SetProjectAsync(portal, project, cancellationToken);
            if (!validationResult.Succeeded)
            {
                return this.ValidationContent(validationResult);
            }
            return Ok();
        }

        /// <summary>
        /// Sets the domain for the specified portal, as an asynchronous HTTP POST operation.
        /// </summary>
        /// <param name="uri">The uri to search for.</param>
        /// <param name="domain">The domain to set.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken" /> used to propagate notifications that the operation should be canceled.</param>
        /// <returns>
        /// The <see cref="Task" /> that represents the asynchronous operation, containing the <see cref="IHttpActionResult" /> of the update operation.
        /// </returns>
        [Route("{uri}/domain")]
        public async Task<IHttpActionResult> PostDomainByUriAsync(string uri, [FromBody] string domain, CancellationToken cancellationToken)
        {
            var portal = await _portalManager.FindByUriAsync(uri, cancellationToken);
            await ApiSecurity.AuthorizeAsync(portal, AccessPermission.CanEdit, cancellationToken);

            if (domain == null)
            {
                using (var hosting = new PortalHosting())
                {
                    portal.Domain = null;
                    var validationResult = await _portalManager.UpdateAsync(portal, cancellationToken);
                    if (validationResult.Succeeded)
                    {
                        hosting.DeleteSite(portal.Domain);
                    }
                    return this.ValidationContent(validationResult);
                }
            }

            if (PortalHosting.IsValidDomain(domain))
            {
                domain = PortalHosting.ExtractDomain(domain);
                if (await PortalHosting.CheckDnsAsync(domain) == false)
                {
                    return BadRequest("DNS not found.");
                }
                using (var hosting = new PortalHosting())
                {
                    if (hosting.DomainExists(domain))
                    {
                        return BadRequest("Duplicated domain.");
                    }
                    portal.Domain = domain;
                    await _portalManager.UpdateAsync(portal, cancellationToken);
                    hosting.CreateSite(domain);
                    hosting.Save();
                }
                return Ok(domain);
            }
            return BadRequest("Invalid domain.");
        }
    }
}