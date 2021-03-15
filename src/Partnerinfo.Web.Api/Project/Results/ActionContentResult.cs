// Copyright (c) János Janka. All rights reserved.

using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Http;
using Partnerinfo.Project.Actions;
using Partnerinfo.Project.Models;

namespace Partnerinfo.Project.Results
{
    public sealed class ActionContentResult : IHttpActionResult
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ActionContentResult" /> class.
        /// </summary>
        public ActionContentResult(ApiController controller)
        {
            Controller = controller;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ActionContentResult" /> class.
        /// </summary>
        public ActionContentResult(IActionLinkService actionLinkService, ActionItem action, ApiController controller)
        {
            ActionLinkService = actionLinkService;
            Action = action;
            Controller = controller;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ActionContentResult" /> class.
        /// </summary>
        public ActionContentResult(HttpStatusCode statusCode, IActionLinkService actionLinkService, ActionItem action, ApiController controller)
        {
            StatusCode = statusCode;
            ActionLinkService = actionLinkService;
            Action = action;
            Controller = controller;
        }

        /// <summary>
        /// HTTP Status Code
        /// </summary>
        public HttpStatusCode StatusCode { get; } = HttpStatusCode.OK;

        /// <summary>
        /// Project manager
        /// </summary>
        public IActionLinkService ActionLinkService { get; }

        /// <summary>
        /// Action result
        /// </summary>
        public ActionItem Action { get; }

        /// <summary>
        /// API Controller
        /// </summary>
        public ApiController Controller { get; }

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
            return Task.FromResult(Request.CreateResponse(StatusCode, Map(ActionLinkService, Action)));
        }

        /// <summary>
        /// Maps an action to a result model object.
        /// </summary>
        /// <param name="action">The action to map.</param>
        /// <returns>
        /// The mapped action.
        /// </returns>
        internal static ActionResultDto Map(IActionLinkService actionLinkService, ActionItem action)
        {
            var model = new ActionResultDto
            {
                Id = action.Id,
                Project = action.Project,
                Type = action.Type,
                Enabled = action.Enabled,
                ModifiedDate = action.ModifiedDate,
                Name = action.Name,
                Options = action.Options,
                Children = action.Children.OfType<ActionItem>().Select(a => Map(actionLinkService, a)).ToList()
            };

            if (action.Parent == null)
            {
                model.Link = actionLinkService.CreateLink(new ActionLink(action.Id), true);
            }

            return model;
        }
    }
}