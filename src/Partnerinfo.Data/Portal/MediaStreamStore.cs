// Copyright (c) János Janka. All rights reserved.

using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace Partnerinfo.Portal
{
    public class MediaStreamStore : IMediaStreamStore
    {
        public static readonly MediaStreamStore Default = new MediaStreamStore();

        /// <summary>
        /// Saves the media stream for the specified <paramref name="portal" />.
        /// </summary>
        /// <param name="portal">The portal which owns the media stream.</param>
        /// <param name="media">The media which contains information on the stream.</param>
        /// <param name="mediaStream">The media stream to save.</param>
        /// <param name="cancellationToken">A token to observe while waiting for the task to complete.</param>
        /// <returns>
        /// A task that represents the asynchronous operation.
        /// </returns>
        public virtual async Task SaveMediaStreamAsync(PortalItem portal, MediaItem media, Stream mediaStream, CancellationToken cancellationToken)
        {
            if (portal == null)
            {
                throw new ArgumentNullException(nameof(portal));
            }
            if (media == null)
            {
                throw new ArgumentNullException(nameof(media));
            }
            if (mediaStream == null)
            {
                throw new ArgumentNullException(nameof(mediaStream));
            }

            var directory = ServerPaths.Map(ServerPaths.PortalMedia, portal.Uri);
            if (!Directory.Exists(directory))
            {
                Directory.CreateDirectory(directory);
            }

            using (var fileStream = File.Create(Path.Combine(directory, media.Uri), 4096))
            {
                await mediaStream.CopyToAsync(fileStream);
            }
        }

        /// <summary>
        /// Reads the media stream for the specified <paramref name="portal" />.
        /// </summary>
        /// <param name="portal">The portal which owns the media stream.</param>
        /// <param name="media">The media which contains information on the stream.</param>
        /// <param name="cancellationToken">A token to observe while waiting for the task to complete.</param>
        /// <returns>
        /// A task that represents the asynchronous operation.
        /// </returns>
        public virtual Task<Stream> ReadMediaStreamAsync(PortalItem portal, MediaItem media, CancellationToken cancellationToken)
        {
            if (portal == null)
            {
                throw new ArgumentNullException(nameof(portal));
            }
            if (media == null)
            {
                throw new ArgumentNullException(nameof(media));
            }

            var directory = ServerPaths.Map(ServerPaths.PortalMedia, portal.Uri, media.Uri);

            return Task.FromResult<Stream>(File.OpenRead(Path.Combine(directory, media.Name)));
        }

        /// <summary>
        /// Deletes a media <see cref="Stream" /> from the specified <paramref name="portal" /> as an asynchronous operation.
        /// </summary>
        /// <param name="portal">The portal which owns the media stream.</param>
        /// <param name="media">The media which contains information on the stream.</param>
        /// <param name="cancellationToken">A token to observe while waiting for the task to complete.</param>
        /// <returns>
        /// A task that represents the asynchronous operation.
        /// </returns>
        public virtual Task DeleteMediaStreamAsync(PortalItem portal, MediaItem media, CancellationToken cancellationToken)
        {
            if (portal == null)
            {
                throw new ArgumentNullException(nameof(portal));
            }
            if (media == null)
            {
                throw new ArgumentNullException(nameof(media));
            }

            var fileName = ServerPaths.Map(ServerPaths.PortalMedia, portal.Uri, media.Uri);

            if (File.Exists(fileName))
            {
                File.Delete(fileName);
            }

            return Task.CompletedTask;
        }

        /// <summary>
        /// Gets the absolute URL for a media by its <paramref name="uri" />.
        /// </summary>
        /// <param name="portalUri">The portal URI.</param>
        /// <param name="mediaUri">The media URI.</param>
        /// <returns>
        /// The absolute URL for a media.
        /// </returns>
        public virtual string GetMediaLinkByUri(string portalUri, string mediaUri)
        {
            if (portalUri == null)
            {
                throw new ArgumentNullException(nameof(portalUri));
            }
            if (mediaUri == null)
            {
                throw new ArgumentNullException(nameof(mediaUri));
            }
            return ServerPaths.Uri(ServerPaths.PortalMedia, portalUri, mediaUri).AbsoluteUri;
        }
    }
}
