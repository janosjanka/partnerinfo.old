// Copyright (c) János Janka. All rights reserved.

using System;
using System.Threading;
using System.Threading.Tasks;

namespace Partnerinfo.Identity
{
    public class UserManager : IDisposable
    {
        private bool _disposed;

        /// <summary>
        /// Initializes a new instance of the <see cref="UserManager" /> class
        /// </summary>
        /// <param name="store">The project store.</param>
        public UserManager(IUserStore store)
        {
            if (store == null)
            {
                throw new ArgumentNullException(nameof(store));
            }
            Store = store;
        }

        /// <summary>
        /// The store that the <see cref="UserManager" /> operates against.
        /// </summary>
        protected IUserStore Store { get; set; }

        /// <summary>
        /// Gets a list of <see cref="IUserValidator" />s.
        /// </summary>
        /// <value>
        /// A list of <see cref="IUserValidator" />s.
        /// </value>
        internal IUserValidator Validator { get; }

        /// <summary>
        /// Inserts a user.
        /// </summary>
        /// <param name="user">The user to insert.</param>
        /// <param name="cancellationToken">A token to observe while waiting for the task to complete.</param>
        /// <returns>
        /// A task that represents the asynchronous operation.
        /// </returns>
        public virtual async Task<ValidationResult> CreateAsync(UserItem user, CancellationToken cancellationToken)
        {
            ThrowIfDisposed();
            if (user == null)
            {
                throw new ArgumentNullException(nameof(user));
            }
            var validationResult = await ValidateUserAsync(user, cancellationToken);
            if (!validationResult.Succeeded)
            {
                return validationResult;
            }
            return await Store.CreateAsync(user, cancellationToken);
        }

        /// <summary>
        /// Updates a user.
        /// </summary>
        /// <param name="user">The user to update.</param>
        /// <param name="cancellationToken">A token to observe while waiting for the task to complete.</param>
        /// <returns>
        /// A task that represents the asynchronous operation.
        /// </returns>
        public virtual async Task<ValidationResult> UpdateAsync(UserItem user, CancellationToken cancellationToken)
        {
            ThrowIfDisposed();
            if (user == null)
            {
                throw new ArgumentNullException(nameof(user));
            }
            var validationResult = await ValidateUserAsync(user, cancellationToken);
            if (!validationResult.Succeeded)
            {
                return validationResult;
            }
            return await Store.UpdateAsync(user, cancellationToken);
        }

        /// <summary>
        /// Deletes a user.
        /// </summary>
        /// <param name="user">The user to delete.</param>
        /// <param name="cancellationToken">A token to observe while waiting for the task to complete.</param>
        /// <returns>
        /// A task that represents the asynchronous operation.
        /// </returns>
        public virtual Task<ValidationResult> DeleteAsync(UserItem user, CancellationToken cancellationToken)
        {
            ThrowIfDisposed();
            if (user == null)
            {
                throw new ArgumentNullException(nameof(user));
            }
            return Store.DeleteAsync(user, cancellationToken);
        }

        /// <summary>
        /// Retrieves the user associated with the key, as an asynchronous operation.
        /// </summary>
        /// <param name="id">The id provided by the <paramref name="id"/> to identify a user.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/> used to propagate notifications that the operation should be canceled.</param>
        /// <returns>
        /// The <see cref="Task"/> for the asynchronous operation, containing the user, if any which matched the specified key.
        /// </returns>
        public virtual Task<UserItem> FindByIdAsync(int id, CancellationToken cancellationToken)
        {
            return FindByIdAsync(id, UserField.None, cancellationToken);
        }

        /// <summary>
        /// Retrieves the user associated with the key, as an asynchronous operation.
        /// </summary>
        /// <param name="id">The id provided by the <paramref name="id"/> to identify a user.</param>
        /// <param name="fields">The related fields to include in the query results.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/> used to propagate notifications that the operation should be canceled.</param>
        /// <returns>
        /// The <see cref="Task"/> for the asynchronous operation, containing the user, if any which matched the specified key.
        /// </returns>
        public virtual Task<UserItem> FindByIdAsync(int id, UserField fields, CancellationToken cancellationToken)
        {
            ThrowIfDisposed();
            return Store.FindByIdAsync(id, fields, cancellationToken);
        }

        /// <summary>
        /// Retrieves the user associated with the email address, as an asynchronous operation.
        /// </summary>
        /// <param name="email">The email address to return the user for.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/> used to propagate notifications that the operation should be canceled.</param>
        /// <returns>
        /// The <see cref="Task"/> for the asynchronous operation, containing the user, if any which matched the specified key.
        /// </returns>
        public virtual Task<UserItem> FindByEmailAsync(string email, CancellationToken cancellationToken)
        {
            return FindByEmailAsync(email, UserField.None, cancellationToken);
        }

        /// <summary>
        /// Retrieves the user associated with the email address, as an asynchronous operation.
        /// </summary>
        /// <param name="email">The email address to return the user for.</param>
        /// <param name="fields">The related fields to include in the query results.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/> used to propagate notifications that the operation should be canceled.</param>
        /// <returns>
        /// The <see cref="Task"/> for the asynchronous operation, containing the user, if any which matched the specified key.
        /// </returns>
        public virtual Task<UserItem> FindByEmailAsync(string email, UserField fields, CancellationToken cancellationToken)
        {
            ThrowIfDisposed();
            return Store.FindByEmailAsync(email, fields, cancellationToken);
        }

        /// <summary>
        /// Finds a collection of users with the given values.
        /// </summary>
        /// <param name="name">The name for the item to be found.</param>
        /// <param name="orderBy">The order in which items are returned in a result set.</param>
        /// <param name="pageIndex">The index of the page of results to return. Use 1 to indicate the first page.</param>
        /// <param name="pageSize">The size of the page of results to return. <paramref name="pageIndex" /> is non-zero-based.</param>
        /// <param name="fields">The related fields to include in the query results.</param>
        /// <param name="cancellationToken">A token to observe while waiting for the task to complete.</param>
        /// <returns>
        /// A task that represents the asynchronous operation.
        /// </returns>
        public virtual Task<ListResult<UserItem>> FindAllAsync(string name, UserSortOrder orderBy, int pageIndex, int pageSize, UserField fields, CancellationToken cancellationToken)
        {
            ThrowIfDisposed();
            return Store.FindAllAsync(name, orderBy, pageIndex, pageSize, fields, cancellationToken);
        }

        /// <summary>
        /// Validates user. Called by other UserManager methods.
        /// </summary>
        private Task<ValidationResult> ValidateUserAsync(UserItem user, CancellationToken cancellationToken)
        {
            return (Validator == null) ? Task.FromResult(ValidationResult.Success) : Validator.ValidateAsync(this, user, cancellationToken);
        }

        /// <summary>
        /// Throws a <see cref="ObjectDisposedException" /> if the context has already been disposed
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
                Store.Dispose();
            }
            _disposed = true;
            Store = null;
        }
    }
}
