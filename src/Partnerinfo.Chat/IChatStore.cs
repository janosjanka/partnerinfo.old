// Copyright (c) János Janka. All rights reserved.

using System;
using System.Threading;
using System.Threading.Tasks;

namespace Partnerinfo.Chat
{
    public interface IChatStore : IDisposable
    {
        /// <summary>
        /// Finds a chat room with the given primary key value.
        /// </summary>
        /// <param name="id">The primary key for the item to be found.</param>
        /// <param name="cancellationToken">A token to observe while waiting for the task to complete.</param>
        /// <returns>
        /// A task that represents the asynchronous operation.
        /// </returns>
        Task<ChatRoomInfo> FindByIdAsync(string id, CancellationToken cancellationToken);

        /// <summary>
        /// Inserts a chat room.
        /// </summary>
        /// <param name="chatRoom">The chat room to add.</param>
        /// <param name="cancellationToken">A token to observe while waiting for the task to complete.</param>        
        /// <returns>
        /// A task that represents the asynchronous operation.
        /// </returns>
        Task CreateAsync(ChatRoomInfo chatRoom, CancellationToken cancellationToken);

        /// <summary>
        /// Updates a chat room.
        /// </summary>
        /// <param name="chatRoom">The chat room to update.</param>
        /// <param name="cancellationToken">A token to observe while waiting for the task to complete.</param>        
        /// <returns>
        /// A task that represents the asynchronous operation.
        /// </returns>
        Task UpdateAsync(ChatRoomInfo chatRoom, CancellationToken cancellationToken);

        /// <summary>
        /// Removes a chat room.
        /// </summary>
        /// <param name="chatRoom">The chat room to remove.</param>
        /// <param name="cancellationToken">A token to observe while waiting for the task to complete.</param>
        /// <returns>
        /// A task that represents the asynchronous operation.
        /// </returns>
        Task DeleteAsync(ChatRoomInfo chatRoom, CancellationToken cancellationToken);
    }
}
