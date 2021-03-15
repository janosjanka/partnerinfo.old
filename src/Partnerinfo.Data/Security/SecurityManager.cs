// Copyright (c) János Janka. All rights reserved.

using System;
using System.Globalization;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Partnerinfo.Identity;

namespace Partnerinfo.Security
{
    public class SecurityManager : IDisposable
    {
        private bool _disposed;

        /// <summary>
        /// Initializes a new instance of the <see cref="SecurityManager" /> class.
        /// </summary>
        /// <param name="userManager">The user manager.</param>
        public SecurityManager(UserManager userManager, IAccessRuleStore store, IAccessRuleValidator validator)
        {
            if (userManager == null)
            {
                throw new ArgumentNullException(nameof(userManager));
            }
            if (store == null)
            {
                throw new ArgumentNullException(nameof(store));
            }
            if (validator == null)
            {
                throw new ArgumentNullException(nameof(validator));
            }
            UserManager = userManager;
            Store = store;
            Validator = validator;
        }

        /// <summary>
        /// Gets or sets the user manager that the <see cref="SecurityManager" /> operates against.
        /// </summary>
        /// <value>
        /// The user manager.
        /// </value>
        protected UserManager UserManager { get; set; }

        /// <summary>
        /// Gets or sets the store that the <see cref="SecurityManager" /> operates against.
        /// </summary>
        /// <value>
        /// The store.
        /// </value>
        protected IAccessRuleStore Store { get; set; }

        /// <summary>
        /// Gets a list of <see cref="SecurityManager" />s.
        /// </summary>
        /// <value>
        /// A list of <see cref="SecurityManager" />s.
        /// </value>
        protected IAccessRuleValidator Validator { get; set; }

        /// <summary>
        /// Checks whether the specified user has the specified <paramref name="requiredPermission" /> for the specified object.
        /// </summary>
        /// <param name="item">The item which owns the entries.</param>
        /// <param name="userId">The user identifier.</param>
        /// <param name="requiredPermission">The required permission.</param>
        /// <param name="cancellationToken">A token to observe while waiting for the task to complete.</param>
        /// <returns>
        /// A task that represents the asynchronous operation.
        /// </returns>
        /// <exception cref="System.ArgumentNullException"></exception>
        public virtual Task<SecurityResult> CheckAccessAsync(SharedResourceItem item, int? userId, AccessPermission requiredPermission, CancellationToken cancellationToken)
        {
            ThrowIfDisposed();
            if (item == null)
            {
                throw new ArgumentNullException(nameof(item));
            }
            return CheckAccessAsync(item.ObjectType, item.Id, userId, requiredPermission, cancellationToken);
        }

        /// <summary>
        /// Checks whether the specified user has the specified <paramref name="requiredPermission" /> for the specified object.
        /// </summary>
        /// <param name="objectType">The type of the shared object to be found.</param>
        /// <param name="objectId">The primary key of the shared object to be found.</param>
        /// <param name="userId">The primary key of the user.</param>
        /// <param name="requiredPermission">The required permission.</param>
        /// <param name="cancellationToken">A token to observe while waiting for the task to complete.</param>
        /// <returns>
        /// A task that represents the asynchronous operation.
        /// </returns>
        public virtual async Task<SecurityResult> CheckAccessAsync(AccessObjectType objectType, int objectId, int? userId, AccessPermission requiredPermission, CancellationToken cancellationToken)
        {
            ThrowIfDisposed();

            var acl = await Store.GetAccessRulesAsync(objectType, objectId, cancellationToken);
            if (acl.Data.IsEmpty)
            {
                return SecurityResult.AccessDenied;
            }

            var anyone = acl.Data.FirstOrDefault(rule => rule.Anyone);
            if (userId == null && (anyone == null || anyone.Permission < requiredPermission || anyone.Visibility == AccessVisibility.Private))
            {
                return SecurityResult.AccessDenied;
            }

            var userOrAnyone = acl.Data.FirstOrDefault(rule => rule.User?.Id == userId) ?? anyone;
            if (userOrAnyone == null || userOrAnyone.Permission < requiredPermission)
            {
                return SecurityResult.AccessDenied;
            }

            return new SecurityResult(true, anyone?.Visibility ?? AccessVisibility.Unknown);
        }

        /// <summary>
        /// Checks whether the specified user has the specified <paramref name="requiredPermission" /> for the specified object.
        /// </summary>
        /// <param name="item">The item which owns the entries.</param>
        /// <param name="userEmail">The user email.</param>
        /// <param name="requiredPermission">The required permission.</param>
        /// <param name="cancellationToken">A token to observe while waiting for the task to complete.</param>
        /// <returns>
        /// A task that represents the asynchronous operation.
        /// </returns>
        public virtual Task<SecurityResult> CheckAccessAsync(SharedResourceItem item, string userEmail, AccessPermission requiredPermission, CancellationToken cancellationToken)
        {
            ThrowIfDisposed();
            if (item == null)
            {
                throw new ArgumentNullException(nameof(item));
            }
            return CheckAccessAsync(item.ObjectType, item.Id, userEmail, requiredPermission, cancellationToken);
        }

        /// <summary>
        /// Checks whether the specified user has the specified <paramref name="requiredPermission" /> for the specified object.
        /// </summary>
        /// <param name="objectType">The type of the shared object to be found.</param>
        /// <param name="objectId">The primary key of the shared object to be found.</param>
        /// <param name="userEmail">The user email.</param>
        /// <param name="requiredPermission">The required permission.</param>
        /// <param name="cancellationToken">A token to observe while waiting for the task to complete.</param>
        /// <returns>
        /// A task that represents the asynchronous operation.
        /// </returns>
        public virtual async Task<SecurityResult> CheckAccessAsync(AccessObjectType objectType, int objectId, string userEmail, AccessPermission requiredPermission, CancellationToken cancellationToken)
        {
            ThrowIfDisposed();
            var user = await UserManager.FindByEmailAsync(userEmail, cancellationToken);
            return await CheckAccessAsync(objectType, objectId, user?.Id, requiredPermission, cancellationToken);
        }

        /// <summary>
        /// Finds a collection of Access Control Entries (ACEs) with the given values.
        /// </summary>
        /// <param name="item">The item which owns the entries.</param>
        /// <param name="cancellationToken">A token to observe while waiting for the task to complete.</param>
        /// <returns>
        /// A task that represents the asynchronous operation.
        /// </returns>
        public virtual Task<ListResult<AccessRuleItem>> GetOwnersAsync(SharedResourceItem item, CancellationToken cancellationToken)
        {
            ThrowIfDisposed();
            if (item == null)
            {
                throw new ArgumentNullException(nameof(item));
            }
            return GetOwnersAsync(item.ObjectType, item.Id, cancellationToken);
        }

        /// <summary>
        /// Finds a collection of Access Control Entries (ACEs) with the given values.
        /// </summary>
        /// <param name="objectType">The type of the shared object to be found.</param>
        /// <param name="objectId">The primary key of the shared object to be found.</param>
        /// <param name="cancellationToken">A token to observe while waiting for the task to complete.</param>
        /// <returns>
        /// A task that represents the asynchronous operation.
        /// </returns>
        public virtual async Task<ListResult<AccessRuleItem>> GetOwnersAsync(AccessObjectType objectType, int objectId, CancellationToken cancellationToken)
        {
            ThrowIfDisposed();
            var rules = await Store.GetAccessRulesAsync(objectType, objectId, cancellationToken);
            return ListResult.Create(rules.Data.Where(ace => !ace.Anyone && ace.Permission == AccessPermission.IsOwner));
        }

        /// <summary>
        /// Finds a collection of Access Control Entries (ACEs) with the given values.
        /// </summary>
        /// <param name="item">The item which owns the entries.</param>
        /// <param name="cancellationToken">A token to observe while waiting for the task to complete.</param>
        /// <returns>
        /// A task that represents the asynchronous operation.
        /// </returns>
        public virtual Task<ListResult<AccessRuleItem>> GetAccessRulesAsync(SharedResourceItem item, CancellationToken cancellationToken)
        {
            ThrowIfDisposed();
            if (item == null)
            {
                throw new ArgumentNullException(nameof(item));
            }
            return GetAccessRulesAsync(item.ObjectType, item.Id, cancellationToken);
        }

        /// <summary>
        /// Finds a collection of Access Control Entries (ACEs) with the given values.
        /// </summary>
        /// <param name="objectType">The type of the shared object to be found.</param>
        /// <param name="objectId">The primary key of the shared object to be found.</param>
        /// <param name="cancellationToken">A token to observe while waiting for the task to complete.</param>
        /// <returns>
        /// A task that represents the asynchronous operation.
        /// </returns>
        public virtual Task<ListResult<AccessRuleItem>> GetAccessRulesAsync(AccessObjectType objectType, int objectId, CancellationToken cancellationToken)
        {
            ThrowIfDisposed();
            return Store.GetAccessRulesAsync(objectType, objectId, cancellationToken);
        }

        /// <summary>
        /// Retrieves the <see cref="AccessRuleItem" /> associated with the key, as an asynchronous operation.
        /// </summary>
        /// <param name="item">The item to be found.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken" /> used to propagate notifications that the operation should be canceled.</param>
        /// <returns>
        /// The <see cref="Task" /> for the asynchronous operation, containing the contact, if any which matched the specified key.
        /// </returns>
        public virtual Task<AccessRuleItem> GetAccessRuleAsync(SharedResourceItem item, CancellationToken cancellationToken)
        {
            ThrowIfDisposed();
            if (item == null)
            {
                throw new ArgumentNullException(nameof(item));
            }
            return GetAccessRuleAsync(item.ObjectType, item.Id, cancellationToken);
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
        public virtual async Task<AccessRuleItem> GetAccessRuleAsync(AccessObjectType objectType, int objectId, CancellationToken cancellationToken)
        {
            ThrowIfDisposed();
            return await Store.GetAccessRuleAsync(objectType, objectId, cancellationToken);
        }

        /// <summary>
        /// Retrieves the <see cref="AccessRuleItem" /> associated with the key, as an asynchronous operation.
        /// </summary>
        /// <param name="item">The item to be found.</param>
        /// <param name="userId">The primary key of the user to be found.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken" /> used to propagate notifications that the operation should be canceled.</param>
        /// <returns>
        /// The <see cref="Task" /> for the asynchronous operation, containing the contact, if any which matched the specified key.
        /// </returns>
        public virtual Task<AccessRuleItem> GetAccessRuleByUserIdAsync(SharedResourceItem item, int userId, CancellationToken cancellationToken)
        {
            ThrowIfDisposed();
            if (item == null)
            {
                throw new ArgumentNullException(nameof(item));
            }
            return GetAccessRuleByUserIdAsync(item.ObjectType, item.Id, userId, cancellationToken);
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
        public virtual Task<AccessRuleItem> GetAccessRuleByUserIdAsync(AccessObjectType objectType, int objectId, int userId, CancellationToken cancellationToken)
        {
            ThrowIfDisposed();
            return Store.GetAccessRuleByUserAsync(objectType, objectId, userId, cancellationToken);
        }

        /// <summary>
        /// Retrieves the <see cref="AccessRuleItem" /> associated with the key, as an asynchronous operation.
        /// </summary>
        /// <param name="item">The item to be found.</param>
        /// <param name="userEmail">The email address of the user to be found.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken" /> used to propagate notifications that the operation should be canceled.</param>
        /// <returns>
        /// The <see cref="Task" /> for the asynchronous operation, containing the contact, if any which matched the specified key.
        /// </returns>
        public virtual Task<AccessRuleItem> GetAccessRuleByUserEmailAsync(SharedResourceItem item, string userEmail, CancellationToken cancellationToken)
        {
            ThrowIfDisposed();
            if (item == null)
            {
                throw new ArgumentNullException(nameof(item));
            }
            return GetAccessRuleByUserEmailAsync(item.ObjectType, item.Id, userEmail, cancellationToken);
        }

        /// <summary>
        /// Retrieves the <see cref="AccessRuleItem" /> associated with the key, as an asynchronous operation.
        /// </summary>
        /// <param name="objectType">The type of the shared object to be found.</param>
        /// <param name="objectId">The primary key of the shared object to be found.</param>
        /// <param name="userEmail">The email address of the user to be found.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken" /> used to propagate notifications that the operation should be canceled.</param>
        /// <returns>
        /// The <see cref="Task" /> for the asynchronous operation, containing the contact, if any which matched the specified key.
        /// </returns>
        public virtual async Task<AccessRuleItem> GetAccessRuleByUserEmailAsync(AccessObjectType objectType, int objectId, string userEmail, CancellationToken cancellationToken)
        {
            ThrowIfDisposed();

            // If the specified user does not exist, do not try to return a 'Anyone'.
            // This method should be straight and unambiguous.
            var user = await UserManager.FindByEmailAsync(userEmail, cancellationToken);
            if (user == null)
            {
                return null;
            }
            return await Store.GetAccessRuleByUserAsync(objectType, objectId, user.Id, cancellationToken);
        }

        /// <summary>
        /// Inserts a <see cref="AccessRuleItem" /> to the specified <paramref name="item" />.
        /// </summary>
        /// <param name="item">The item to be shared.</param>
        /// <param name="rule">The ACEs (Access Control Entries) to set.</param>
        /// <param name="cancellationToken">A token to observe while waiting for the task to complete.</param>
        /// <returns>
        /// A task that represents the asynchronous operation.
        /// </returns>
        /// <exception cref="System.ArgumentNullException"></exception>
        public virtual Task<ValidationResult> AddAccessRuleAsync(SharedResourceItem item, AccessRuleItem rule, CancellationToken cancellationToken)
        {
            ThrowIfDisposed();
            if (item == null)
            {
                throw new ArgumentNullException(nameof(item));
            }
            return AddAccessRuleAsync(item.ObjectType, item.Id, rule, cancellationToken);
        }

        /// <summary>
        /// Inserts a <see cref="AccessRuleItem" /> to the specified <paramref name="item" />.
        /// </summary>
        /// <param name="objectType">The type of the shared object to be found.</param>
        /// <param name="objectId">The primary key of the shared object to be found.</param>
        /// <param name="rule">The ACEs (Access Control Entries) to set.</param>
        /// <param name="cancellationToken">A token to observe while waiting for the task to complete.</param>
        /// <returns>
        /// A task that represents the asynchronous operation.
        /// </returns>
        /// <exception cref="System.ArgumentNullException"></exception>
        public virtual async Task<ValidationResult> AddAccessRuleAsync(AccessObjectType objectType, int objectId, AccessRuleItem rule, CancellationToken cancellationToken)
        {
            ThrowIfDisposed();
            if (rule == null)
            {
                throw new ArgumentNullException(nameof(rule));
            }

            var validationResult = await ValidateAccessRuleAsync(objectType, objectId, rule, cancellationToken);
            if (!validationResult.Succeeded)
            {
                return validationResult;
            }
            return await Store.AddAccessRuleAsync(objectType, objectId, rule, cancellationToken);
        }

        /// <summary>
        /// Inserts a list of <see cref="AccessRuleItem" />s to the specified <paramref name="item" />.
        /// </summary>
        /// <param name="item">The item to be shared.</param>
        /// <param name="rule">The ACEs (Access Control Entries) to set.</param>
        /// <param name="cancellationToken">A token to observe while waiting for the task to complete.</param>
        /// <returns>
        /// A task that represents the asynchronous operation.
        /// </returns>
        /// <exception cref="System.ArgumentNullException"></exception>
        public virtual Task<ValidationResult> SetAccessRuleAsync(SharedResourceItem item, AccessRuleItem rule, CancellationToken cancellationToken)
        {
            ThrowIfDisposed();
            if (item == null)
            {
                throw new ArgumentNullException(nameof(item));
            }
            return SetAccessRuleAsync(item.ObjectType, item.Id, rule, cancellationToken);
        }

        /// <summary>
        /// Inserts a list of <see cref="AccessRuleItem" />s to the specified <paramref name="item" />.
        /// </summary>
        /// <param name="objectType">The type of the shared object to be found.</param>
        /// <param name="objectId">The primary key of the shared object to be found.</param>
        /// <param name="rule">The ACEs (Access Control Entries) to set.</param>
        /// <param name="cancellationToken">A token to observe while waiting for the task to complete.</param>
        /// <returns>
        /// A task that represents the asynchronous operation.
        /// </returns>
        /// <exception cref="System.ArgumentNullException"></exception>
        public virtual async Task<ValidationResult> SetAccessRuleAsync(AccessObjectType objectType, int objectId, AccessRuleItem rule, CancellationToken cancellationToken)
        {
            ThrowIfDisposed();
            if (rule == null)
            {
                throw new ArgumentNullException(nameof(rule));
            }

            AccessRuleItem oldRule;
            if (rule.Anyone)
            {
                oldRule = await Store.GetAccessRuleAsync(objectType, objectId, cancellationToken);
            }
            else
            {
                oldRule = await Store.GetAccessRuleByUserAsync(objectType, objectId, rule.User.Id, cancellationToken);
            }
            if (oldRule == null)
            {
                return await AddAccessRuleAsync(objectType, objectId, rule, cancellationToken);
            }

            var validationResult = await ValidateAccessRuleAsync(objectType, objectId, rule, cancellationToken);
            if (!validationResult.Succeeded)
            {
                return validationResult;
            }
            return await Store.ReplaceAccessRuleAsync(objectType, objectId, oldRule, rule, cancellationToken);
        }

        /// <summary>
        /// Deletes a list of <see cref="AccessRuleItem" />s from the specified <paramref name="item" />.
        /// </summary>
        /// <param name="item">The shared to be unshared.</param>
        /// <param name="rule">The ACEs (Access Control Entries) to delete.</param>
        /// <param name="cancellationToken">A token to observe while waiting for the task to complete.</param>
        /// <returns>
        /// A task that represents the asynchronous operation.
        /// </returns>
        public virtual Task<ValidationResult> RemoveAccessRuleAsync(SharedResourceItem item, AccessRuleItem rule, CancellationToken cancellationToken)
        {
            ThrowIfDisposed();
            if (item == null)
            {
                throw new ArgumentNullException(nameof(item));
            }
            return RemoveAccessRuleAsync(item.ObjectType, item.Id, rule, cancellationToken);
        }

        /// <summary>
        /// Deletes a list of <see cref="AccessRuleItem" />s from the specified <paramref name="item" />.
        /// </summary>
        /// <param name="item">The shared to be unshared.</param>
        /// <param name="rule">The ACEs (Access Control Entries) to delete.</param>
        /// <param name="cancellationToken">A token to observe while waiting for the task to complete.</param>
        /// <returns>
        /// A task that represents the asynchronous operation.
        /// </returns>
        public virtual async Task<ValidationResult> RemoveAccessRuleAsync(AccessObjectType objectType, int objectId, AccessRuleItem rule, CancellationToken cancellationToken)
        {
            ThrowIfDisposed();
            if (rule == null)
            {
                throw new ArgumentNullException(nameof(rule));
            }

            // At least one specific user is required as owner of the specified object.
            if (!rule.Anyone && rule.Permission == AccessPermission.IsOwner)
            {
                var owners = await Store.GetAccessRulesAsync(objectType, objectId, cancellationToken);
                if (owners.Data.Count(o => !o.Anyone && o.Permission == AccessPermission.IsOwner) < 2)
                {
                    return ValidationResult.Failed(string.Format(CultureInfo.CurrentCulture, SecurityResources.OwnerRequired));
                }
            }

            return await Store.RemoveAccessRuleAsync(objectType, objectId, rule, cancellationToken);
        }

        /// <summary>
        /// Deletes all <see cref="AccessRuleItem" />s from the specified <paramref name="item" />.
        /// </summary>
        /// <param name="item">The shared to be unshared.</param>
        /// <param name="cancellationToken">A token to observe while waiting for the task to complete.</param>
        /// <returns>
        /// A task that represents the asynchronous operation.
        /// </returns>
        public virtual Task<ValidationResult> RemoveAccessRulesAsync(SharedResourceItem item, CancellationToken cancellationToken)
        {
            ThrowIfDisposed();
            if (item == null)
            {
                throw new ArgumentNullException(nameof(item));
            }
            return RemoveAccessRulesAsync(item.ObjectType, item.Id, cancellationToken);
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
        public virtual Task<ValidationResult> RemoveAccessRulesAsync(AccessObjectType objectType, int objectId, CancellationToken cancellationToken)
        {
            ThrowIfDisposed();
            return Store.RemoveAccessRulesAsync(objectType, objectId, cancellationToken);
        }

        /// <summary>
        /// Validates access rule. Called by other <see cref="SecurityManager" /> methods.
        /// </summary>
        /// <param name="rule">The rule to validate.</param>
        /// <param name="cancellationToken">A token to observe while waiting for the task to complete.</param>
        /// <returns>
        /// A task that represents the asynchronous operation.
        /// </returns>
        private Task<ValidationResult> ValidateAccessRuleAsync(AccessObjectType objectType, int objectId, AccessRuleItem rule, CancellationToken cancellationToken)
        {
            return (Validator == null) ? Task.FromResult(ValidationResult.Success) : Validator.ValidateAsync(this, objectType, objectId, rule, cancellationToken);
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
