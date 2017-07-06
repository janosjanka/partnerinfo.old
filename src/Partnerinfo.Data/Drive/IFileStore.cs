// Copyright (c) János Janka. All rights reserved.

using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Partnerinfo.Drive
{
    /// <summary>
    /// Provides facilities for querying and working with document data as objects.
    /// </summary>
    public interface IFileStore : IDisposable
    {
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
        Task<ListResult<FileResult>> FindAllAsync(int ownerId, int? parentId, string name, int pageIndex, int pageSize, CancellationToken cancellationToken);

        /// <summary>
        /// Gets a collection of all the documents in the database.
        /// </summary>
        /// <param name="ids">A set of unique document identifiers.</param>
        /// <param name="cancellationToken">A token to observe while waiting for the task to complete.</param>
        /// <returns>
        /// Returns the information for all documents for an application as a collection of <see cref="FileItem" /> objects.
        /// </returns>
        Task<IList<FileItem>> FindAllAsync(IEnumerable<int> ids, CancellationToken cancellationToken);

        /// <summary>
        /// Gets the information from the data source for the document associated with the specified unique identifier.
        /// </summary>
        /// <param name="id">The unique identifier from the data source for the document.</param>
        /// <param name="cancellationToken">A token to observe while waiting for the task to complete.</param>
        /// <returns>
        /// Returns the document associated with the specified unique identifier.
        /// </returns>
        Task<FileItem> FindByIdAsync(int id, CancellationToken cancellationToken);

        /// <summary>
        /// Gets the information from the data source for the document associated with the specified unique identifier.
        /// </summary>
        /// <param name="uri">The unique identifier from the data source for the document.</param>
        /// <param name="cancellationToken">A token to observe while waiting for the task to complete.</param>
        /// <returns>
        /// Returns the document associated with the specified unique identifier.
        /// </returns>
        Task<FileItem> FindByUriAsync(string uri, CancellationToken cancellationToken);

        /// <summary>
        /// Inserts a file.
        /// </summary>
        /// <param name="file">The file to insert.</param>
        /// <param name="cancellationToken">A token to observe while waiting for the task to complete.</param>
        /// <returns>
        /// A task that represents the asynchronous operation.
        /// </returns>
        Task<ValidationResult> CreateAsync(FileItem file, CancellationToken cancellationToken);

        /// <summary>
        /// Updates a file.
        /// </summary>
        /// <param name="file">The file to update.</param>
        /// <param name="cancellationToken">A token to observe while waiting for the task to complete.</param>
        /// <returns>
        /// A task that represents the asynchronous operation.
        /// </returns>
        Task<ValidationResult> UpdateAsync(FileItem file, CancellationToken cancellationToken);

        /// <summary>
        /// Deletes a file.
        /// </summary>
        /// <param name="file">The file to delete.</param>
        /// <param name="cancellationToken">A token to observe while waiting for the task to complete.</param>
        /// <returns>
        /// A task that represents the asynchronous operation.
        /// </returns>
        Task<ValidationResult> DeleteAsync(FileItem file, CancellationToken cancellationToken);

        /// <summary>
        /// Updates information about a file in the database.
        /// </summary>
        /// <param name="id">The unique identifier from the data source for the document.</param>
        /// <param name="name">The name of the document to update.</param>
        /// <param name="cancellationToken">A token to observe while waiting for the task to complete.</param>
        Task SetNameAsync(int id, string name, CancellationToken cancellationToken);
    }
}
