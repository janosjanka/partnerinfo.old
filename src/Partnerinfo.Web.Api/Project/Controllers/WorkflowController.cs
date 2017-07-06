// Copyright (c) János Janka. All rights reserved.

using System;
using System.Linq;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;
using Partnerinfo.Logging;
using Partnerinfo.Project.Actions;
using Partnerinfo.Project.ModelBinders;
using Partnerinfo.Project.Models;
using Partnerinfo.Project.Results;
using Partnerinfo.Security;

namespace Partnerinfo.Project.Controllers
{
    /// <summary>
    /// Provides methods that respond to HTTP requests that are made to an ASP.NET Web API.
    /// </summary>
    public sealed class WorkflowController : ApiController
    {
        private readonly SecurityManager _securityManager;
        private readonly ProjectManager _projectManager;
        private readonly WorkflowInvoker _workflowInvoker;
        private readonly IActionLinkService _actionLinkService;

        /// <summary>
        /// Initializes a new instance of the <see cref="WorkflowController" /> class.
        /// </summary>
        public WorkflowController(
            SecurityManager securityManager,
            ProjectManager projectManager,
            WorkflowInvoker workflowInvoker,
            IActionLinkService actionLinkService)
        {
            _securityManager = securityManager;
            _projectManager = projectManager;
            _workflowInvoker = workflowInvoker;
            _actionLinkService = actionLinkService;
        }

        /// <summary>
        /// Represents an action that handles only HTTP GET requests.
        /// </summary>
        [Route("a.{paramUri}/{customUri?}")]
        public async Task<IHttpActionResult> GetAsync(
            [FromUri(BinderType = typeof(ActionLinkModelBinder))] ActionLink linkParams,
            [FromUri(BinderType = typeof(AuthTicketModelBinder))] AuthTicket authTicket,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            if (!ModelState.IsValid)
            {
                return StatusCode(HttpStatusCode.Forbidden);
            }
            var contact = await GetContactAsync(linkParams, authTicket, cancellationToken);
            return await ExecuteActionAsync(linkParams, authTicket, contact, null, cancellationToken);
        }

        /// <summary>
        /// Represents an action that handles only HTTP GET requests.
        /// </summary>
        [Route("a/{args}/{salt?}")]
        public async Task<IHttpActionResult> GetAsync(
            [FromUri(BinderType = typeof(ActionEventModelBinder))] ActionEventArgs args,
            [FromUri(BinderType = typeof(AuthTicketModelBinder))] AuthTicket authTicket,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            if (args == null || !ModelState.IsValid)
            {
                return StatusCode(HttpStatusCode.Forbidden);
            }
            var action = await _projectManager.GetActionByLinkIdAsync(args.TargetId, cancellationToken);
            if (action == null)
            {
                return NotFound();
            }
            return await GetAsync(new ActionLink(action.Id, args.ContactId, args.Salt), authTicket, cancellationToken);
        }

        /// <summary>
        /// Represents an action that handles only HTTP GET requests.
        /// </summary>
        [Route("a.{paramUri}/{customUri?}")]
        public async Task<IHttpActionResult> PostAsync(
            [FromUri(BinderType = typeof(ActionLinkModelBinder))] ActionLink linkParams,
            [FromUri(BinderType = typeof(AuthTicketModelBinder))] AuthTicket authTicket,
            [FromBody] WorkflowDto model,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            if (!ModelState.IsValid)
            {
                return StatusCode(HttpStatusCode.Forbidden);
            }

            ContactItem contact;

            if (model == null)
            {
                contact = await GetContactAsync(linkParams, authTicket, cancellationToken);
                return await ExecuteActionAsync(linkParams, authTicket, contact, null, cancellationToken);
            }
            if (model.Name != null)
            {
                linkParams.CustomUri = UriUtility.Normalize(model.Name);
            }
            if (model.Contact != null)
            {
                contact = model.Contact.ToContact();
            }
            else
            {
                contact = await GetContactAsync(linkParams, authTicket, cancellationToken);
            }

            var result = await ExecuteActionAsync(linkParams, authTicket, contact, null, cancellationToken);
            if (model.Invitation != null)
            {
                await InviteAsync(linkParams, authTicket, contact, model.Invitation, cancellationToken);
            }
            return result;
        }

        /// <summary>
        /// Represents an action that handles only HTTP POST requests.
        /// </summary>
        [Route("a/{args}/{salt?}")]
        public async Task<IHttpActionResult> PostAsync(
            [FromUri(BinderType = typeof(ActionEventModelBinder))] ActionEventArgs args,
            [FromUri(BinderType = typeof(AuthTicketModelBinder))] AuthTicket authTicket,
            [FromBody] WorkflowDto model,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            if (args == null || !ModelState.IsValid)
            {
                return StatusCode(HttpStatusCode.Forbidden);
            }
            var action = await _projectManager.GetActionByLinkIdAsync(args.TargetId, cancellationToken);
            if (action == null)
            {
                return NotFound();
            }
            return await PostAsync(new ActionLink { ActionId = action.Id, ContactId = args.ContactId, CustomUri = args.Salt }, authTicket, model, cancellationToken);
        }

        /// <summary>
        /// Chooses a contact by ticket or ID
        /// </summary>
        private async Task<ContactItem> GetContactAsync(ActionLink actionLink, AuthTicket authTicket, CancellationToken cancellationToken)
        {
            ContactItem contact = null;

            if (authTicket != null)
            {
                contact = await _projectManager.GetContactByIdAsync(
                    authTicket.Id, ContactField.Project | ContactField.Sponsor, cancellationToken);
            }

            if (contact == null && actionLink.ContactId != null)
            {
                contact = await _projectManager.GetContactByIdAsync(
                    (int)actionLink.ContactId, ContactField.Project | ContactField.Sponsor, cancellationToken);
            }

            return contact;
        }

        /// <summary>
        /// Executes the given action
        /// </summary>
        private async Task<IHttpActionResult> ExecuteActionAsync(
            ActionLink actionLink,
            AuthTicket authTicket,
            ContactItem contact,
            PropertyDictionary properties,
            CancellationToken cancellationToken)
        {
            var action = await _projectManager.GetActionByIdAsync(actionLink.ActionId, cancellationToken);
            if (action == null)
            {
                return NotFound();
            }
            var project = await _projectManager.FindByIdAsync(action.Project.Id, cancellationToken);
            var users = await _securityManager.GetAccessRulesAsync(project, cancellationToken);
            var context = Request.Properties["MS_HttpContext"] as HttpContextWrapper;
            var eventItem = context.Request.CreateEvent();
            eventItem.CustomUri = actionLink.CustomUri;
            var result = await _workflowInvoker.InvokeAsync(
                new ActionActivityContext(
                    project,
                    action,
                    authTicket,
                    contact,
                    ObjectState.Unchanged,
                    properties,
                    eventItem),
                cancellationToken);
            if (result == null)
            {
                return new WorkflowContentResult(this);
            }
            if (result.StatusCode == ActionActivityStatusCode.Forbidden)
            {
                return StatusCode(HttpStatusCode.Forbidden);
            }
            return new WorkflowContentResult(result, this);
        }

        /// <summary>
        /// Asynchronously sends invitation to each of the users specified.
        /// </summary>
        private async Task InviteAsync(
            ActionLink actionLink,
            AuthTicket authTicket,
            ContactItem contact,
            WorkflowInvitationDto model,
            CancellationToken cancellationToken)
        {
            if (actionLink == null)
            {
                throw new ArgumentNullException(nameof(actionLink));
            }
            if (model == null)
            {
                throw new ArgumentNullException(nameof(model));
            }
            if (model.Action == null)
            {
                throw new InvalidOperationException("An action link with the given ID was not found.");
            }
            if (model.To == null || model.To.Length == 0)
            {
                throw new InvalidOperationException("At least one person is required for invitation.");
            }
            var invActionLinkParams = _actionLinkService.DecodeLink(model.Action);

            var action = await _projectManager.GetActionByIdAsync(invActionLinkParams.ActionId, cancellationToken);
            if (action == null)
            {
                throw new InvalidOperationException($"The specified action link was not found: {invActionLinkParams.ToString()}");
            }

            var project = await _projectManager.FindByIdAsync(action.Project.Id, cancellationToken);
            var context = Request.Properties["MS_HttpContext"] as HttpContextWrapper;
            var logEvent = context.Request.CreateEvent();
            var invitation = new ProjectInvitation { From = contact, Message = model.Message, To = model.To.Select(c => c.ToContact()) };
            var properties = new PropertyDictionary { { "Invitation", invitation } };
            foreach (var to in invitation.To.Where(to => to != null))
            {
                var eventItem = new EventItem
                {
                    AnonymId = logEvent.AnonymId,
                    Project = project,
                    ClientId = logEvent.ClientId,
                    CustomUri = actionLink.CustomUri,
                    BrowserBrand = logEvent.BrowserBrand,
                    BrowserVersion = logEvent.BrowserVersion,
                    MobileDevice = logEvent.MobileDevice,
                    ReferrerUrl = logEvent.ReferrerUrl
                };
                await _workflowInvoker.InvokeAsync(new ActionActivityContext(
                    project: project,
                    action: action,
                    authTicket: authTicket,
                    contact: to,
                    contactState: ObjectState.Unchanged,
                    properties: properties,
                    eventItem: eventItem), cancellationToken);
            }
        }
    }
}