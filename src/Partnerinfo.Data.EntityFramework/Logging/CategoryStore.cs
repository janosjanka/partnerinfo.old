// Copyright (c) János Janka. All rights reserved.

using System;
using System.Data.Entity;
using System.Globalization;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Partnerinfo.Project;

namespace Partnerinfo.Logging.EntityFramework
{
    public class CategoryStore : ICategoryStore
    {
        private bool _disposed;

        /// <summary>
        /// Initializes a new instance of the <see cref="CategoryStore" /> class.
        /// </summary>
        /// <param name="context">The context.</param>
        public CategoryStore(DbContext context)
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
        public DbContext Context { get; private set; }

        /// <summary>
        /// Inserts a category.
        /// </summary>
        /// <param name="category">The category to insert.</param>
        /// <param name="cancellationToken">A token to observe while waiting for the task to complete.</param>
        /// <returns>
        /// A task that represents the asynchronous operation.
        /// </returns>
        public virtual async Task<ValidationResult> CreateAsync(AccountItem user, ProjectItem project, CategoryItem category, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            if (user == null)
            {
                throw new ArgumentNullException("user");
            }
            if (category == null)
            {
                throw new ArgumentNullException("category");
            }
            var categoryEntity = Context.Add(new LoggingCategory
            {
                OwnerId = user.Id,
                ProjectId = project?.Id,
                Name = category.Name,
                Color = category.Color?.RGB ?? 0
            });
            await SaveChangesAsync(cancellationToken);
            category.Id = categoryEntity.Id;
            return ValidationResult.Success;
        }

        /// <summary>
        /// Updates a category.
        /// </summary>
        /// <param name="category">The category to update.</param>
        /// <param name="cancellationToken">A token to observe while waiting for the task to complete.</param>
        /// <returns>
        /// A task that represents the asynchronous operation.
        /// </returns>
        public virtual async Task<ValidationResult> UpdateAsync(CategoryItem category, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            ThrowIfDisposed();
            if (category == null)
            {
                throw new ArgumentNullException("category");
            }
            var categoryEntity = await Categories.FindAsync(cancellationToken, category.Id);
            if (categoryEntity == null)
            {
                throw new InvalidOperationException(string.Format(CultureInfo.CurrentCulture, LoggingResources.CategoryNotFound, category.Id));
            }
            categoryEntity.Name = category.Name;
            categoryEntity.Color = category.Color?.RGB ?? 0;
            categoryEntity.ModifiedDate = DateTime.UtcNow;
            await SaveChangesAsync(cancellationToken);
            return ValidationResult.Success;
        }

        /// <summary>
        /// Deletes a category.
        /// </summary>
        /// <param name="category">The category to delete.</param>
        /// <param name="cancellationToken">A token to observe while waiting for the task to complete.</param>
        /// <returns>
        /// A task that represents the asynchronous operation.
        /// </returns>
        public virtual async Task<ValidationResult> DeleteAsync(CategoryItem category, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            if (category == null)
            {
                throw new ArgumentNullException("category");
            }
            var categoryEntity = await Categories.FindAsync(cancellationToken, category.Id);
            if (categoryEntity == null)
            {
                throw new InvalidOperationException(string.Format(CultureInfo.CurrentCulture, LoggingResources.CategoryNotFound, category.Id));
            }
            Context.Remove(categoryEntity);
            await SaveChangesAsync(cancellationToken);
            return ValidationResult.Success;
        }

        /// <summary>
        /// Finds a category with the given primary key value.
        /// </summary>
        /// <param name="id">The primary key for the item to be found.</param>
        /// <param name="fields">The related fields to include in the query results.</param>
        /// <param name="cancellationToken">A token to observe while waiting for the task to complete.</param>
        /// <returns>
        /// A task that represents the asynchronous operation.
        /// </returns>
        public virtual Task<CategoryItem> FindByIdAsync(int id, CategoryField fields, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            ThrowIfDisposed();
            return Categories.Where(c => c.Id == id).Select(fields).FirstOrDefaultAsync(cancellationToken);
        }

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
        public virtual Task<ListResult<CategoryItem>> FindAllAsync(AccountItem user, ProjectItem project, CategorySortOrder orderBy, CategoryField fields, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            ThrowIfDisposed();
            if (user == null)
            {
                throw new ArgumentNullException("user");
            }
            return Categories
                .Where(user.Id, project?.Id)
                .OrderBy(orderBy)
                .Select(fields)
                .ToListResultAsync(cancellationToken);
        }

        /// <summary>
        /// The DbSet for access to categories in the context.
        /// </summary>
        private DbSet<LoggingCategory> Categories
        {
            get { return Context.Set<LoggingCategory>(); }
        }

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
        private Task SaveChangesAsync(CancellationToken cancellationToken)
        {
            return AutoSaveChanges ? Context.SaveChangesAsync(cancellationToken) : Task.FromResult(0);
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
