// Copyright (c) János Janka. All rights reserved.

using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace Partnerinfo.Portal
{
    public interface IMediaStreamStore
    {
        /// <summary>
        /// Saves a media <see cref="Stream" /> for the specified <paramref name="portal" /> as an asynchronous operation.
        /// </summary>
        /// <param name="portal">The portal which owns the media stream.</param>
        /// <param name="media">The media which contains information on the stream.</param>
        /// <param name="mediaStream">The media stream to save.</param>
        /// <param name="cancellationToken">A token to observe while waiting for the task to complete.</param>
        /// <returns>
        /// A task that represents the asynchronous operation.
        /// </returns>
        Task SaveMediaStreamAsync(PortalItem portal, MediaItem media, Stream mediaStream, CancellationToken cancellationToken);

        /// <summary>
        /// Reads a media <see cref="Stream" /> for the specified <paramref name="portal" /> as an asynchronous operation.
        /// </summary>
        /// <param name="portal">The portal which owns the media stream.</param>
        /// <param name="media">The media which contains information on the stream.</param>
        /// <param name="cancellationToken">A token to observe while waiting for the task to complete.</param>
        /// <returns>
        /// A task that represents the asynchronous operation.
        /// </returns>
        Task<Stream> ReadMediaStreamAsync(PortalItem portal, MediaItem media, CancellationToken cancellationToken);

        /// <summary>
        /// Deletes a media <see cref="Stream" /> from the specified <paramref name="portal" /> as an asynchronous operation.
        /// </summary>
        /// <param name="portal">The portal which owns the media stream.</param>
        /// <param name="media">The media which contains information on the stream.</param>
        /// <param name="cancellationToken">A token to observe while waiting for the task to complete.</param>
        /// <returns>
        /// A task that represents the asynchronous operation.
        /// </returns>
        Task DeleteMediaStreamAsync(PortalItem portal, MediaItem media, CancellationToken cancellationToken);

        /// <summary>
        /// Gets the absolute URL for a media by its <paramref name="uri" />.
        /// </summary>
        /// <param name="portalUri">The portal URI.</param>
        /// <param name="mediaUri">The media URI.</param>
        /// <returns>
        /// The absolute URL for a media.
        /// </returns>
        string GetMediaLinkByUri(string portalUri, string mediaUri);
    }
}
