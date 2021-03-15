// Copyright (c) János Janka. All rights reserved.

using System;
using System.Data.Entity;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Partnerinfo.Identity.EntityFramework;

namespace Partnerinfo.Security.EntityFramework
{
    public class SecurityStore : IAccessRuleStore
    {
        private bool _disposed;

        /// <summary>
        /// Initializes a new instance of the <see cref="SecurityStore" /> class.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <exception cref="System.ArgumentNullException"></exception>
        public SecurityStore(DbContext context)
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
        /// <value>
        ///   <c>true</c> if save changes; otherwise, <c>false</c>.
        /// </value>
        public bool AutoSaveChanges { get; set; } = true;

        /// <summary>
        /// Gets the context for the store.
        /// </summary>
        /// <value>
        /// The context.
        /// </value>
        public DbContext Context { get; }

        /// <summary>
        /// Finds a collection of Access Control Entries (ACEs) with the given values.
        /// </summary>
        /// <param name="objectType">The type of the shared object to be found.</param>
        /// <param name="objectId">The primary key of the shared object to be found.</param>
        /// <param name="cancellationToken">A token to observe while waiting for the task to complete.</param>
        /// <returns>
        /// A task that represents the asynchronous operation.
        /// </returns>
        public virtual async Task<ListResult<AccessRuleItem>> GetAccessRulesAsync(AccessObjectType objectType, int objectId, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            ThrowIfDisposed();
            return await
            (
                from ace in AccessRules
                where ace.ObjectType == objectType && ace.ObjectId == objectId
                orderby ace.User.Email.Name
                select new AccessRuleItem
                {
                    Permission = ace.Permission,
                    Visibility = ace.Visibility,
                    User = ace.User == null ? null : new AccountItem
                    {
                        Id = ace.User.Id,
                        Email = ace.User.Email,
                        FirstName = ace.User.FirstName,
                        LastName = ace.User.LastName,
                        Gender = ace.User.Gender,
                        Birthday = ace.User.Birthday
                    }
                }
            ).ToListResultAsync(cancellationToken);
        }

        /// <summary>
        /// Retrieves the <see cref="AccessRuleItem" /> associated with the key, as an asynchronous operation.
        /// </summary>
        /// <param name="objectType">The type of the shared object to be found.</param>
        /// <param name="objectId">The primary key of the shared object to be found.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken" /> used to propagate notifications that the operation should be canceled.</param>
        /// <returns>
        /// The <see cref="Task" /> for the asynchronous operation, containing the contact, if any which matched the specified key.
        /// </returns>
        public virtual Task<AccessRuleItem> GetAccessRuleAsync(AccessObjectType objectType, int objectId, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            ThrowIfDisposed();
            return AccessRules
                .Where(objectType, objectId, null)
                .Select(ace => new AccessRuleItem
                {
                    Permission = ace.Permission,
                    Visibility = ace.Visibility
                })
                .SingleOrDefaultAsync(cancellationToken);
        }

        /// <summary>
        /// Retrieves the <see cref="AccessRuleItem" /> associated with the key, as an asynchronous operation.
        /// </summary>
        /// <param name="objectType">The type of the shared object to be found.</param>
        /// <param name="objectId">The primary key of the shared object to be found.</param>
        /// <param name="userId">The primary key of the user to be found.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken" /> used to propagate notifications that the operation should be canceled.</param>
        /// <returns>
        /// The <see cref="Task" /> for the asynchronous operation, containing the contact, if any which matched the specified key.
        /// </returns>
        public virtual Task<AccessRuleItem> GetAccessRuleByUserAsync(AccessObjectType objectType, int objectId, int userId, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            ThrowIfDisposed();
            return AccessRules.Where(objectType, objectId, userId).Select(ace => new AccessRuleItem
            {
                Permission = ace.Permission,
                Visibility = ace.Visibility,
                User = new AccountItem
                {
                    Id = ace.User.Id,
                    Email = ace.User.Email,
                    FirstName = ace.User.FirstName,
                    LastName = ace.User.LastName,
                    Gender = ace.User.Gender,
                    Birthday = ace.User.Birthday
                }
            }).SingleOrDefaultAsync(cancellationToken);
        }

        /// <summary>
        /// Inserts a list of <see cref="AccessRuleItem" />s to the specified <paramref name="item" />.
        /// </summary>
        /// <param name="objectType">The type of the shared object to be inserted.</param>
        /// <param name="objectId">The primary key for the shared object to be inserted.</param>
        /// <param name="rule">The ACEs (Access Control Entries) to insert.</param>
        /// <param name="cancellationToken">A token to observe while waiting for the task to complete.</param>
        /// <returns>
        /// A task that represents the asynchronous operation.
        /// </returns>
        public virtual async Task<ValidationResult> AddAccessRuleAsync(AccessObjectType objectType, int objectId, AccessRuleItem rule, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            ThrowIfDisposed();
            if (rule == null)
            {
                throw new ArgumentNullException(nameof(rule));
            }

            if (await AccessRules.Where(objectType, objectId, rule.User?.Id).AnyAsync(cancellationToken))
            {
                return ValidationResult.Failed("Duplicated ACE entry.");
            }
            AccessRules.Add(new SecurityAccessRule
            {
                ObjectType = objectType,
                ObjectId = objectId,
                UserId = rule.User?.Id,
                Permission = rule.Permission,
                Visibility = rule.Visibility
            });
            await SaveChangesAsync(cancellationToken);
            return ValidationResult.Success;
        }

        /// <summary>
        /// Replaces the given <paramref name="rule" /> on the specified object with the <paramref name="newTrustee" />.
        /// </summary>
        /// <param name="objectType">The type of the shared object to be found.</param>
        /// <param name="objectId">The primary key for the shared object to be found.</param>
        /// <param name="rule">The ACE (Access Control Entry) to replace.</param>
        /// <param name="newTrustee">The new ACE (Access Control Entry) to set.</param>
        /// <param name="cancellationToken">A token to observe while waiting for the task to complete.</param>
        /// <returns>
        /// A task that represents the asynchronous operation.
        /// </returns>
        public virtual async Task<ValidationResult> ReplaceAccessRuleAsync(AccessObjectType objectType, int objectId, AccessRuleItem rule, AccessRuleItem newTrustee, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            ThrowIfDisposed();
            if (rule == null)
            {
                throw new ArgumentNullException(nameof(rule));
            }
            if (newTrustee == null)
            {
                throw new ArgumentNullException(nameof(newTrustee));
            }

            var userId = rule.User?.Id;
            var aceEntity = await AccessRules.SingleOrDefaultAsync(p => p.ObjectType == objectType && p.ObjectId == objectId && p.UserId == userId, cancellationToken);
            if (aceEntity == null)
            {
                throw new InvalidOperationException("ACE does not exist.");
            }
            aceEntity.Permission = newTrustee.Permission;
            aceEntity.Visibility = newTrustee.Visibility;
            await SaveChangesAsync(cancellationToken);
            return ValidationResult.Success;
        }

        /// <summary>
        /// Deletes a list of <see cref="AccessRuleItem" />s from the specified <paramref name="item" />.
        /// </summary>
        /// <param name="objectType">The type of the shared object to be removed.</param>
        /// <param name="objectId">The primary key for the shared object to be removed.</param>
        /// <param name="rule">The ACEs (Access Control Entries) to delete.</param>
        /// <param name="cancellationToken">A token to observe while waiting for the task to complete.</param>
        /// <returns>
        /// A task that represents the asynchronous operation.
        /// </returns>
        public virtual async Task<ValidationResult> RemoveAccessRuleAsync(AccessObjectType objectType, int objectId, AccessRuleItem rule, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            ThrowIfDisposed();
            if (rule == null)
            {
                throw new ArgumentNullException(nameof(rule));
            }

            var aceEntity = await AccessRules.Where(objectType, objectId, rule.User?.Id).SingleOrDefaultAsync(cancellationToken);
            if (aceEntity == null)
            {
                throw new InvalidOperationException("ACE does not exist.");
            }
            Context.Remove(aceEntity);
            await SaveChangesAsync(cancellationToken);
            return ValidationResult.Success;
        }

        /// <summary>
        /// Deletes all <see cref="AccessRuleItem" />s from the specified <paramref name="item" />.
        /// </summary>
        /// <param name="objectType">The type of the shared object to be found.</param>
        /// <param name="objectId">The primary key for the shared object to be found.</param>
        /// <param name="cancellationToken">A token to observe while waiting for the task to complete.</param>
        /// <returns>
        /// A task that represents the asynchronous operation.
        /// </returns>
        public virtual async Task<ValidationResult> RemoveAccessRulesAsync(AccessObjectType objectType, int objectId, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            ThrowIfDisposed();

            await Context.Database.ExecuteSqlCommandAsync(
               $"{DbSchema.Security}.DeleteAccessRules @ObjectTypeId, @ObjectId",
               cancellationToken,
               DbParameters.Value("ObjectTypeId", objectType),
               DbParameters.Value("ObjectId", objectId));

            return ValidationResult.Success;
        }

        /// <summary>
        /// The <see cref="DbSet" /> for access to <see cref="SecurityAccessRule" />s in the context.
        /// </summary>
        /// <value>
        /// The <see cref="SecurityAccessRule" />s.
        /// </value>
        private DbSet<SecurityAccessRule> AccessRules => Context.Set<SecurityAccessRule>();

        /// <summary>
        /// The <see cref="DbSet" /> for access to <see cref="IdentityUser" />s in the context.
        /// </summary>
        /// <value>
        /// The <see cref="IdentityUser" />s.
        /// </value>
        private DbSet<IdentityUser> Users => Context.Set<IdentityUser>();

        /// <summary>
        /// Saves all changes made in this context to the underlying database.
        /// </summary>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>
        /// A task that represents the asynchronous operation.
        /// </returns>
        private Task SaveChangesAsync(CancellationToken cancellationToken)
        {
            return AutoSaveChanges ? Context.SaveChangesAsync(cancellationToken) : Task.FromResult(0);
        }

        /// <summary>
        /// Throws a <see cref="ObjectDisposedException" /> if the context has already been disposed.
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
