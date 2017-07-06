// Copyright (c) János Janka. All rights reserved.

using System;
using System.Threading;
using System.Threading.Tasks;

namespace Partnerinfo.Input
{
    public interface ICommandStore : IDisposable
    {
        /// <summary>
        /// Finds a command with the given primary key value.
        /// </summary>
        /// <param name="id">The primary key for the item to be found.</param>
        /// <param name="cancellationToken">A token to observe while waiting for the task to complete.</param>
        /// <returns>
        /// A task that represents the asynchronous operation.
        /// </returns>
        Task<CommandItem> FindByIdAsync(int id, CancellationToken cancellationToken);

        /// <summary>
        /// Finds a command with the given primary key value.
        /// </summary>
        /// <param name="uri">The primary key for the item to be found.</param>
        /// <param name="cancellationToken">A token to observe while waiting for the task to complete.</param>
        /// <returns>
        /// A task that represents the asynchronous operation.
        /// </returns>
        Task<CommandItem> FindByUriAsync(string uri, CancellationToken cancellationToken);

        /// <summary>
        /// Inserts a command.
        /// </summary>
        /// <param name="command">The command to insert.</param>
        /// <param name="cancellationToken">A token to observe while waiting for the task to complete.</param>
        /// <returns>
        /// A task that represents the asynchronous operation.
        /// </returns>
        Task<ValidationResult> CreateAsync(CommandItem command, CancellationToken cancellationToken);

        /// <summary>
        /// Updates a command.
        /// </summary>
        /// <param name="command">The command to update.</param>
        /// <param name="cancellationToken">A token to observe while waiting for the task to complete.</param>
        /// <returns>
        /// A task that represents the asynchronous operation.
        /// </returns>
        Task<ValidationResult> UpdateAsync(CommandItem command, CancellationToken cancellationToken);

        /// <summary>
        /// Deletes a command.
        /// </summary>
        /// <param name="command">The command to delete.</param>
        /// <param name="cancellationToken">A token to observe while waiting for the task to complete.</param>
        /// <returns>
        /// A task that represents the asynchronous operation.
        /// </returns>
        Task<ValidationResult> DeleteAsync(CommandItem command, CancellationToken cancellationToken);

        /// <summary>
        /// Removes all the expired commands.
        /// </summary>
        /// <param name="cancellationToken">A token to observe while waiting for the task to complete.</param>
        /// <returns>
        /// A task that represents the asynchronous operation.
        /// </returns>
        Task<ValidationResult> CleanAsync(CancellationToken cancellationToken);
    }
}
