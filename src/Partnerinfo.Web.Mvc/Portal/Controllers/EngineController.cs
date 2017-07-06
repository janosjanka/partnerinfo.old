// Copyright (c) János Janka. All rights reserved.

using System;
using System.Collections.Immutable;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using Partnerinfo.Logging;
using Partnerinfo.Portal.ModelBinders;
using Partnerinfo.Portal.ViewModels;
using Partnerinfo.Project;
using Partnerinfo.Project.Actions;
using Partnerinfo.Project.Templating;
using Partnerinfo.Security;

namespace Partnerinfo.Portal.Controllers
{
    /// <summary>
    /// Provides methods that respond to HTTP requests that are made to an ASP.NET MVC Web site.
    /// </summary>
    public sealed class EngineController : Controller
    {
        private readonly SecurityManager _securityManager;
        private readonly ProjectManager _projectManager;
        private readonly PortalManager _portalManager;
        private readonly PortalCompiler _portalCompiler;
        private readonly LogManager _logManager;

        /// <summary>
        /// Initializes a new instance of the <see cref="EngineController" /> class.
        /// </summary>
        /// <param name="securityManager">The security manager that the <see cref="EngineController" /> operates against.</param>
        /// <param name="projectManager">The project manager that the <see cref="EngineController" /> operates against.</param>
        /// <param name="portalManager">The portal manager that the <see cref="EngineController" /> operates against.</param>
        /// <param name="portalCompiler">The portal compiler that the <see cref="EngineController" /> operates against.</param>
        /// <param name="logManager">The log manager that the <see cref="EngineController" /> operates against.</param>
        public EngineController(
            SecurityManager securityManager,
            ProjectManager projectManager,
            PortalManager portalManager,
            PortalCompiler portalCompiler,
            LogManager logManager)
        {
            _securityManager = securityManager;
            _projectManager = projectManager;
            _portalManager = portalManager;
            _portalCompiler = portalCompiler;
            _logManager = logManager;
        }

        /// <summary>
        /// Creates a ViewResult object that renders a view to the response.
        /// </summary>
        /// <param name="portalUri">The portal URI to search for.</param>
        /// <param name="pageUri">The page URI to search for.</param>
        /// <param name="preview">if set to <c>true</c> [preview].</param>
        /// <param name="actionLink">The action link.</param>
        /// <param name="authTicket">The authentication ticket.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns></returns>
        [AsyncTimeout(10000)]
        [OutputCache(CacheProfile = "PortalCache")]
        public async Task<System.Web.Mvc.ActionResult> Portal(
            string portalUri,
            string pageUri,
            string preview,
            [ModelBinder(typeof(ActionLinkModelBinder))] ActionLink actionLink,
            [ModelBinder(typeof(AuthTicketModelBinder))] AuthTicket authTicket,
            CancellationToken cancellationToken)
        {
            var portal = await _portalManager.FindByUriAsync(portalUri, PortalField.Project | PortalField.Owners, cancellationToken);
            if (portal == null)
            {
                return HttpNotFound();
            }
            if (!string.IsNullOrEmpty(portal.Domain))
            {
                return Redirect(string.Join("/", "http://www." + portal.Domain, pageUri));
            }
            var security = await _securityManager.CheckAccessAsync(portal, User.Identity.Name, AccessPermission.CanView, cancellationToken);
            if (!security.AccessGranted)
            {
                return new HttpStatusCodeResult(HttpStatusCode.Forbidden);
            }
            var pageLayers = await _portalManager.GetPageLayersByUriAsync(portal, pageUri, cancellationToken);
            if (pageLayers?.ContentPage == null)
            {
                return HttpNotFound();
            }
            return View("Content", await CreateResultAsync(security, portal, pageLayers, actionLink, authTicket, preview == null, cancellationToken));
        }

        /// <summary>
        /// Creates a ViewResult object that renders a view to the response.
        /// </summary>
        /// <param name="domain">The portal domain.</param>
        /// <param name="pageUri">The page URI.</param>
        /// <param name="preview">The preview.</param>
        /// <param name="actionLink">The action link.</param>
        /// <param name="authTicket">The authentication ticket.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns></returns>
        [AsyncTimeout(10000)]
        [OutputCache(CacheProfile = "PortalCache")]
        public async Task<System.Web.Mvc.ActionResult> Domain(
            string domain,
            string pageUri,
            string preview,
            [ModelBinder(typeof(ActionLinkModelBinder))] ActionLink actionLink,
            [ModelBinder(typeof(AuthTicketModelBinder))] AuthTicket authTicket,
            CancellationToken cancellationToken)
        {
            var portal = await _portalManager.FindByDomainAsync(domain, PortalField.Project | PortalField.Owners, cancellationToken);
            if (portal == null)
            {
                return HttpNotFound();
            }
            var security = await _securityManager.CheckAccessAsync(portal, User.Identity.Name, AccessPermission.CanView, cancellationToken);
            if (!security.AccessGranted)
            {
                return new HttpStatusCodeResult(HttpStatusCode.Forbidden);
            }
            var pageLayers = await _portalManager.GetPageLayersByUriAsync(portal, pageUri, cancellationToken);
            if (pageLayers?.ContentPage == null)
            {
                return HttpNotFound();
            }
            return View("Content", await CreateResultAsync(security, portal, pageLayers, actionLink, authTicket, preview == null, cancellationToken));
        }

        /// <summary>
        /// Creates a <see cref="EngineResultViewModel" /> that represents page information.
        /// Asynchronous calls cannot be inlined so we keep these in one method body.
        /// </summary>
        /// <param name="portal">The portal which owns the page.</param>
        /// <param name="pageLayers">The page layers that can be compiled by <see cref="PortalCompiler" />.</param>
        /// <param name="actionLink">A deserialized form of an action link.</param>
        /// <param name="authTicket">A deserialized form of an authentication ticket.</param>
        /// <param name="addEventLogItem">Logs the current request if set to <c>true</c>.</param>
        /// <param name="cancellationToken">A token to observe while waiting for the task to complete.</param>
        /// <returns>
        /// A task that represents the asynchronous operation.
        /// </returns>
        private async Task<EngineResultViewModel> CreateResultAsync(
            SecurityResult security,
            PortalItem portal,
            PageLayers pageLayers,
            ActionLink actionLink,
            AuthTicket authTicket,
            bool addEventLogItem,
            CancellationToken cancellationToken)
        {
            var result = new EngineResultViewModel
            {
                AnonymId = Request.GetAnonymId() ?? GuidUtility.NewSequentialGuid(), // Gets the Anonym User ID from the request.
                Portal = new EnginePortalViewModel
                {
                    Uri = portal.Uri,
                    Name = portal.Name,
                    Description = portal.Description,
                    GATrackingId = portal.GATrackingId,
                    Project = portal.Project
                },
                Security = security
            };
            var cookiePath = portal.Uri;
            var contact = default(ContactItem);

            Response.Cookies.Remove(ResourceKeys.ContactTokenCookieName);

            if (authTicket != null)
            {
                // Generate a bearer token for the ticket to authenticate
                // HTTP requests are from client side if the specified contact exists.

                contact = await _projectManager.GetContactByIdAsync(authTicket.Id, cancellationToken);
                if (contact != null)
                {
                    result.Identity = AuthUtility.Create(contact);
                    result.IdentityToken = AuthUtility.Protect(result.Identity);
                    Response.Cookies.Add(new HttpCookie(ResourceKeys.ContactTokenCookieName, result.IdentityToken) { Path = cookiePath, HttpOnly = true });
                }
            }
            else if (actionLink?.ContactId != null)
            {
                // Generate a simple identity without a bearer token
                // to identify a user without authenticating him/her.

                contact = await _projectManager.GetContactByIdAsync((int)actionLink.ContactId, cancellationToken);
                if (contact != null)
                {
                    result.Identity = AuthUtility.Create(contact);
                }
            }

            // Compile the page and replace all the placeholders, action links, etc.

            var compilerResult = await _portalCompiler.CompileAsync(
                new PortalCompilerOptions
                (
                    portal: portal,
                    contentPage: pageLayers.ContentPage,
                    masterPages: pageLayers.MasterPages.ToImmutableArray(),
                    properties: new PropertyDictionary
                    {
                        [StringTemplate.IdentityIdProperty] = result.Identity?.Id,
                        [PortalAppResources.IdentityProperty] = contact
                    }.ToImmutableDictionary(),
                    compilerFlags: PortalCompilerFlags.MergeMasterAndContent | PortalCompilerFlags.InterpolateHtmlContent
                ),
                cancellationToken);

            result.Page = new EnginePageViewModel
            {
                Uri = compilerResult.CompiledPage.Uri,
                Name = compilerResult.CompiledPage.Name,
                Description = compilerResult.CompiledPage.Description,
                HtmlContent = compilerResult.CompiledPage.HtmlContent,
                StyleContent = compilerResult.CompiledPage.StyleContent,
                References = compilerResult.CompiledPage.References
            };

            // Add an event log entry to the database.

            if (addEventLogItem)
            {
                var eventItem = Request.CreateEvent();
                eventItem.ObjectType = ObjectType.Page;
                eventItem.ObjectId = pageLayers.ContentPage.Id;
                eventItem.Project = result.Portal.Project;
                eventItem.Contact = contact;
                eventItem.AnonymId = result.AnonymId;
                eventItem.CustomUri = actionLink?.CustomUri;
                await _logManager.LogAsync(eventItem, portal.Owners, cancellationToken);
                result.Event = new EngineEventViewModel { Id = eventItem.Id };
            }

            Response.SetAnonymId(result.AnonymId, cookiePath);

            return result;
        }
    }
}
