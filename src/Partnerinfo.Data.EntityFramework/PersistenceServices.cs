// Copyright (c) János Janka. All rights reserved.

using System;
using System.Threading;
using System.Threading.Tasks;
using Partnerinfo.Drive;
using Partnerinfo.Drive.EntityFramework;
using Partnerinfo.Media.EntityFramework;

namespace Partnerinfo
{
    /// <summary>
    /// Maintains a list of objects affected by a business transaction and coordinates the writing out
    /// of changes and the resolution of concurrency problems.
    /// </summary>
    public sealed class PersistenceServices : IPersistenceServices
    {
        private readonly PartnerDbContext _context;

        private IFileStore _driveStore;
        private IMediaStore _mediaStore;

        /// <summary>
        /// Gets a store implementation that manages drive folders, files, and other related data.
        /// </summary>
        /// <value>
        /// The drive store implementation.
        /// </value>
        public IFileStore Drive => _driveStore ?? (_driveStore = new FileStore(_context));

        /// <summary>
        /// Gets a store implementation that manages media playlists, playlist items, and other related data.
        /// </summary>
        /// <value>
        /// The media store implementation.
        /// </value>
        public IMediaStore Media => _mediaStore ?? (_mediaStore = new MediaStore(_context));

        /// <summary>
        /// Initializes a new instance of the <see cref="PersistenceServices" /> class.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <exception cref="System.ArgumentNullException">Thrown when a null reference is passed to the method.</exception>
        public PersistenceServices(PartnerDbContext context)
        {
            if (context == null)
            {
                throw new ArgumentNullException(nameof(context));
            }

            _context = context;
        }

        /// <summary>
        /// Asynchronously saves all changes made in this context to the underlying database.
        /// </summary>
        /// <param name="cancellationToken">A <see cref="System.Threading.CancellationToken" /> to observe while waiting for the task to complete.</param>
        /// <returns>
        /// A task that represents the asynchronous save operation.
        /// </returns>
        public Task SaveAsync(CancellationToken cancellationToken) => _context.SaveChangesAsync(cancellationToken);
    }
}
