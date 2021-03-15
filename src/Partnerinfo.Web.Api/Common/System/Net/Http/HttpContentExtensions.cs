// Copyright (c) János Janka. All rights reserved.

using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Http;

namespace System.Net.Http
{
    internal static class HttpContentExtensions
    {
        /// <summary>
        /// Reads a file stream asynchronously.
        /// </summary>
        /// <param name="request">The request.</param>
        /// <returns>A key/value pair that contains the name of the file with a stream value.</returns>
        /// <exception cref="System.Web.Http.HttpResponseException">
        /// </exception>
        public static async Task<FileStreamContent> ReadAsFileStreamAsync(this HttpContent content, CancellationToken cancellationToken)
        {
            if (content.IsMimeMultipartContent("form-data"))
            {
                content = (await content.ReadAsMultipartAsync(cancellationToken)).Contents.FirstOrDefault();
            }
            if (content == null || string.IsNullOrEmpty(content.Headers.ContentDisposition.FileName))
            {
                throw new HttpResponseException(HttpStatusCode.BadRequest);
            }
            return new FileStreamContent(
                content.Headers.ContentDisposition.FileName.Replace("\"", string.Empty),
                await content.ReadAsStreamAsync());
        }
    }
}