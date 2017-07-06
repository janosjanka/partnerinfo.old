// Copyright (c) János Janka. All rights reserved.

using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Runtime.Caching;
using System.Threading;
using System.Threading.Tasks;

namespace Partnerinfo.Chat
{
    public class ChatMemoryStore :
        IChatStore,
        IChatUserStore,
        IChatConnectionStore,
        IChatMessageStore
    {
        private static readonly MemoryCache s_rooms = new MemoryCache("ChatRooms");
        private static readonly MemoryCache s_users = new MemoryCache("ChatUsers");
        private static readonly MemoryCache s_connections = new MemoryCache("ChatConnections");
        private static readonly MemoryCache s_roomConnections = new MemoryCache("ChatRoomConnections");
        private static readonly MemoryCache s_userConnections = new MemoryCache("ChatUserConnections");

        private bool _disposed;

        //
        // IChatRoomStore
        //

        /// <summary>
        /// Finds a chat room with the given primary key value.
        /// </summary>
        /// <param name="id">The primary key for the item to be found.</param>
        /// <param name="cancellationToken">A token to observe while waiting for the task to complete.</param>
        /// <returns>
        /// A task that represents the asynchronous operation.
        /// </returns>
        public virtual Task<ChatRoomInfo> FindByIdAsync(string id, CancellationToken cancellationToken)
        {
            ThrowIfDisposed();
            return Task.FromResult(s_rooms.Get(id) as ChatRoomInfo);
        }

        /// <summary>
        /// Inserts a chat room.
        /// </summary>
        /// <param name="chatRoom">The chat room to add.</param>
        /// <param name="cancellationToken">A token to observe while waiting for the task to complete.</param>        
        /// <returns>
        /// A task that represents the asynchronous operation.
        /// </returns>
        public virtual Task CreateAsync(ChatRoomInfo chatRoom, CancellationToken cancellationToken)
        {
            ThrowIfDisposed();
            if (chatRoom == null)
            {
                throw new ArgumentNullException("chatRoom");
            }
            s_rooms.Add(chatRoom.Id, chatRoom, DateTimeOffset.MaxValue);
            return Task.FromResult(0);
        }

        /// <summary>
        /// Updates a chat room.
        /// </summary>
        /// <param name="chatRoom">The chat room to update.</param>
        /// <param name="cancellationToken">A token to observe while waiting for the task to complete.</param>        
        /// <returns>
        /// A task that represents the asynchronous operation.
        /// </returns>
        public virtual Task UpdateAsync(ChatRoomInfo chatRoom, CancellationToken cancellationToken)
        {
            ThrowIfDisposed();
            if (chatRoom == null)
            {
                throw new ArgumentNullException("chatRoom");
            }
            //Rooms.Set(chatRoom.Id, chatRoom, DateTimeOffset.MaxValue);
            return Task.FromResult(0);
        }

        /// <summary>
        /// Removes a chat room.
        /// </summary>
        /// <param name="chatRoom">The chat room to remove.</param>
        /// <param name="cancellationToken">A token to observe while waiting for the task to complete.</param>
        /// <returns>
        /// A task that represents the asynchronous operation.
        /// </returns>
        public virtual Task DeleteAsync(ChatRoomInfo chatRoom, CancellationToken cancellationToken)
        {
            ThrowIfDisposed();
            if (chatRoom == null)
            {
                throw new ArgumentNullException("chatRoom");
            }
            s_rooms.Remove(chatRoom.Id);
            return Task.FromResult(0);
        }

        //
        // IChatRoomConnectionStore
        //

        /// <summary>
        /// Gets all the connections for the given chat room.
        /// </summary>
        /// <param name="chatRoom">The chat room.</param>
        /// <param name="cancellationToken">A token to observe while waiting for the task to complete.</param>
        /// <returns>
        /// A task that represents the asynchronous operation.
        /// </returns>
        public virtual Task<IList<string>> GetConnectionsAsync(ChatRoomInfo chatRoom, CancellationToken cancellationToken)
        {
            ThrowIfDisposed();
            if (chatRoom == null)
            {
                throw new ArgumentNullException("chatRoom");
            }

            var connections = s_roomConnections.Get(chatRoom.Id) as ImmutableHashSet<string>;
            if (connections != null)
            {
                return Task.FromResult<IList<string>>(connections.ToList());
            }
            return Task.FromResult<IList<string>>(new List<string>());
        }

        /// <summary>
        /// Gets all the connections for the given chat user.
        /// </summary>
        /// <param name="chatUser">The chat room.</param>
        /// <param name="userName">The user name.</param>
        /// <param name="cancellationToken">A token to observe while waiting for the task to complete.</param>
        /// <returns>
        /// A task that represents the asynchronous operation.
        /// </returns>
        public virtual Task<IList<string>> GetConnectionsByUserAsync(ChatUserInfo chatUser, CancellationToken cancellationToken)
        {
            ThrowIfDisposed();
            if (chatUser == null)
            {
                throw new ArgumentNullException("chatUser");
            }

            var connections = s_userConnections.Get(chatUser.RoomId + chatUser.UserName) as ImmutableHashSet<string>;
            if (connections != null)
            {
                return Task.FromResult<IList<string>>(connections.ToList());
            }
            return Task.FromResult<IList<string>>(new List<string>());
        }

        /// <summary>
        /// Gets a connection for the given chat room.
        /// </summary>
        /// <param name="id">The connection ID.</param>
        /// <param name="cancellationToken">A token to observe while waiting for the task to complete.</param>
        /// <returns>
        /// A task that represents the asynchronous operation.
        /// </returns>
        public virtual Task<ChatConnection> GetConnectionByIdAsync(string id, CancellationToken cancellationToken)
        {
            ThrowIfDisposed();
            if (string.IsNullOrEmpty(id))
            {
                throw new ArgumentNullException("id");
            }
            return Task.FromResult(s_connections.Get(id) as ChatConnection);
        }

        /// <summary>
        /// Adds a connection ID to the given chat room.
        /// </summary>
        /// <param name="connectionId">The connection to add.</param>
        /// <param name="chatUser">The chat room.</param>
        /// <param name="cancellationToken">A token to observe while waiting for the task to complete.</param>
        /// <returns>
        /// A task that represents the asynchronous operation.
        /// </returns>
        public virtual Task AddConnectionToUserAsync(string connectionId, ChatUserInfo chatUser, CancellationToken cancellationToken)
        {
            ThrowIfDisposed();
            if (string.IsNullOrEmpty(connectionId))
            {
                throw new ArgumentNullException("connectionId");
            }
            if (chatUser == null)
            {
                throw new ArgumentNullException("chatUser");
            }

            var roomKey = chatUser.RoomId;
            var roomUserKey = chatUser.RoomId + chatUser.UserName;
            var roomConnectionIds = s_roomConnections.Get(roomKey) as ImmutableHashSet<string>;
            var userConnectionIds = s_userConnections.Get(roomUserKey) as ImmutableHashSet<string>;
            roomConnectionIds = roomConnectionIds != null ? roomConnectionIds.Add(connectionId) : ImmutableHashSet.Create(connectionId);
            userConnectionIds = userConnectionIds != null ? userConnectionIds.Add(connectionId) : ImmutableHashSet.Create(connectionId);

            // Add the connection to the global connection cache, the room connection cache,
            // and the user connection cache because the memory cache does not support AppFabric-style tagging
            // This is used by the GetConnectionByIdAsync method because SignalR Hub's OnDisconnected event
            // can only work with a ConnectionID

            s_connections.Set(connectionId, new ChatConnection { RoomId = chatUser.RoomId, UserName = chatUser.UserName }, DateTimeOffset.MaxValue);
            s_roomConnections.Set(roomKey, roomConnectionIds, DateTimeOffset.MaxValue);
            s_userConnections.Set(roomUserKey, userConnectionIds, DateTimeOffset.MaxValue);

            return Task.FromResult(0);
        }

        /// <summary>
        /// Removes a connection from the given chat user.
        /// </summary>
        /// <param name="connectionId">The connection to remove.</param>
        /// <param name="chatUser">The chat user.</param>
        /// <param name="cancellationToken">A token to observe while waiting for the task to complete.</param>
        /// <returns>
        /// A task that represents the asynchronous operation.
        /// </returns>
        public virtual Task RemoveConnectionFromUserAsync(string connectionId, ChatUserInfo chatUser, CancellationToken cancellationToken)
        {
            ThrowIfDisposed();
            if (string.IsNullOrEmpty(connectionId))
            {
                throw new ArgumentNullException("connectionId");
            }
            if (chatUser == null)
            {
                throw new ArgumentNullException("chatUser");
            }

            var roomKey = chatUser.RoomId;
            var roomConnectionIds = s_roomConnections.Get(roomKey) as ImmutableHashSet<string>;
            if (roomConnectionIds != null)
            {
                roomConnectionIds = roomConnectionIds.Remove(connectionId);
                s_roomConnections.Set(roomKey, roomConnectionIds, DateTimeOffset.MaxValue);
            }

            var roomUserKey = chatUser.RoomId + chatUser.UserName;
            var userConnectionIds = s_userConnections.Get(roomUserKey) as ImmutableHashSet<string>;
            if (userConnectionIds != null)
            {
                userConnectionIds = userConnectionIds.Remove(connectionId);
                s_userConnections.Set(roomUserKey, userConnectionIds, DateTimeOffset.MaxValue);
            }

            s_connections.Remove(connectionId);
            return Task.FromResult(0);
        }

        //
        // IChatRoomUserStore
        //

        /// <summary>
        /// Gets all the users for the given chat room.
        /// </summary>
        /// <param name="chatRoom">The chat room which contains the chat users.</param>
        /// <param name="cancellationToken">A token to observe while waiting for the task to complete.</param>
        /// <returns>
        /// A task that represents the asynchronous operation.
        /// </returns>
        public virtual Task<IList<ChatUserInfo>> GetUsersAsync(ChatRoomInfo chatRoom, CancellationToken cancellationToken)
        {
            ThrowIfDisposed();
            if (chatRoom == null)
            {
                throw new ArgumentNullException("chatRoom");
            }

            var users = s_users.Get(chatRoom.Id) as ConcurrentDictionary<string, ChatUserInfo>;
            if (users != null)
            {
                return Task.FromResult<IList<ChatUserInfo>>(users.Values.ToList());
            }
            return Task.FromResult<IList<ChatUserInfo>>(new List<ChatUserInfo>());
        }

        /// <summary>
        /// Gets the information from the cache for the chat user associated with the specified unique identifier.
        /// </summary>
        /// <param name="chatRoom">The chat room which owns the user.</param>
        /// <param name="userName">The user name for the item to be found.</param>
        /// <param name="cancellationToken">A token to observe while waiting for the task to complete.</param>
        /// <returns>
        /// A task that represents the asynchronous operation.
        /// </returns>
        public virtual Task<ChatUserInfo> GetUserByUserNameAsync(ChatRoomInfo chatRoom, string userName, CancellationToken cancellationToken)
        {
            ThrowIfDisposed();
            if (chatRoom == null)
            {
                throw new ArgumentNullException("chatRoom");
            }
            if (userName == null)
            {
                throw new ArgumentNullException("userName");
            }

            var user = default(ChatUserInfo);
            var users = s_users.Get(chatRoom.Id) as ConcurrentDictionary<string, ChatUserInfo>;
            if (users != null)
            {
                users.TryGetValue(userName, out user);
            }
            return Task.FromResult(user);
        }

        /// <summary>
        /// Inserts a chat user.
        /// </summary>
        /// <param name="chatRoom">The chat room which contains the user.</param>
        /// <param name="chatUser">The chat user to add.</param>
        /// <param name="cancellationToken">A token to observe while waiting for the task to complete.</param>
        /// <returns>
        /// A task that represents the asynchronous operation.
        /// </returns>
        public virtual Task AddUserAsync(ChatRoomInfo chatRoom, ChatUserInfo chatUser, CancellationToken cancellationToken)
        {
            ThrowIfDisposed();
            if (chatRoom == null)
            {
                throw new ArgumentNullException("chatRoom");
            }
            if (chatUser == null)
            {
                throw new ArgumentNullException("chatUser");
            }

            chatUser.RoomId = chatRoom.Id;

            var users = s_users.Get(chatRoom.Id) as ConcurrentDictionary<string, ChatUserInfo>;
            if (users == null)
            {
                users = new ConcurrentDictionary<string, ChatUserInfo>();
            }
            if (users.TryAdd(chatUser.UserName, chatUser))
            {
                s_users.Set(chatRoom.Id, users, DateTimeOffset.MaxValue);
            }
            return Task.FromResult(0);
        }

        /// <summary>
        /// Removes a chat user.
        /// </summary>
        /// <param name="chatRoom">The chat room which contains the user.</param>
        /// <param name="chatUser">The chat user to remove.</param>
        /// <param name="cancellationToken">A token to observe while waiting for the task to complete.</param>
        /// <returns>
        /// A task that represents the asynchronous operation.
        /// </returns>
        public virtual Task RemoveUserAsync(ChatRoomInfo chatRoom, ChatUserInfo chatUser, CancellationToken cancellationToken)
        {
            ThrowIfDisposed();
            if (chatRoom == null)
            {
                throw new ArgumentNullException("chatRoom");
            }
            if (chatUser == null)
            {
                throw new ArgumentNullException("chatUser");
            }

            var users = s_users.Get(chatRoom.Id) as ConcurrentDictionary<string, ChatUserInfo>;
            if (users != null && users.TryRemove(chatUser.UserName, out chatUser))
            {
                s_users.Set(chatRoom.Id, users, DateTimeOffset.MaxValue);
            }
            return Task.FromResult(0);
        }

        //
        // IChatMessageStore
        //

        /// <summary>
        /// Inserts a chat message.
        /// </summary>
        /// <param name="chatRoom">The chat room which contains the chat message.</param>
        /// <param name="chatMessage">The chat message to add.</param>
        /// <param name="cancellationToken">A token to observe while waiting for the task to complete.</param>
        /// <returns>
        /// A task that represents the asynchronous operation.
        /// </returns>
        public virtual Task AddMessageAsync(ChatRoomInfo chatRoom, ChatMessageInfo chatMessage, CancellationToken cancellationToken)
        {
            ThrowIfDisposed();
            if (chatRoom == null)
            {
                throw new ArgumentNullException("chatRoom");
            }
            if (chatMessage == null)
            {
                throw new ArgumentNullException("chatMessage");
            }
            return Task.FromResult(0);
        }

        /// <summary>
        /// Removes a chat message.
        /// </summary>
        /// <param name="chatRoom">The chat room which contains the chat message.</param>
        /// <param name="chatMessage">The chat message to remove.</param>
        /// <param name="cancellationToken">A token to observe while waiting for the task to complete.</param>
        /// <returns>
        /// A task that represents the asynchronous operation.
        /// </returns>        
        public virtual Task RemoveMessageAsync(ChatRoomInfo chatRoom, ChatMessageInfo chatMessage, CancellationToken cancellationToken)
        {
            ThrowIfDisposed();
            if (chatRoom == null)
            {
                throw new ArgumentNullException("chatRoom");
            }
            if (chatMessage == null)
            {
                throw new ArgumentNullException("chatMessage");
            }
            return Task.FromResult(0);
        }

        /// <summary>
        /// Throws a <see cref="ObjectDisposedException" /> if the context has already been disposed.
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
            _disposed = true;
        }
    }
}
