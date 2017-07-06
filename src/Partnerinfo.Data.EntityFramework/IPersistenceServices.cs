// Copyright (c) János Janka. All rights reserved.

using System.Threading;
using System.Threading.Tasks;
using Partnerinfo.Drive;
using Partnerinfo.Media.EntityFramework;

namespace Partnerinfo
{
    /// <summary>
    /// Maintains a list of objects affected by a business transaction and coordinates the writing out
    /// of changes and the resolution of concurrency problems.
    /// </summary>
    public interface IPersistenceServices
    {
        /// <summary>
        /// Gets a store implementation that manages drive folders, files, and other related data.
        /// </summary>
        /// <value>
        /// The drive store implementation.
        /// </value>
        IFileStore Drive { get; }

        /// <summary>
        /// Gets a store implementation that manages media playlists, playlist items, and other related data.
        /// </summary>
        /// <value>
        /// The media store implementation.
        /// </value>
        IMediaStore Media { get; }

        /// <summary>
        /// Asynchronously saves all changes made in this context to the underlying database.
        /// </summary>
        /// <param name="cancellationToken">A <see cref="System.Threading.CancellationToken" /> to observe while waiting for the task to complete.</param>
        /// <returns>
        /// A task that represents the asynchronous save operation.
        /// </returns>
        Task SaveAsync(CancellationToken cancellationToken);
    }
}
