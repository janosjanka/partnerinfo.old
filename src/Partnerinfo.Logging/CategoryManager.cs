// Copyright (c) János Janka. All rights reserved.

using System;
using System.Threading;
using System.Threading.Tasks;
using Partnerinfo.Project;

namespace Partnerinfo.Logging
{
    public class CategoryManager
    {
        private bool _disposed;

        /// <summary>
        /// Initializes a new instance of the <see cref="CategoryManager"/> class.
        /// </summary>
        /// <param name="store">The store that the <see cref="CategoryManager" /> operates against.</param>
        /// <exception cref="System.ArgumentNullException"></exception>
        public CategoryManager(ICategoryStore store)
        {
            if (store == null)
            {
                throw new ArgumentNullException(nameof(store));
            }
            Store = store;
        }

        /// <summary>
        /// Gets the store that the <see cref="CategoryManager" /> operates against.
        /// </summary>
        /// <value>
        /// The store.
        /// </value>
        protected ICategoryStore Store { get; set; }

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
        /// <exception cref="System.ArgumentNullException">category</exception>
        public virtual Task<ValidationResult> CreateAsync(AccountItem user, ProjectItem project, CategoryItem category, CancellationToken cancellationToken)
        {
            ThrowIfDisposed();
            if (category == null)
            {
                throw new ArgumentNullException("category");
            }
            return Store.CreateAsync(user, project, category, cancellationToken);
        }

        /// <summary>
        /// Updates a category.
        /// </summary>
        /// <param name="category">The category to update.</param>
        /// <param name="cancellationToken">A token to observe while waiting for the task to complete.</param>
        /// <returns>
        /// A task that represents the asynchronous operation.
        /// </returns>
        /// <exception cref="System.ArgumentNullException">category</exception>
        public virtual Task<ValidationResult> UpdateAsync(CategoryItem category, CancellationToken cancellationToken)
        {
            ThrowIfDisposed();
            if (category == null)
            {
                throw new ArgumentNullException("category");
            }
            return Store.UpdateAsync(category, cancellationToken);
        }

        /// <summary>
        /// Deletes a category.
        /// </summary>
        /// <param name="category">The category to delete.</param>
        /// <param name="cancellationToken">A token to observe while waiting for the task to complete.</param>
        /// <returns>
        /// A task that represents the asynchronous operation.
        /// </returns>
        /// <exception cref="System.ArgumentNullException">category</exception>
        public virtual Task<ValidationResult> DeleteAsync(CategoryItem category, CancellationToken cancellationToken)
        {
            ThrowIfDisposed();
            if (category == null)
            {
                throw new ArgumentNullException("category");
            }
            return Store.DeleteAsync(category, cancellationToken);
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
            ThrowIfDisposed();
            return Store.FindByIdAsync(id, fields, cancellationToken);
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
        /// <exception cref="System.ArgumentNullException">user</exception>
        public virtual Task<ListResult<CategoryItem>> FindAllAsync(AccountItem user, ProjectItem project, CategorySortOrder orderBy, CategoryField fields, CancellationToken cancellationToken)
        {
            ThrowIfDisposed();
            if (user == null)
            {
                throw new ArgumentNullException("user");
            }
            return Store.FindAllAsync(user, project, orderBy, fields, cancellationToken);
        }

        /// <summary>
        /// Throws a <see cref="ObjectDisposedException" /> if the context has already been disposed
        /// </summary>
        /// <exception cref="System.ObjectDisposedException"></exception>
        private void ThrowIfDisposed()
        {
            if (_disposed)
            {
                throw new ObjectDisposedException(GetType().Name);
            }
        }

        /// <summary>
        /// Releases unmanaged and - optionally - managed resources
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// If disposing, calls dispose on the Context. Always nulls out the Context
        /// </summary>
        /// <param name="disposing"><c>true</c> to release both managed and unmanaged resources; <c>false</c> to release only unmanaged resources</param>
        protected virtual void Dispose(bool disposing)
        {
            if (disposing && Store != null)
            {
                // Store.Dispose();
            }
            _disposed = true;
            Store = null;
        }
    }
}
