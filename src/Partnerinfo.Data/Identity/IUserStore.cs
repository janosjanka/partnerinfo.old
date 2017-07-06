// Copyright (c) János Janka. All rights reserved.

using System;
using System.Threading;
using System.Threading.Tasks;

namespace Partnerinfo.Identity
{
    public interface IUserStore : IDisposable
    {
        /// <summary>
        /// Inserts a user.
        /// </summary>
        /// <param name="user">The user to insert.</param>
        /// <param name="cancellationToken">A token to observe while waiting for the task to complete.</param>
        /// <returns>
        /// A task that represents the asynchronous operation.
        /// </returns>
        Task<ValidationResult> CreateAsync(UserItem user, CancellationToken cancellationToken);

        /// <summary>
        /// Updates a user.
        /// </summary>
        /// <param name="user">The user to update.</param>
        /// <param name="cancellationToken">A token to observe while waiting for the task to complete.</param>
        /// <returns>
        /// A task that represents the asynchronous operation.
        /// </returns>
        Task<ValidationResult> UpdateAsync(UserItem user, CancellationToken cancellationToken);

        /// <summary>
        /// Deletes a user.
        /// </summary>
        /// <param name="user">The user to delete.</param>
        /// <param name="cancellationToken">A token to observe while waiting for the task to complete.</param>
        /// <returns>
        /// A task that represents the asynchronous operation.
        /// </returns>
        Task<ValidationResult> DeleteAsync(UserItem user, CancellationToken cancellationToken);

        /// <summary>
        /// Retrieves the user associated with the key, as an asynchronous operation.
        /// </summary>
        /// <param name="id">The id provided by the <paramref name="id"/> to identify a user.</param>
        /// <param name="fields">The related fields to include in the query results.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/> used to propagate notifications that the operation should be canceled.</param>
        /// <returns>
        /// The <see cref="Task"/> for the asynchronous operation, containing the user, if any which matched the specified key.
        /// </returns>
        Task<UserItem> FindByIdAsync(int id, UserField fields, CancellationToken cancellationToken);

        /// <summary>
        /// Retrieves the user associated with the email address, as an asynchronous operation.
        /// </summary>
        /// <param name="email">The email address to return the user for.</param>
        /// <param name="fields">The related fields to include in the query results.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/> used to propagate notifications that the operation should be canceled.</param>
        /// <returns>
        /// The <see cref="Task"/> for the asynchronous operation, containing the user, if any which matched the specified key.
        /// </returns>
        Task<UserItem> FindByEmailAsync(string email, UserField fields, CancellationToken cancellationToken);

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
        Task<ListResult<UserItem>> FindAllAsync(string name, UserSortOrder orderBy, int pageIndex, int pageSize, UserField fields, CancellationToken cancellationToken);
    }
}
