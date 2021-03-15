// Copyright (c) János Janka. All rights reserved.

using Partnerinfo.Project.Models;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Http;

namespace Partnerinfo.Project.Results
{
    public sealed class BusinessTagContentResult : IHttpActionResult
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="BusinessTagContentResult" /> class.
        /// </summary>
        public BusinessTagContentResult(HttpStatusCode statusCode, ApiController controller)
        {
            StatusCode = statusCode;
            Controller = controller;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="BusinessTagContentResult" /> class.
        /// </summary>
        public BusinessTagContentResult(HttpStatusCode statusCode, BusinessTagItem businessTagInfo, ApiController controller)
        {
            StatusCode = statusCode;
            BusinessTagInfo = businessTagInfo;
            Controller = controller;
        }

        /// <summary>
        /// HTTP Status Code
        /// </summary>
        public HttpStatusCode StatusCode { get; private set; } = HttpStatusCode.OK;

        /// <summary>
        /// Project manager
        /// </summary>
        public ProjectManager ProjectManager { get; set; }

        /// <summary>
        /// Action result
        /// </summary>
        public BusinessTagItem BusinessTagInfo { get; private set; }

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
            if (BusinessTagInfo == null)
            {
                return Task.FromResult(Request.CreateResponse(HttpStatusCode.NoContent));
            }

            return Task.FromResult(Request.CreateResponse(StatusCode, new BusinessTagResultDto
            {
                Id = BusinessTagInfo.Id,
                Name = BusinessTagInfo.Name,
                Color = BusinessTagInfo.Color
            }));
        }
    }
}