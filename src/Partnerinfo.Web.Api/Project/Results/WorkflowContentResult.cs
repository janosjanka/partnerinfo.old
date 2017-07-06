// Copyright (c) János Janka. All rights reserved.

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Http;
using Partnerinfo.Project.Actions;
using Partnerinfo.Project.Models;

namespace Partnerinfo.Project.Results
{
    public sealed class WorkflowContentResult : IHttpActionResult
    {
        /// <summary>
        /// Gets the <see cref="ActionActivityResult" />.
        /// </summary>
        /// <value>
        /// The <see cref="ActionActivityResult" />.
        /// </value>
        public ActionActivityResult Result { get; }

        /// <summary>
        /// Gets the <see cref="ApiController" />.
        /// </summary>
        /// <value>
        /// The <see cref="ApiController" />.
        /// </value>
        public ApiController Controller { get; }

        /// <summary>
        /// Gets the <see cref="HttpRequestMessage" /> of the current <see cref="System.Web.Http.ApiController" />.
        /// </summary>
        /// <value>
        /// The <see cref="HttpRequestMessage" /> of the current <see cref="System.Web.Http.ApiController" />.
        /// </value>
        public HttpRequestMessage Request => Controller.Request;

        /// <summary>
        /// Initializes a new instance of the <see cref="PagedListResult" /> class.
        /// </summary>
        /// <param name="controller">The controller.</param>
        public WorkflowContentResult(ApiController controller)
        {
            Controller = controller;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="PagedListResult" /> class.
        /// </summary>
        /// <param name="result">The result.</param>
        /// <param name="controller">The controller.</param>
        public WorkflowContentResult(ActionActivityResult result, ApiController controller)
        {
            Result = result;
            Controller = controller;
        }

        /// <summary>
        /// Creates an <see cref="T:System.Net.Http.HttpResponseMessage" /> asynchronously.
        /// </summary>
        /// <param name="cancellationToken">The token to monitor for cancellation requests.</param>
        /// <returns>
        /// A task that, when completed, contains the <see cref="T:System.Net.Http.HttpResponseMessage" />.
        /// </returns>
        public Task<HttpResponseMessage> ExecuteAsync(CancellationToken cancellationToken)
        {
            HttpResponseMessage message;

            if (Result == null)
            {
                message = Request.CreateResponse(HttpStatusCode.NoContent);
                AddAnonymHeader(message.Headers);
                return Task.FromResult(message);
            }

            if (Result.StatusCode == ActionActivityStatusCode.Forbidden)
            {
                message = Request.CreateResponse(HttpStatusCode.Forbidden);
                AddAnonymHeader(message.Headers);
                return Task.FromResult(message);
            }

            message = Request.CreateResponse(HttpStatusCode.OK, CreateActionResult(Result));

            if (Result.ReturnUrl != null)
            {
                if (!Request.Headers.IsAjaxRequest())
                {
                    message.StatusCode = HttpStatusCode.Redirect;
                }
                message.Headers.Location = new Uri(Result.ReturnUrl, UriKind.Absolute);
            }

            AddAnonymHeader(message.Headers);
            return Task.FromResult(message);
        }

        /// <summary>
        /// Gets the session ID from the request
        /// </summary>
        /// <returns></returns>
        private Guid? GetAnonymId()
        {
            Guid anonymId;
            IEnumerable<string> values;
            return Request.Headers.TryGetValues(ResourceKeys.AnonymIdCookieName, out values)
                && Guid.TryParse(values.FirstOrDefault(), out anonymId) ? anonymId : default(Guid?);
        }

        /// <summary>
        /// Creates a new response message
        /// </summary>
        /// <param name="headers">The headers.</param>
        private void AddAnonymHeader(HttpResponseHeaders headers)
        {
            Debug.Assert(headers != null);

            Guid? anonymId;

            if (Result == null)
            {
                anonymId = GetAnonymId();
            }
            else
            {
                anonymId = Result.AnonymId;
            }

            if (anonymId != null)
            {
                headers.Add(ResourceKeys.AnonymIdCookieName, anonymId.Value.ToString());
            }
        }

        /// <summary>
        /// Creates a result model with a bearer token
        /// </summary>
        /// <param name="result">The result.</param>
        /// <returns></returns>
        private WorkflowIdentityResultDto CreateActionResult(ActionActivityResult result)
        {
            Debug.Assert(result != null);

            if (result.Ticket != null)
            {
                return new WorkflowIdentityResultDto
                {
                    TokenType = "Bearer",
                    AccessToken = AuthUtility.Protect(result.Ticket),
                    Identity = result.Ticket
                };
            }

            if (Request.Headers.Authorization != null)
            {
                return new WorkflowIdentityResultDto
                {
                    TokenType = Request.Headers.Authorization.Scheme,
                    AccessToken = Request.Headers.Authorization.Parameter,
                    Identity = result.Ticket
                };
            }

            return null;
        }
    }
}