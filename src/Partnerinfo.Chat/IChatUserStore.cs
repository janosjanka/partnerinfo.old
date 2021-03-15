// Copyright (c) János Janka. All rights reserved.

using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Partnerinfo.Chat
{
    public interface IChatUserStore : IChatStore
    {
        /// <summary>
        /// Gets all the users for the given chat room.
        /// </summary>
        /// <param name="chatRoom">The chat room which contains the chat users.</param>
        /// <param name="cancellationToken">A token to observe while waiting for the task to complete.</param>
        /// <returns>
        /// A task that represents the asynchronous operation.
        /// </returns>
        Task<IList<ChatUserInfo>> GetUsersAsync(ChatRoomInfo chatRoom, CancellationToken cancellationToken);

        /// <summary>
        /// Gets the information from the cache for the chat user associated with the specified unique identifier.
        /// </summary>
        /// <param name="chatRoom">The chat room which owns the user.</param>
        /// <param name="userName">The user name for the item to be found.</param>
        /// <param name="cancellationToken">A token to observe while waiting for the task to complete.</param>
        /// <returns>
        /// A task that represents the asynchronous operation.
        /// </returns>
        Task<ChatUserInfo> GetUserByUserNameAsync(ChatRoomInfo chatRoom, string userName, CancellationToken cancellationToken);

        /// <summary>
        /// Inserts a chat user.
        /// </summary>
        /// <param name="chatRoom">The chat room which contains the user.</param>
        /// <param name="chatUser">The chat user to add.</param>
        /// <param name="cancellationToken">A token to observe while waiting for the task to complete.</param>
        /// <returns>
        /// A task that represents the asynchronous operation.
        /// </returns>
        Task AddUserAsync(ChatRoomInfo chatRoom, ChatUserInfo chatUser, CancellationToken cancellationToken);

        /// <summary>
        /// Removes a chat user.
        /// </summary>
        /// <param name="chatRoom">The chat room which contains the user.</param>
        /// <param name="chatUser">The chat user to remove.</param>
        /// <param name="cancellationToken">A token to observe while waiting for the task to complete.</param>
        /// <returns>
        /// A task that represents the asynchronous operation.
        /// </returns>        
        Task RemoveUserAsync(ChatRoomInfo chatRoom, ChatUserInfo chatUser, CancellationToken cancellationToken);
    }
}
