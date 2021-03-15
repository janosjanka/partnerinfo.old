// Copyright (c) János Janka. All rights reserved.

using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Http;
using Partnerinfo.Project.Actions;
using Partnerinfo.Project.Models;

namespace Partnerinfo.Project.Results
{
    public sealed class ActionLinkContentResult : IHttpActionResult
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ActionLinkContentResult" /> class.
        /// </summary>
        public ActionLinkContentResult(ApiController controller)
        {
            Controller = controller;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ActionLinkContentResult" /> class.
        /// </summary>
        public ActionLinkContentResult(IActionLinkService actionLinkService, ActionItem action, ContactItem contact, string customUri, ApiController controller)
        {
            ActionLinkService = actionLinkService;
            Action = action;
            Contact = contact;
            CustomUri = customUri;
            Controller = controller;
        }

        /// <summary>
        /// Project manager
        /// </summary>
        public IActionLinkService ActionLinkService { get; set; }

        /// <summary>
        /// Action
        /// </summary>
        public ActionItem Action { get; private set; }

        /// <summary>
        /// Contact
        /// </summary>
        public ContactItem Contact { get; private set; }

        /// <summary>
        /// Custom URI
        /// </summary>
        public string CustomUri { get; set; }

        /// <summary>
        /// API Controller
        /// </summary>
        public ApiController Controller { get; private set; }

        /// <summary>
        /// Request message
        /// </summary>
        public HttpRequestMessage Request => Controller.Request;

        /// <summary>
        /// Creates an <see cref="T:System.Net.Http.HttpResponseMessage" /> asynchronously.
        /// </summary>
        /// <param name="cancellationToken">The token to monitor for cancellation requests.</param>
        /// <returns>
        /// A task that, when completed, contains the <see cref="T:System.Net.Http.HttpResponseMessage" />.
        /// </returns>
        public Task<HttpResponseMessage> ExecuteAsync(CancellationToken cancellationToken)
        {
            return Task.FromResult(Request.CreateResponse(HttpStatusCode.OK,
                new ActionLinkResultDto
                {
                    Action = ActionContentResult.Map(ActionLinkService, Action),
                    Contact = Contact == null ? null : new AccountItem
                    {
                        Id = Contact.Id,
                        Email = Contact.Email,
                        FirstName = Contact.FirstName,
                        LastName = Contact.LastName,
                        NickName = Contact.NickName,
                        Gender = Contact.Gender,
                        Birthday = Contact.Birthday
                    },
                    CustomUri = CustomUri,
                    Link = ActionLinkService.CreateLink(new ActionLink(Action.Id, Contact?.Id, CustomUri), true)
                }));
        }
    }
}