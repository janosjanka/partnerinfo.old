// Copyright (c) János Janka. All rights reserved.

using System;
using System.Threading;
using System.Threading.Tasks;

namespace Partnerinfo.Logging
{
    public interface IRuleStore : IDisposable
    {
        /// <summary>
        /// Inserts a rule.
        /// </summary>
        /// <param name="rule">The rule to insert.</param>
        /// <param name="user">The user who owns the rule.</param>
        /// <param name="cancellationToken">A token to observe while waiting for the task to complete.</param>
        /// <returns>
        /// A task that represents the asynchronous operation.
        /// </returns>
        Task<ValidationResult> CreateAsync(RuleItem rule, AccountItem user, CancellationToken cancellationToken);

        /// <summary>
        /// Updates a rule.
        /// </summary>
        /// <param name="rule">The rule to update.</param>
        /// <param name="cancellationToken">A token to observe while waiting for the task to complete.</param>
        /// <returns>
        /// A task that represents the asynchronous operation.
        /// </returns>
        Task<ValidationResult> UpdateAsync(RuleItem rule, CancellationToken cancellationToken);

        /// <summary>
        /// Deletes a rule.
        /// </summary>
        /// <param name="rule">The rule to delete.</param>
        /// <param name="cancellationToken">A token to observe while waiting for the task to complete.</param>
        /// <returns>
        /// A task that represents the asynchronous operation.
        /// </returns>
        Task<ValidationResult> DeleteAsync(RuleItem rule, CancellationToken cancellationToken);

        /// <summary>
        /// Finds a rule with the given primary key value.
        /// </summary>
        /// <param name="id">The primary key for the item to be found.</param>
        /// <param name="fields">The related fields to include in the query results.</param>
        /// <param name="cancellationToken">A token to observe while waiting for the task to complete.</param>
        /// <returns>
        /// A task that represents the asynchronous operation.
        /// </returns>
        Task<RuleItem> FindByIdAsync(int id, RuleField fields, CancellationToken cancellationToken);

        /// <summary>
        /// Finds a collection of rules with the given values.
        /// </summary>
        /// <param name="userId">The user identifier for the item to be found.</param>
        /// <param name="fields">The related fields to include in the query results.</param>
        /// <param name="cancellationToken">A token to observe while waiting for the task to complete.</param>
        /// <returns>
        /// A task that represents the asynchronous operation.
        /// </returns>
        Task<ListResult<RuleItem>> FindAllAsync(int userId, RuleField fields, CancellationToken cancellationToken);
    }
}
