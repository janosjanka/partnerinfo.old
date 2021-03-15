// Copyright (c) János Janka. All rights reserved.

using System;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Globalization;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Partnerinfo.Identity.EntityFramework
{
    public class UserStore : IUserStore
    {
        private bool _disposed;

        /// <summary>
        /// Initializes a new instance of the <see cref="UserStore" /> class.
        /// </summary>
        /// <param name="context">The context.</param>
        public UserStore(DbContext context)
        {
            if (context == null)
            {
                throw new ArgumentNullException(nameof(context));
            }
            Context = context;
        }

        /// <summary>
        /// If true will call SaveChanges after CreateAsync/UpdateAsync/DeleteAsync.
        /// </summary>
        public bool AutoSaveChanges { get; set; } = true;

        /// <summary>
        /// Gets the context for the store.
        /// </summary>
        public DbContext Context { get; }

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
            cancellationToken.ThrowIfCancellationRequested();
            ThrowIfDisposed();
            if (user == null)
            {
                throw new ArgumentNullException(nameof(user));
            }
            var userEntity = Context.Add(new IdentityUser
            {
                Email = user.Email,
                FirstName = user.FirstName,
                LastName = user.LastName,
                NickName = user.NickName,
                Gender = user.Gender,
                Birthday = user.Birthday,
                LastLoginDate = user.LastLoginDate,
                LastIPAddress = user.LastIPAddress
            });
            try
            {
                await SaveChangesAsync(cancellationToken);
            }
            catch (DbUpdateConcurrencyException)
            {
                return ValidationResult.Failed("");
            }
            user.Id = userEntity.Id;
            user.CreatedDate = userEntity.CreatedDate;
            user.ModifiedDate = userEntity.ModifiedDate;
            return ValidationResult.Success;
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
            cancellationToken.ThrowIfCancellationRequested();
            ThrowIfDisposed();
            if (user == null)
            {
                throw new ArgumentNullException(nameof(user));
            }
            var userEntity = await Users.FindAsync(cancellationToken, user.Id);
            if (userEntity == null)
            {
                throw new InvalidOperationException(string.Format(CultureInfo.CurrentCulture, IdentityResources.UserNotFound, user.Id));
            }
            Context.Patch(userEntity, new
            {
                user.Email,
                user.FirstName,
                user.LastName,
                user.NickName,
                user.Gender,
                user.Birthday,
                user.LastLoginDate,
                user.LastIPAddress
            });
            try
            {
                await SaveChangesAsync(cancellationToken);
            }
            catch (DbUpdateConcurrencyException)
            {
                return ValidationResult.Failed("");
            }
            user.ModifiedDate = userEntity.ModifiedDate;
            return ValidationResult.Success;
        }

        /// <summary>
        /// Deletes a user.
        /// </summary>
        /// <param name="user">The user to delete.</param>
        /// <param name="cancellationToken">A token to observe while waiting for the task to complete.</param>
        /// <returns>
        /// A task that represents the asynchronous operation.
        /// </returns>
        public virtual async Task<ValidationResult> DeleteAsync(UserItem user, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            ThrowIfDisposed();
            if (user == null)
            {
                throw new ArgumentNullException(nameof(user));
            }
            var userEntity = await Users.FindAsync(cancellationToken, user.Id);
            if (userEntity == null)
            {
                throw new InvalidOperationException(string.Format(CultureInfo.CurrentCulture, IdentityResources.UserNotFound, user.Id));
            }
            Context.Remove(userEntity);
            try
            {
                await SaveChangesAsync(cancellationToken);
            }
            catch (DbUpdateConcurrencyException)
            {
                return ValidationResult.Failed("");
            }
            return ValidationResult.Success;
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
            cancellationToken.ThrowIfCancellationRequested();
            ThrowIfDisposed();
            return Users.Where(u => u.Id == id).Select(fields).FirstOrDefaultAsync(cancellationToken);
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
            cancellationToken.ThrowIfCancellationRequested();
            ThrowIfDisposed();
            return Users.Where(u => u.Email.Address == email).Select(fields).FirstOrDefaultAsync(cancellationToken);
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
            cancellationToken.ThrowIfCancellationRequested();
            ThrowIfDisposed();
            return Users
               .AsNoTracking()
               .Where(name)
               .OrderBy(orderBy)
               .Select(fields)
               .ToListResultAsync(pageIndex, pageSize, cancellationToken);
        }

        /// <summary>
        /// The DbSet for access to users in the context.
        /// </summary>
        private DbSet<IdentityUser> Users
        {
            get { return Context.Set<IdentityUser>(); }
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
