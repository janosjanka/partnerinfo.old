// Copyright (c) János Janka. All rights reserved.

using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Partnerinfo.Chat
{
    public interface IChatConnectionStore : IChatUserStore
    {
        /// <summary>
        /// Gets all the connections for the given chat room.
        /// </summary>
        /// <param name="chatRoom">The chat room.</param>
        /// <param name="cancellationToken">A token to observe while waiting for the task to complete.</param>
        /// <returns>
        /// A task that represents the asynchronous operation.
        /// </returns>
        Task<IList<string>> GetConnectionsAsync(ChatRoomInfo chatRoom, CancellationToken cancellationToken);

        /// <summary>
        /// Gets all the connections for the given chat user.
        /// </summary>
        /// <param name="chatUser">The chat room.</param>
        /// <param name="userName">The user name.</param>
        /// <param name="cancellationToken">A token to observe while waiting for the task to complete.</param>
        /// <returns>
        /// A task that represents the asynchronous operation.
        /// </returns>
        Task<IList<string>> GetConnectionsByUserAsync(ChatUserInfo chatUser, CancellationToken cancellationToken);

        /// <summary>
        /// Gets a connection for the given chat room.
        /// </summary>
        /// <param name="id">The connection ID.</param>
        /// <param name="cancellationToken">A token to observe while waiting for the task to complete.</param>
        /// <returns>
        /// A task that represents the asynchronous operation.
        /// </returns>
        Task<ChatConnection> GetConnectionByIdAsync(string id, CancellationToken cancellationToken);

        /// <summary>
        /// Adds a connection ID to the given chat user.
        /// </summary>
        /// <param name="connectionId">The connection to add.</param>
        /// <param name="chatUser">The chat user.</param>
        /// <param name="cancellationToken">A token to observe while waiting for the task to complete.</param>
        /// <returns>
        /// A task that represents the asynchronous operation.
        /// </returns>
        Task AddConnectionToUserAsync(string connectionId, ChatUserInfo chatUser, CancellationToken cancellationToken);

        /// <summary>
        /// Removes a connection from the given chat user.
        /// </summary>
        /// <param name="connectionId">The connection to remove.</param>
        /// <param name="chatUser">The chat user.</param>
        /// <param name="cancellationToken">A token to observe while waiting for the task to complete.</param>
        /// <returns>
        /// A task that represents the asynchronous operation.
        /// </returns>
        Task RemoveConnectionFromUserAsync(string connectionId, ChatUserInfo chatUser, CancellationToken cancellationToken);
    }
}
