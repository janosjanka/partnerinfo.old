// Copyright (c) János Janka. All rights reserved.

using System;
using System.Threading;
using System.Threading.Tasks;
using Partnerinfo.Project;

namespace Partnerinfo.Logging
{
    public interface ICategoryStore : IDisposable
    {
        /// <summary>
        /// Inserts a category.
        /// </summary>
        /// <param name="user">The user who owns the category.</param>
        /// <param name="project">The project which is associated with the category.</param>
        /// <param name="category">The category to insert.</param>
        /// <param name="cancellationToken">A token to observe while waiting for the task to complete.</param>
        /// <returns>
        /// A task that represents the asynchronous operation.
        /// </returns>
        Task<ValidationResult> CreateAsync(AccountItem user, ProjectItem project, CategoryItem category, CancellationToken cancellationToken);

        /// <summary>
        /// Updates a category.
        /// </summary>
        /// <param name="category">The category to update.</param>
        /// <param name="cancellationToken">A token to observe while waiting for the task to complete.</param>
        /// <returns>
        /// A task that represents the asynchronous operation.
        /// </returns>
        Task<ValidationResult> UpdateAsync(CategoryItem category, CancellationToken cancellationToken);

        /// <summary>
        /// Deletes a category.
        /// </summary>
        /// <param name="category">The category to delete.</param>
        /// <param name="cancellationToken">A token to observe while waiting for the task to complete.</param>
        /// <returns>
        /// A task that represents the asynchronous operation.
        /// </returns>
        Task<ValidationResult> DeleteAsync(CategoryItem category, CancellationToken cancellationToken);

        /// <summary>
        /// Finds a category with the given primary key value.
        /// </summary>
        /// <param name="id">The primary key for the item to be found.</param>
        /// <param name="fields">The related fields to include in the query results.</param>
        /// <param name="cancellationToken">A token to observe while waiting for the task to complete.</param>
        /// <returns>
        /// A task that represents the asynchronous operation.
        /// </returns>
        Task<CategoryItem> FindByIdAsync(int id, CategoryField fields, CancellationToken cancellationToken);

        /// <summary>
        /// Finds a collection of categories with the given values.
        /// </summary>
        /// <param name="user">The user for the item to be found.</param>
        /// <param name="project">The project for the item to be found.</param>
        /// <param name="orderBy">The order in which items are returned in a result set.</param>
        /// <param name="fields">The related fields to include in the query results.</param>
        /// <param name="cancellationToken">A token to observe while waiting for the task to complete.</param>
        /// <returns>
        /// A task that represents the asynchronous operation.
        /// </returns>
        Task<ListResult<CategoryItem>> FindAllAsync(AccountItem user, ProjectItem project, CategorySortOrder orderBy, CategoryField fields, CancellationToken cancellationToken);
    }
}
