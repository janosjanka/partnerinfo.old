// Copyright (c) János Janka. All rights reserved.

using System;
using System.Threading;
using System.Threading.Tasks;

namespace Partnerinfo.Security
{
    /// <summary>
    /// Provides an abstraction for a store of ACEs (Access Control Entries) for both user and resource.
    /// </summary>
    public interface IAccessRuleStore : IDisposable
    {
        /// <summary>
        /// Finds a collection of Access Control Entries (ACEs) with the given values.
        /// </summary>
        /// <param name="objectType">The type of the shared object to be found.</param>
        /// <param name="objectId">The primary key of the shared object to be found.</param>
        /// <param name="cancellationToken">A token to observe while waiting for the task to complete.</param>
        /// <returns>
        /// A task that represents the asynchronous operation.
        /// </returns>
        Task<ListResult<AccessRuleItem>> GetAccessRulesAsync(AccessObjectType objectType, int objectId, CancellationToken cancellationToken);

        /// <summary>
        /// Retrieves the <see cref="AccessRuleItem" /> associated with the key, as an asynchronous operation.
        /// </summary>
        /// <param name="objectType">The type of the shared object to be found.</param>
        /// <param name="objectId">The primary key of the shared object to be found.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken" /> used to propagate notifications that the operation should be canceled.</param>
        /// <returns>
        /// The <see cref="Task" /> for the asynchronous operation, containing the contact, if any which matched the specified key.
        /// </returns>
        Task<AccessRuleItem> GetAccessRuleAsync(AccessObjectType objectType, int objectId, CancellationToken cancellationToken);

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
        Task<AccessRuleItem> GetAccessRuleByUserAsync(AccessObjectType objectType, int objectId, int userId, CancellationToken cancellationToken);

        /// <summary>
        /// Inserts a list of <see cref="AccessRuleItem" />s to the specified <paramref name="item" />.
        /// </summary>
        /// <param name="objectType">The type of the shared object to be found.</param>
        /// <param name="objectId">The primary key for the shared object to be found.</param>
        /// <param name="rule">The ACE (Access Control Entry) to insert.</param>
        /// <param name="cancellationToken">A token to observe while waiting for the task to complete.</param>
        /// <returns>
        /// A task that represents the asynchronous operation.
        /// </returns>
        Task<ValidationResult> AddAccessRuleAsync(AccessObjectType objectType, int objectId, AccessRuleItem rule, CancellationToken cancellationToken);

        /// <summary>
        /// Replaces the given <paramref name="rule" /> on the specified object with the <paramref name="newRule" />.
        /// </summary>
        /// <param name="objectType">The type of the shared object to be found.</param>
        /// <param name="objectId">The primary key for the shared object to be found.</param>
        /// <param name="rule">The ACE (Access Control Entry) to replace.</param>
        /// <param name="newRule">The new ACE (Access Control Entry) to set.</param>
        /// <param name="cancellationToken">A token to observe while waiting for the task to complete.</param>
        /// <returns>
        /// A task that represents the asynchronous operation.
        /// </returns>
        Task<ValidationResult> ReplaceAccessRuleAsync(AccessObjectType objectType, int objectId, AccessRuleItem rule, AccessRuleItem newRule, CancellationToken cancellationToken);

        /// <summary>
        /// Deletes a list of <see cref="AccessRuleItem" />s from the specified <paramref name="item" />.
        /// </summary>
        /// <param name="objectType">The type of the shared object to be found.</param>
        /// <param name="objectId">The primary key for the shared object to be found.</param>
        /// <param name="rule">The ACE (Access Control Entry) to delete.</param>
        /// <param name="cancellationToken">A token to observe while waiting for the task to complete.</param>
        /// <returns>
        /// A task that represents the asynchronous operation.
        /// </returns>
        Task<ValidationResult> RemoveAccessRuleAsync(AccessObjectType objectType, int objectId, AccessRuleItem rule, CancellationToken cancellationToken);

        /// <summary>
        /// Deletes all <see cref="AccessRuleItem" />s from the specified <paramref name="item" />.
        /// </summary>
        /// <param name="objectType">The type of the shared object to be found.</param>
        /// <param name="objectId">The primary key for the shared object to be found.</param>
        /// <param name="cancellationToken">A token to observe while waiting for the task to complete.</param>
        /// <returns>
        /// A task that represents the asynchronous operation.
        /// </returns>
        Task<ValidationResult> RemoveAccessRulesAsync(AccessObjectType objectType, int objectId, CancellationToken cancellationToken);
    }
}
