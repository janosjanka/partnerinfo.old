// Copyright (c) János Janka. All rights reserved.

using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Http;
using Partnerinfo.Logging.Models;

namespace Partnerinfo.Logging.Results
{
    /// <summary>
    /// Represents a <see cref="RuleItem" /> that will be mapped to a <see cref="RuleResultDto" />.
    /// </summary>
    public sealed class RuleListContentResult : IHttpActionResult
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="RuleListContentResult" /> class.
        /// </summary>
        public RuleListContentResult(ApiController controller)
        {
            Controller = controller;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="RuleListContentResult" /> class.
        /// </summary>
        public RuleListContentResult(ListResult<RuleItem> rules, ApiController controller)
        {
            Rules = rules;
            Controller = controller;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="RuleListContentResult" /> class.
        /// </summary>
        public RuleListContentResult(HttpStatusCode statusCode, ListResult<RuleItem> rules, ApiController controller)
        {
            StatusCode = statusCode;
            Rules = rules;
            Controller = controller;
        }

        /// <summary>
        /// HTTP Status Code
        /// </summary>
        public HttpStatusCode StatusCode { get; private set; } = HttpStatusCode.OK;

        /// <summary>
        /// Action result
        /// </summary>
        public ListResult<RuleItem> Rules { get; private set; }

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
            // Add HTTP headers and hypermedia links to the items here.
            return Task.FromResult(Request.CreateResponse(
                StatusCode,
                ListResult.Create(Rules.Data.Select(r => ModelMapper.ToRuleResultDto(r)), Rules.Total)));
        }
    }
}