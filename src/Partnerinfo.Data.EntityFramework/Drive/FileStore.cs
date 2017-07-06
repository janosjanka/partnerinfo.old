// Copyright (c) János Janka. All rights reserved.

using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Threading;
using System.Threading.Tasks;

namespace Partnerinfo.Drive.EntityFramework
{
    /// <summary>
    /// Provides facilities for querying and working with system data as objects.
    /// </summary>
    public class FileStore : IFileStore
    {
        private bool _disposed;

        /// <summary>
        /// Initializes a new instance of the <see cref="ContactStore" /> class.
        /// </summary>
        /// <param name="context">The context.</param>
        public FileStore(PartnerDbContext context)
        {
            if (context == null)
            {
                throw new ArgumentNullException(nameof(context));
            }

            Context = context;
        }

        /// <summary>
        /// If true will call SaveChanges after Create/Update/Delete.
        /// </summary>
        public bool AutoSaveChanges { get; set; } = true;

        /// <summary>
        /// Gets the context for the store.
        /// </summary>
        public PartnerDbContext Context { get; private set; }

        /// <summary>
        /// Finds a collection of projects with the given values.
        /// </summary>
        /// <param name="userId">The user identifier for the item to be found.</param>
        /// <param name="parentId">The unique document identifier from the data source for the document.</param>
        /// <param name="name">The name for the item to be found.</param>
        /// <param name="pageIndex">The index of the page of results to return. Use 1 to indicate the first page.</param>
        /// <param name="pageSize">The size of the page of results to return. <paramref name="pageIndex" /> is non-zero-based.</param>
        /// <param name="cancellationToken">A token to observe while waiting for the task to complete.</param>
        /// <returns>
        /// A task that represents the asynchronous operation.
        /// </returns>
        public virtual Task<ListResult<FileResult>> FindAllAsync(int ownerId, int? parentId, string name, int pageIndex, int pageSize, CancellationToken cancellationToken)
        {
            return Context.FindItemsAsync(DriveQueries.QueryAll(Context, ownerId, parentId, name), pageIndex, pageSize, cancellationToken);
        }

        /// <summary>
        /// Gets a collection of all the documents in the database.
        /// </summary>
        /// <param name="ids">A set of unique document identifiers.</param>
        /// <param name="cancellationToken">A token to observe while waiting for the task to complete.</param>
        /// <returns>
        /// Returns the information for all documents for an application as a collection of <see cref="FileItem" /> objects.
        /// </returns>
        public virtual Task<IList<FileItem>> FindAllAsync(IEnumerable<int> ids, CancellationToken cancellationToken)
        {
            return Context.FindItemsAsync(DriveQueries.QueryAllByIds(Context, ids), cancellationToken);
        }

        /// <summary>
        /// Gets the information from the data source for the document associated with the specified unique identifier.
        /// </summary>
        /// <param name="id">The unique identifier from the data source for the document.</param>
        /// <param name="cancellationToken">A token to observe while waiting for the task to complete.</param>
        /// <returns>
        /// Returns the document associated with the specified unique identifier.
        /// </returns>
        public virtual Task<FileItem> FindByIdAsync(int id, CancellationToken cancellationToken)
        {
            return Files.FindAsync(cancellationToken, id);
        }

        /// <summary>
        /// Gets the information from the data source for the document associated with the specified unique identifier.
        /// </summary>
        /// <param name="uri">The unique identifier from the data source for the document.</param>
        /// <param name="cancellationToken">A token to observe while waiting for the task to complete.</param>
        /// <returns>
        /// Returns the document associated with the specified unique identifier.
        /// </returns>
        public virtual Task<FileItem> FindByUriAsync(string uri, CancellationToken cancellationToken)
        {
            return Files.FirstOrDefaultAsync(f => f.Slug == uri, cancellationToken);
        }

        /// <summary>
        /// Updates information about a document in the database.
        /// </summary>
        /// <param name="id">The unique identifier from the data source for the document.</param>
        /// <param name="name">The name of the document to update.</param>
        /// <param name="cancellationToken">A token to observe while waiting for the task to complete.</param>
        public virtual async Task SetNameAsync(int id, string name, CancellationToken cancellationToken)
        {
            var file = await FindByIdAsync(id, cancellationToken);
            if (file == null)
            {
                throw new InvalidOperationException("File was not found.");
            }

            file.Name = name;
            Context.Update(file);
        }

        /// <summary>
        /// Inserts a file.
        /// </summary>
        /// <param name="file">The file to insert.</param>
        /// <param name="cancellationToken">A token to observe while waiting for the task to complete.</param>
        /// <returns>
        /// A task that represents the asynchronous operation.
        /// </returns>
        public virtual Task<ValidationResult> CreateAsync(FileItem file, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            ThrowIfDisposed();
            if (file == null)
            {
                throw new ArgumentNullException(nameof(file));
            }

            Context.Add(file);
            return Task.FromResult(ValidationResult.Success);
        }

        /// <summary>
        /// Updates a site.
        /// </summary>
        /// <param name="file">The file to update.</param>
        /// <param name="cancellationToken">A token to observe while waiting for the task to complete.</param>
        /// <returns>
        /// A task that represents the asynchronous operation.
        /// </returns>
        public virtual async Task<ValidationResult> UpdateAsync(FileItem file, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            ThrowIfDisposed();
            if (file == null)
            {
                throw new ArgumentNullException(nameof(file));
            }
            var oldFile = await Files.FindAsync(cancellationToken, file.Id);
            if (oldFile == null)
            {
                throw new InvalidOperationException("File does not exist.");
            }

            Context.Patch(oldFile, file);
            await SaveChangesAsync(cancellationToken);
            return ValidationResult.Success;
        }

        /// <summary>
        /// Deletes a site.
        /// </summary>
        /// <param name="file">The file to delete.</param>
        /// <param name="cancellationToken">A token to observe while waiting for the task to complete.</param>
        /// <returns>
        /// A task that represents the asynchronous operation.
        /// </returns>
        public virtual async Task<ValidationResult> DeleteAsync(FileItem file, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            ThrowIfDisposed();
            if (file == null)
            {
                throw new ArgumentNullException(nameof(file));
            }

            await Context.Database.ExecuteSqlCommandAsync("Drive.DeleteFile @Id", cancellationToken, DbParameters.Value("Id", file.Id));
            return ValidationResult.Success;
        }

        /// <summary>
        /// The DbSet for access to files in the context.
        /// </summary>
        /// <value>
        /// The DbSet.
        /// </value>
        private DbSet<FileItem> Files => Context.Set<FileItem>();

        /// <summary>
        /// Throws a <see cref="ObjectDisposedException" /> if the context has already been disposed.
        /// </summary>
        /// <exception cref="System.ObjectDisposedException" />
        private void ThrowIfDisposed()
        {
            if (_disposed)
            {
                throw new ObjectDisposedException(GetType().Name);
            }
        }

        /// <summary>
        /// Saves all changes made in this context to the underlying database.
        /// </summary>
        /// <returns>
        /// A task that represents the asynchronous operation.
        /// </returns>
        private async Task SaveChangesAsync(CancellationToken cancellationToken)
        {
            if (AutoSaveChanges)
            {
                await Context.SaveChangesAsync(cancellationToken);
            }
        }

        /// <summary>
        /// Releases unmanaged and - optionally - managed resources.
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// If disposing, calls dispose on the Context.  Always nulls out the Context
        /// </summary>
        /// <param name="disposing"><c>true</c> to release both managed and unmanaged resources; <c>false</c> to release only unmanaged resources.</param>
        protected virtual void Dispose(bool disposing)
        {
            // Do not dispose the context here because it must be performed by the caller
            // An ASP.NET application, which uses dependency injection with lifetime configuration, 
            // can be crashed if the context is disposed prematurely
            _disposed = true;
        }
    }
}
