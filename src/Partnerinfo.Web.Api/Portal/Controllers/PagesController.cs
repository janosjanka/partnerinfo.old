// Copyright (c) János Janka. All rights reserved.

using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Description;
using System.Web.Routing;
using Partnerinfo.Portal.Models;
using Partnerinfo.Project;
using Partnerinfo.Security;

namespace Partnerinfo.Portal.Controllers
{
    [Authorize]
    [RoutePrefix("api/portal/pages")]
    public sealed class PagesController : ApiController
    {
        private readonly ProjectManager _projectManager;
        private readonly PortalManager _portalManager;
        private readonly PortalCompiler _portalCompiler;

        /// <summary>
        /// Initializes a new instance of the <see cref="PagesController" /> class.
        /// </summary>
        /// <param name="projectManager">The project manager that the <see cref="PagesController" /> operates against.</param>
        /// <param name="portalManager">The portal manager that the <see cref="PagesController" /> operates against.</param>
        /// <param name="portalCompiler">The portal compiler that the <see cref="PagesController" /> operates against.</param>
        public PagesController(ProjectManager projectManager, PortalManager portalManager, PortalCompiler portalCompiler)
        {
            _projectManager = projectManager;
            _portalManager = portalManager;
            _portalCompiler = portalCompiler;
        }

        /// <summary>
        /// Represents an action that handles only HTTP GET requests.
        /// </summary>
        [Route("")]
        [ResponseType(typeof(IList<PageItemDto>))]
        public async Task<IHttpActionResult> GetAllAsync(string portalUri, CancellationToken cancellationToken)
        {
            var portal = await _portalManager.FindByUriAsync(portalUri, cancellationToken);
            await ApiSecurity.AuthorizeAsync(portal, AccessPermission.CanView, cancellationToken);

            var pages = await _portalManager.GetPagesAsync(portal, cancellationToken);
            if (pages == null)
            {
                return NotFound();
            }
            return Ok(ModelMapper.ToPageListDto(pages));
        }

        /// <summary>
        /// Represents an action that handles only HTTP GET requests.
        /// </summary>
        [Route("{id:int}", Name = "Portal.Pages.GetById")]
        [ResponseType(typeof(PageItemDto))]
        public Task<IHttpActionResult> GetByUriAsync(int id, CancellationToken cancellationToken)
        {
            return GetByIdAsync(id, PageField.None, cancellationToken);
        }

        /// <summary>
        /// Represents an action that handles only HTTP GET requests.
        /// </summary>
        [Route("{id:int}")]
        [ResponseType(typeof(PageItemDto))]
        public async Task<IHttpActionResult> GetByIdAsync(int id, PageField fields, CancellationToken cancellationToken)
        {
            var page = await _portalManager.GetPageByIdAsync(id, fields | PageField.Portal, cancellationToken);
            if (page == null)
            {
                return NotFound();
            }
            await ApiSecurity.AuthorizeAsync(AccessObjectType.Portal, page.Portal.Id, AccessPermission.CanView, cancellationToken);

            return Ok(ModelMapper.ToPageDto(page));
        }

        /// <summary>
        /// Represents an action that handles only HTTP GET requests.
        /// </summary>
        [Route("{portalUri}/{*pageUri}", Name = "Portal.Pages.GetByUri")]
        [ResponseType(typeof(PageItemDto))]
        public Task<IHttpActionResult> GetByUriAsync(string portalUri, string pageUri, CancellationToken cancellationToken)
        {
            return GetByUriAsync(portalUri, pageUri, PageField.None, cancellationToken);
        }

        /// <summary>
        /// Represents an action that handles only HTTP GET requests.
        /// </summary>
        [Route("{portalUri}/{*pageUri}")]
        [ResponseType(typeof(PageItemDto))]
        public async Task<IHttpActionResult> GetByUriAsync(string portalUri, string pageUri, PageField fields, CancellationToken cancellationToken)
        {
            var portal = await _portalManager.FindByUriAsync(portalUri, cancellationToken);
            await ApiSecurity.AuthorizeAsync(portal, AccessPermission.CanView, cancellationToken);

            var page = await _portalManager.GetPageByUriAsync(portal, pageUri, fields, cancellationToken);
            if (page == null)
            {
                return NotFound();
            }
            return Ok(ModelMapper.ToPageDto(page));
        }

        /// <summary>
        /// Represents an action that handles only HTTP GET requests.
        /// </summary>
        [Route("layers/{portalUri}/{*pageUri}")]
        [ResponseType(typeof(PageLayersDto))]
        public async Task<IHttpActionResult> GetLayersByUriAsync(string portalUri, string pageUri, CancellationToken cancellationToken)
        {
            var portal = await _portalManager.FindByUriAsync(portalUri, PortalField.All, cancellationToken);
            await ApiSecurity.AuthorizeAsync(portal, AccessPermission.CanView, cancellationToken);

            var pageLayers = await _portalManager.GetPageLayersByUriAsync(portal, pageUri, cancellationToken);
            if (pageLayers?.ContentPage == null)
            {
                return NotFound();
            }

            var compilerResult = await _portalCompiler.CompileAsync(
               new PortalCompilerOptions
               (
                   portal: portal,
                   contentPage: pageLayers.ContentPage,
                   masterPages: pageLayers.MasterPages.ToImmutableArray()
               ),
               cancellationToken);

            return Ok(ModelMapper.ToPageLayersDto(portal, compilerResult));
        }

        /// <summary>
        /// Represents an action that handles only HTTP PUT requests.
        /// </summary>
        [Route("{portalUri}")]
        [ResponseType(typeof(PageItemDto))]
        public Task<IHttpActionResult> PostAsync(string portalUri, [FromBody] CreatePageDto model, CancellationToken cancellationToken)
        {
            return PostAsync(portalUri, null, model, cancellationToken);
        }

        /// <summary>
        /// Represents an action that handles only HTTP PUT requests.
        /// </summary>
        [Route("{portalUri}/{*pageUri}")]
        [ResponseType(typeof(PageItemDto))]
        public async Task<IHttpActionResult> PostAsync(string portalUri, string pageUri, [FromBody] CreatePageDto model, CancellationToken cancellationToken)
        {
            if (model == null || !ModelState.IsValid)
            {
                return BadRequest();
            }
            var portal = await _portalManager.FindByUriAsync(portalUri, cancellationToken);
            await ApiSecurity.AuthorizeAsync(portal, AccessPermission.CanEdit, cancellationToken);

            var parentPage = default(PageItem);
            if (pageUri != null)
            {
                parentPage = await _portalManager.GetPageByUriAsync(portal, pageUri, cancellationToken);
                if (parentPage == null)
                {
                    return BadRequest(string.Format(PortalApiResources.PageNotFound, pageUri));
                }
            }
            var page = new PageItem
            {
                Uri = model.Uri,
                Name = model.Name,
                Description = model.Description,
                HtmlContent = model.HtmlContent,
                StyleContent = model.StyleContent
            };
            var validationResult = await _portalManager.AddPageAsync(portal, parentPage, page, cancellationToken);
            if (!validationResult.Succeeded)
            {
                return this.ValidationContent(validationResult);
            }
            return CreatedAtRoute("Portal.Pages.GetByUri", new RouteValueDictionary { ["portalUri"] = portalUri, ["pageUri"] = page.Uri }, ModelMapper.ToPageDto(page));
        }

        /// <summary>
        /// Represents an action that handles only HTTP PUT requests.
        /// </summary>
        [Route("{portalUri}/{*pageUri}")]
        [ResponseType(typeof(PageItemDto))]
        public async Task<IHttpActionResult> PutAsync(string portalUri, string pageUri, [FromBody] SetPageHeadDto model, CancellationToken cancellationToken)
        {
            if (model == null || !ModelState.IsValid)
            {
                return BadRequest();
            }
            var portal = await _portalManager.FindByUriAsync(portalUri, cancellationToken);
            await ApiSecurity.AuthorizeAsync(portal, AccessPermission.CanEdit, cancellationToken);

            var page = await _portalManager.GetPageByUriAsync(portal, pageUri, PageField.All, cancellationToken);
            if (page == null)
            {
                return NotFound();
            }

            ValidationResult validationResult;
            if (model.Uri != null)
            {
                validationResult = await _portalManager.SetPageUriAsync(portal, page, model.Uri, cancellationToken);
                if (!validationResult.Succeeded)
                {
                    return this.ValidationContent(validationResult);
                }
            }

            page.Name = model.Name;
            page.Description = model.Description;

            validationResult = await _portalManager.ReplacePageAsync(portal, page, page, cancellationToken);
            if (!validationResult.Succeeded)
            {
                return this.ValidationContent(validationResult);
            }

            return Ok(ModelMapper.ToPageDto(page));
        }

        /// <summary>
        /// Represents an action that handles only HTTP GET requests.
        /// </summary>
        [Route("content/{portalUri}/{*pageUri}")]
        [ResponseType(typeof(PageItemDto))]
        public async Task<IHttpActionResult> PostContentAsync(string portalUri, string pageUri, [FromBody] SetPageBodyDto model, CancellationToken cancellationToken)
        {
            if (model == null || !ModelState.IsValid)
            {
                return BadRequest();
            }
            var portal = await _portalManager.FindByUriAsync(portalUri, cancellationToken);
            await ApiSecurity.AuthorizeAsync(portal, AccessPermission.CanEdit, cancellationToken);

            var page = await _portalManager.GetPageByUriAsync(portal, pageUri, PageField.All, cancellationToken);
            if (page == null)
            {
                return NotFound();
            }

            page.HtmlContent = model.HtmlContent;
            page.StyleContent = model.StyleContent;

            var validationResult = await _portalManager.ReplacePageAsync(portal, page, page, cancellationToken);
            if (!validationResult.Succeeded)
            {
                return this.ValidationContent(validationResult);
            }
            return Ok(ModelMapper.ToPageDto(page));
        }

        /// <summary>
        /// Represents an action that handles only HTTP GET requests.
        /// </summary>
        [Route("master/{portalUri}/{*pageUri}")]
        [ResponseType(typeof(PageItemDto))]
        public async Task<IHttpActionResult> PostMasterAsync(string portalUri, string pageUri, [FromBody] string uri, CancellationToken cancellationToken)
        {
            var portal = await _portalManager.FindByUriAsync(portalUri, cancellationToken);
            await ApiSecurity.AuthorizeAsync(portal, AccessPermission.CanEdit, cancellationToken);

            var page = await _portalManager.GetPageByUriAsync(portal, pageUri, cancellationToken);
            if (page == null)
            {
                return NotFound();
            }
            var validationResult = await _portalManager.SetPageMasterAsync(portal, page, uri, cancellationToken);
            if (!validationResult.Succeeded)
            {
                return this.ValidationContent(validationResult);
            }
            return Ok(ModelMapper.ToPageDto(page));
        }

        /// <summary>
        /// Represents an action that handles only HTTP GET requests.
        /// </summary>
        [Route("references/{portalUri}/{*pageUri}")]
        [ResponseType(typeof(PageItemDto))]
        public async Task<IHttpActionResult> PostReferencesAsync(string portalUri, string pageUri, [FromBody] IEnumerable<ReferenceItemDto> model, CancellationToken cancellationToken)
        {
            if (model == null || !ModelState.IsValid)
            {
                return BadRequest();
            }
            var portal = await _portalManager.FindByUriAsync(portalUri, cancellationToken);
            await ApiSecurity.AuthorizeAsync(portal, AccessPermission.CanEdit, cancellationToken);

            var page = await _portalManager.GetPageByUriAsync(portal, pageUri, cancellationToken);
            if (page == null)
            {
                return NotFound();
            }
            var validationResult = await _portalManager.SetPageReferencesAsync(portal, page, model.Select(r => ReferenceItem.Create(r.Type, r.Uri)), cancellationToken);
            if (!validationResult.Succeeded)
            {
                return this.ValidationContent(validationResult);
            }
            return Ok(ModelMapper.ToPageDto(page));
        }

        /// <summary>
        /// Copies the specified page, as an asynchronous HTTP COPY operation.
        /// </summary>
        /// <param name="uri">The uri to search for.</param>
        /// <param name="model">The portal to copy.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken" /> used to propagate notifications that the operation should be canceled.</param>
        /// <returns>
        /// The <see cref="Task" /> that represents the asynchronous operation, containing the <see cref="IHttpActionResult" /> of the update operation.
        /// </returns>
        [AcceptVerbs("COPY")]
        [Route("{portalUri}/{*pageUri}")]
        public async Task<IHttpActionResult> CopyAsync(string portalUri, string pageUri, [FromBody] CopyPageDto model, CancellationToken cancellationToken)
        {
            if (model == null || !ModelState.IsValid)
            {
                return BadRequest();
            }
            var portal = await _portalManager.FindByUriAsync(portalUri, cancellationToken);
            await ApiSecurity.AuthorizeAsync(portal, AccessPermission.CanEdit, cancellationToken);

            var page = await _portalManager.GetPageByUriAsync(portal, pageUri, cancellationToken);
            if (page == null)
            {
                return NotFound();
            }
            var validationResult = await _portalManager.CopyPageAsync(portal, page, model.Uri, cancellationToken);
            if (!validationResult.Succeeded)
            {
                return this.ValidationContent(validationResult);
            }
            return Ok();
        }

        /// <summary>
        /// Moves the specified page, as an asynchronous HTTP COPY operation.
        /// </summary>
        /// <param name="uri">The uri to search for.</param>
        /// <param name="model">The portal to copy.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken" /> used to propagate notifications that the operation should be canceled.</param>
        /// <returns>
        /// The <see cref="Task" /> that represents the asynchronous operation, containing the <see cref="IHttpActionResult" /> of the update operation.
        /// </returns>
        [AcceptVerbs("MOVE")]
        [Route("{portalUri}/{*pageUri}")]
        public async Task<IHttpActionResult> MoveAsync(string portalUri, string pageUri, [FromBody] string uri, CancellationToken cancellationToken)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest();
            }
            var portal = await _portalManager.FindByUriAsync(portalUri, cancellationToken);
            await ApiSecurity.AuthorizeAsync(portal, AccessPermission.CanEdit, cancellationToken);

            var page = await _portalManager.GetPageByUriAsync(portal, pageUri, cancellationToken);
            if (page == null)
            {
                return NotFound();
            }
            var referencePage = (uri == null) ? null : await _portalManager.GetPageByUriAsync(portal, uri, cancellationToken);
            var validationResult = await _portalManager.MovePageAsync(portal, page, referencePage, cancellationToken);
            if (!validationResult.Succeeded)
            {
                return this.ValidationContent(validationResult);
            }
            return Ok();
        }

        /// <summary>
        /// Represents an action that handles only HTTP DELETE requests.
        /// </summary>
        [Route("{portalUri}/{*pageUri}")]
        public async Task<IHttpActionResult> DeleteAsync(string portalUri, string pageUri, CancellationToken cancellationToken)
        {
            var portal = await _portalManager.FindByUriAsync(portalUri, cancellationToken);
            await ApiSecurity.AuthorizeAsync(portal, AccessPermission.CanEdit, cancellationToken);

            var page = await _portalManager.GetPageByUriAsync(portal, pageUri, cancellationToken);
            if (page != null)
            {
                await _portalManager.RemovePageAsync(portal, page, cancellationToken);
            }
            return StatusCode(HttpStatusCode.NoContent);
        }
    }
}