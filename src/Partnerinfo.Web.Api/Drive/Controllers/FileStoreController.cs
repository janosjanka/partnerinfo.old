// Copyright (c) János Janka. All rights reserved.

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;
using System.Web.Routing;
using Partnerinfo.Drive.Models;

namespace Partnerinfo.Drive.Controllers
{
    /// <summary>
    /// Provides methods that respond to HTTP requests that are made to an ASP.NET Web API.
    /// </summary>
    [RoutePrefix("filestore")]
    public class FileStoreController : ApiController
    {
        /// <summary>
        /// The root name for <see cref="GetByPublicLink" />.
        /// </summary>
        internal const string PublicRootName = "Drive.FileStore.GetByPath";

        /// <summary>
        /// The root name for <see cref="GetByPrivateLink" />.
        /// </summary>
        internal const string PrivateRootName = "Drive.FileStore.GetByUri";

        /// <summary>
        /// Represents the content root for files.
        /// </summary>
        private static readonly string s_contentRoot = ServerPaths.Map(ServerPaths.DriveFiles);

        /// <summary>
        /// An unit of services that maintain a list of objects affected by a business transaction.
        /// </summary>
        private readonly IPersistenceServices _services;

        /// <summary>
        /// Initializes a new instance of the <see cref="FileStoreController" /> class.
        /// </summary>
        /// <param name="services">Maintains a list of objects affected by a business transaction.</param>
        public FileStoreController(IPersistenceServices services)
        {
            _services = services;
        }

        /// <summary>
        /// Represents an action that handles only HTTP GET requests.
        /// </summary>
        /// <param name="userId">The user identifier.</param>
        /// <param name="path">The path.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns></returns>
        [Route("{userId:int}/{*path}", Name = PublicRootName)]
        public HttpResponseMessage GetByPublicLink(int userId, string path)
        {
            var file = new DriveFile(s_contentRoot, userId, path);
            if (!file.Exists)
            {
                return Request.CreateResponse(HttpStatusCode.NotFound);
            }
            if (!Request.IsModifiedSince(file.ModifiedDate))
            {
                return Request.CreateResponse(HttpStatusCode.NotModified);
            }
            try
            {
                Stream stream;
                return new HttpResponseMessage(HttpStatusCode.OK)
                {
                    Headers = { CacheControl = new CacheControlHeaderValue { Public = true } },
                    Content = new StreamContent(stream = file.OpenRead())
                    {
                        Headers =
                        {
                            Expires = DateTime.UtcNow.AddMonths(3),
                            LastModified = file.ModifiedDate,
                            ContentType = new MediaTypeHeaderValue(MimeMapping.GetMimeMapping(path)),
                            ContentDisposition = new ContentDispositionHeaderValue("inline") { FileName = path, Size = stream.Length }
                        }
                    }
                };
            }
            catch (IOException)
            {
                return Request.CreateResponse(HttpStatusCode.Conflict);
            }
        }

        /// <summary>
        /// Represents an action that handles only HTTP GET requests.
        /// </summary>
        [Route("{uri}/{path}", Name = PrivateRootName)]
        public async Task<HttpResponseMessage> GetByPrivateLink(string uri, string path, CancellationToken cancellationToken)
        {
            var file = await _services.Drive.FindByUriAsync(uri, cancellationToken);
            if (file == null)
            {
                return Request.CreateResponse(HttpStatusCode.NotFound);
            }

            return GetByPublicLink(file.OwnerId, file.PhysicalPath);
        }

        /// <summary>
        /// Represents an action that handles only HTTP POST requests.
        /// </summary>
        /// <param name="parentId">The parent identifier.</param>
        /// <param name="mail">The mail.</param>
        /// <param name="extract">if set to <c>true</c> [extract].</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns></returns>
        [Route("{parentId?}")]
        public async Task<IHttpActionResult> PostAsync(string parentId = null, string mail = null, bool extract = false, CancellationToken cancellationToken = default(CancellationToken))
        {
            int userId = ApiSecurity.CurrentUserId;
            var files = new List<FileItem>();
            var parent = await FindFileAsync(parentId, cancellationToken);
            var parenti = parent == null ? default(int?) : parent.Id;
            var manager = new DriveManager(ApiSecurity.Manager, _services, ServerPaths.Map(ServerPaths.DriveFiles));

            using (var content = await Request.Content.ReadAsFileStreamAsync(cancellationToken))
            {
                files.Add(await manager.CreateFromStreamAsync(content.FileStream, parenti, userId, content.FileName, cancellationToken));
            }

            return Ok(ListResult.Create(files.Select(file => new FileStoreUploadResult
            {
                Name = file.Name,
                Link = new Uri(Request.RequestUri, Url.Route(PublicRootName, new RouteValueDictionary
                {
                    { "userId", file.OwnerId },
                    { "path", file.PhysicalPath }
                }))
                .AbsoluteUri
            })));
        }

        /// <summary>
        /// Finds a file by ID
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns></returns>
        private Task<FileItem> FindFileAsync(string id, CancellationToken cancellationToken)
        {
            if (id == null)
            {
                return Task.FromResult<FileItem>(null);
            }

            int fileId;
            if (int.TryParse(id, out fileId))
            {
                return _services.Drive.FindByIdAsync(fileId, cancellationToken);
            }

            return _services.Drive.FindByUriAsync(id, cancellationToken);
        }
    }
}