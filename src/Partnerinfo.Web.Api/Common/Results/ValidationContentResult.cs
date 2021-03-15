// Copyright (c) János Janka. All rights reserved.

using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.ModelBinding;

namespace Partnerinfo.Results
{
    public class ValidationContentResult : IHttpActionResult
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ValidationContentResult" /> class.
        /// </summary>
        /// <param name="result">The operation result.</param>
        /// <param name="controller">The controller.</param>
        public ValidationContentResult(ValidationResult result, ApiController controller)
        {
            Result = result;
            Controller = controller;
        }

        /// <summary>
        /// Gets the <see cref="ValidationResult" />.
        /// </summary>
        /// <value>
        /// The <see cref="ValidationResult" />.
        /// </value>
        public ValidationResult Result { get; }

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
        /// Gets the model state after the model binding process.
        /// </summary>
        /// <value>
        /// The model state after the model binding process.
        /// </value>
        public ModelStateDictionary ModelState => Controller.ModelState;

        /// <summary>
        /// Creates an <see cref="T:System.Net.Http.HttpResponseMessage" /> asynchronously.
        /// </summary>
        /// <param name="cancellationToken">The token to monitor for cancellation requests.</param>
        /// <returns>
        /// A task that, when completed, contains the <see cref="T:System.Net.Http.HttpResponseMessage" />.
        /// </returns>
        public Task<HttpResponseMessage> ExecuteAsync(CancellationToken cancellationToken)
        {
            if (Result.Succeeded)
            {
                return Task.FromResult(Request.CreateResponse(HttpStatusCode.OK));
            }

            foreach (var error in Result.Errors)
            {
                ModelState.AddModelError("", error);
            }

            return Task.FromResult(Request.CreateErrorResponse(HttpStatusCode.BadRequest, new HttpError(ModelState, false)));
        }
    }
}