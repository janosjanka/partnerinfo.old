// Copyright (c) János Janka. All rights reserved.

using System.Threading;
using System.Threading.Tasks;

namespace Partnerinfo.Chat
{
    public interface IChatMessageStore : IChatUserStore
    {
        /// <summary>
        /// Inserts a chat message.
        /// </summary>
        /// <param name="chatRoom">The chat room which contains the chat message.</param>
        /// <param name="chatMessage">The chat message to add.</param>
        /// <param name="cancellationToken">A token to observe while waiting for the task to complete.</param>
        /// <returns>
        /// A task that represents the asynchronous operation.
        /// </returns>
        Task AddMessageAsync(ChatRoomInfo chatRoom, ChatMessageInfo chatMessage, CancellationToken cancellationToken);

        /// <summary>
        /// Removes a chat message.
        /// </summary>
        /// <param name="chatRoom">The chat room which contains the chat message.</param>
        /// <param name="chatMessage">The chat message to remove.</param>
        /// <param name="cancellationToken">A token to observe while waiting for the task to complete.</param>
        /// <returns>
        /// A task that represents the asynchronous operation.
        /// </returns>        
        Task RemoveMessageAsync(ChatRoomInfo chatRoom, ChatMessageInfo chatMessage, CancellationToken cancellationToken);
    }
}
