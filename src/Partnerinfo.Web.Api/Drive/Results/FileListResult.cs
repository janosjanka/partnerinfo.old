// Copyright (c) János Janka. All rights reserved.

using System.IO;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Routing;
using Partnerinfo.Drive.Controllers;

namespace Partnerinfo.Drive.Results
{
    public sealed class FileListResult : IHttpActionResult
    {
        /// <summary>
        /// Action result
        /// </summary>
        public ListResult<FileResult> Result { get; }

        /// <summary>
        /// API Controller
        /// </summary>
        public ApiController Controller { get; }

        /// <summary>
        /// Request message
        /// </summary>
        public HttpRequestMessage Request => Controller.Request;

        /// <summary>
        /// Initializes a new instance of the <see cref="FileListResult" /> class.
        /// </summary>
        /// <param name="list">The content.</param>
        /// <param name="controller">The controller.</param>
        public FileListResult(ApiController controller)
        {
            Controller = controller;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="FileListResult" /> class.
        /// </summary>
        /// <param name="result">The content.</param>
        /// <param name="controller">The controller.</param>
        public FileListResult(ListResult<FileResult> result, ApiController controller)
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
            var message = Request.CreateResponse(HttpStatusCode.OK, Result);

            var publicLink = new RouteValueDictionary();
            var privateLink = new RouteValueDictionary();
            foreach (var file in Result.Data)
            {
                if (file.Type != FileType.Folder)
                {
                    publicLink["userId"] = file.OwnerId;
                    publicLink["path"] = file.PhysicalPath;
                    file.PublicLink = Controller.Url.Link(FileStoreController.PublicRootName, publicLink).Replace("%5C", "/");

                    privateLink["uri"] = file.Slug;
                    privateLink["path"] = Path.GetFileName(file.PhysicalPath);
                    file.PrivateLink = Controller.Url.Link(FileStoreController.PrivateRootName, privateLink).Replace("%5C", "/");
                }
            }

            return Task.FromResult(message);
        }
    }
}