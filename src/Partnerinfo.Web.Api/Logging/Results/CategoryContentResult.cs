// Copyright (c) János Janka. All rights reserved.

using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Http;
using Partnerinfo.Logging.Models;

namespace Partnerinfo.Logging.Results
{
    public sealed class CategoryContentResult : IHttpActionResult
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="CategoryContentResult" /> class.
        /// </summary>
        public CategoryContentResult(ApiController controller)
        {
            Controller = controller;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="CategoryContentResult" /> class.
        /// </summary>
        public CategoryContentResult(CategoryItem category, ApiController controller)
        {
            Category = category;
            Controller = controller;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="CategoryContentResult" /> class.
        /// </summary>
        public CategoryContentResult(HttpStatusCode statusCode, CategoryItem category, ApiController controller)
        {
            StatusCode = statusCode;
            Category = category;
            Controller = controller;
        }

        /// <summary>
        /// HTTP Status Code
        /// </summary>
        public HttpStatusCode StatusCode { get; private set; } = HttpStatusCode.OK;

        /// <summary>
        /// Action result
        /// </summary>
        public CategoryItem Category { get; private set; }

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
            return Task.FromResult(Request.CreateResponse(StatusCode, new CategoryResultDto
            {
                Id = Category.Id,
                Name = Category.Name,
                Color = Category.Color
            }));
        }
    }
}