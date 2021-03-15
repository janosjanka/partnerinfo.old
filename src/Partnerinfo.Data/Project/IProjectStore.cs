// Copyright (c) János Janka. All rights reserved.

using System;
using System.Threading;
using System.Threading.Tasks;

namespace Partnerinfo.Project
{
    /// <summary>
    /// Provides facilities for querying and working with system data as objects.
    /// </summary>
    public interface IProjectStore : IDisposable
    {
        /// <summary>
        /// Inserts a project.
        /// </summary>
        /// <param name="project">The project to insert.</param>
        /// <param name="cancellationToken">A token to observe while waiting for the task to complete.</param>
        /// <returns>
        /// A task that represents the asynchronous operation.
        /// </returns>
        Task<ValidationResult> CreateAsync(ProjectItem project, CancellationToken cancellationToken);

        /// <summary>
        /// Updates a project.
        /// </summary>
        /// <param name="project">The project to update.</param>
        /// <param name="cancellationToken">A token to observe while waiting for the task to complete.</param>
        /// <returns>
        /// A task that represents the asynchronous operation.
        /// </returns>
        Task<ValidationResult> UpdateAsync(ProjectItem project, CancellationToken cancellationToken);

        /// <summary>
        /// Deletes a project.
        /// </summary>
        /// <param name="project">The project to delete.</param>
        /// <param name="cancellationToken">A token to observe while waiting for the task to complete.</param>
        /// <returns>
        /// A task that represents the asynchronous operation.
        /// </returns>
        Task<ValidationResult> DeleteAsync(ProjectItem project, CancellationToken cancellationToken);

        /// <summary>
        /// Finds a project with the given primary key value.
        /// </summary>
        /// <param name="id">The primary key for the item to be found.</param>
        /// <param name="cancellationToken">A token to observe while waiting for the task to complete.</param>
        /// <returns>
        /// A task that represents the asynchronous operation.
        /// </returns>
        Task<ProjectItem> FindByIdAsync(int id, CancellationToken cancellationToken);

        /// <summary>
        /// Finds a collection of projects with the given values.
        /// </summary>
        /// <param name="userId">The user identifier for the item to be found.</param>
        /// <param name="name">The name for the item to be found.</param>
        /// <param name="orderBy">The order in which items are returned in a result set.</param>
        /// <param name="pageIndex">The index of the page of results to return. Use 1 to indicate the first page.</param>
        /// <param name="pageSize">The size of the page of results to return. <paramref name="pageIndex" /> is non-zero-based.</param>
        /// <param name="fields">The related fields to include in the query results.</param>
        /// <param name="cancellationToken">A token to observe while waiting for the task to complete.</param>
        /// <returns>
        /// A task that represents the asynchronous operation.
        /// </returns>
        Task<ListResult<ProjectItem>> FindAllAsync(int userId, string name, ProjectSortOrder orderBy, int pageIndex, int pageSize, ProjectField fields, CancellationToken cancellationToken);
    }
}
