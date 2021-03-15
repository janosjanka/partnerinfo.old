// Copyright (c) János Janka. All rights reserved.

using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Partnerinfo.Logging;
using Partnerinfo.Portal;
using Partnerinfo.Project;

namespace Partnerinfo.Chat
{
    public class ChatManager
    {
        private bool _disposed;

        /// <summary>
        /// The store that the <see cref="ChatManager" /> operates against.
        /// </summary>
        protected IChatStore Store { get; set; }

        /// <summary>
        /// The manager that the <see cref="ChatManager" /> operates against.
        /// </summary>
        public Func<LogManager> LogManagerFactory { get; set; }

        /// <summary>
        /// The manager that the <see cref="ChatManager" /> operates against.
        /// </summary>
        public Func<ProjectManager> ProjectManagerFactory { get; set; }

        /// <summary>
        /// The manager that the <see cref="ChatManager" /> operates against.
        /// </summary>
        public Func<PortalManager> PortalManagerFactory { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="ChatManager" /> class
        /// </summary>
        /// <param name="store">The cache that the <see cref="ChatManager" /> operates against.</param>
        public ChatManager(IChatStore store)
        {
            if (store == null)
            {
                throw new ArgumentNullException(nameof(store));
            }

            Store = store;
        }

        /// <summary>
        /// Gets a chat room for the given site.
        /// </summary>
        /// <param name="id">The primary key for the item to be found.</param>
        /// <param name="cancellationToken">A token to observe while waiting for the task to complete.</param>
        /// <returns>
        /// The chat room.
        /// </returns>
        public virtual Task<ChatRoomInfo> FindByIdAsync(string id, CancellationToken cancellationToken)
        {
            ThrowIfDisposed();
            if (string.IsNullOrEmpty(id))
            {
                throw new ArgumentNullException("uri");
            }
            return Store.FindByIdAsync(id, cancellationToken);
        }

        /// <summary>
        /// Creates a room for the given page.
        /// </summary>
        /// <param name="portalUri">The portal URI.</param>
        /// <param name="pageUri">The primary key for the item to be found.</param>
        /// <param name="cancellationToken">A token to observe while waiting for the task to complete.</param>
        /// <returns>
        /// The created chat room.
        /// </returns>
        /// <exception cref="InvalidOperationException">
        /// </exception>
        public virtual async Task<ChatRoomInfo> CreateByPageUriAsync(string portalUri, string pageUri, CancellationToken cancellationToken)
        {
            ThrowIfDisposed();
            var manager = GetPortalManager();
            var portal = await manager.FindByUriAsync(portalUri, cancellationToken);
            if (portal == null)
            {
                throw new InvalidOperationException($"The portal '{portalUri}' does not exist.");
            }
            var page = await manager.GetPageByUriAsync(portal, pageUri, cancellationToken);
            if (page == null)
            {
                throw new InvalidOperationException($"The page '{portalUri}/{pageUri}' does not exist.");
            }
            return await CreateAsync(manager, portal, page, cancellationToken);
        }

        /// <summary>
        /// Creates a room for the given page.
        /// </summary>
        /// <param name="manager">The portal manager.</param>
        /// <param name="portal">The site.</param>
        /// <param name="page">The page.</param>
        /// <param name="cancellationToken">A token to observe while waiting for the task to complete.</param>
        /// <returns>
        /// The created chat room.
        /// </returns>
        private async Task<ChatRoomInfo> CreateAsync(PortalManager manager, PortalItem portal, PageItem page, CancellationToken cancellationToken)
        {
            var chatRoom = new ChatRoomInfo
            {
                Id = portal.Id.ToString(),
                PageId = page.Id,
                PortalId = portal.Id,
                ProjectId = portal.Project?.Id
            };
            foreach (var account in portal.Owners)
            {
                chatRoom.Users.Add(account.Email.Address, account);
            }
            await Store.CreateAsync(chatRoom, cancellationToken);
            return chatRoom;
        }

        /// <summary>
        /// Removes a chat room.
        /// </summary>
        /// <param name="chatRoom">The chat room to remove.</param>
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
            return Store.UpdateAsync(chatRoom, cancellationToken);
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
            return Store.DeleteAsync(chatRoom, cancellationToken);
        }

        /// <summary>
        /// Returns true if the given user is an administrator.
        /// </summary>
        /// <param name="chatRoom">The chat room.</param>
        /// <returns>
        /// A value indicating whether the user is an administator.
        /// </returns>
        public virtual bool IsAdmin(ChatRoomInfo chatRoom, string userName)
        {
            ThrowIfDisposed();
            if (chatRoom == null)
            {
                throw new ArgumentNullException("chatRoom");
            }
            return chatRoom.Users.ContainsKey(userName);
        }

        /// <summary>
        /// Gets the IP Address from the request object.
        /// </summary>
        /// <returns>
        /// The IP Address.
        /// </returns>
        public string GenerateClientId(string ipAddress)
        {
            var strBuilder = new StringBuilder();
            for (int i = ipAddress.Length; --i >= 0;)
            {
                char c = ipAddress[i];
                if (c != '.' && c != ':')
                {
                    strBuilder.Append(c);
                }
            }
            return strBuilder.ToString();
        }

        /// <summary>
        /// Gets all the users for the given chat room.
        /// </summary>
        /// <param name="chatRoom">The chat room which contains the chat users.</param>
        /// <param name="cancellationToken">A token to observe while waiting for the task to complete.</param>
        /// <returns>
        /// The chat users.
        /// </returns>
        public virtual async Task<ContactItem> GetContactByUserNameAsync(ChatRoomInfo chatRoom, string userName, CancellationToken cancellationToken)
        {
            ThrowIfDisposed();
            if (ProjectManagerFactory == null)
            {
                throw new InvalidOperationException("ProjectManagerFactory is not supported.");
            }
            var projectManager = ProjectManagerFactory();
            if (chatRoom == null)
            {
                throw new ArgumentNullException("chatRoom");
            }
            if (chatRoom.ProjectId == null)
            {
                return null;
            }
            var project = await projectManager.FindByIdAsync((int)chatRoom.ProjectId, cancellationToken);
            if (project == null)
            {
                throw new InvalidOperationException("The project was not found.");
            }
            return await projectManager.GetContactByMailAsync(project, userName, ContactField.None, cancellationToken);
        }

        /// <summary>
        /// Gets all the users for the given chat room.
        /// </summary>
        /// <param name="chatRoom">The chat room which contains the chat users.</param>
        /// <param name="cancellationToken">A token to observe while waiting for the task to complete.</param>
        /// <returns>
        /// The chat users.
        /// </returns>
        public virtual Task<IList<ChatUserInfo>> GetUsersAsync(ChatRoomInfo chatRoom, CancellationToken cancellationToken)
        {
            ThrowIfDisposed();
            var userStore = GetUserStore();
            if (chatRoom == null)
            {
                throw new ArgumentNullException("chatRoom");
            }
            return userStore.GetUsersAsync(chatRoom, cancellationToken);
        }

        /// <summary>
        /// Gets the information from the cache for the chat user associated with the specified unique identifier.
        /// </summary>
        /// <param name="chatRoom">The chat site which owns the user.</param>
        /// <param name="userName">The user name for the item to be found.</param>
        /// <param name="cancellationToken">A token to observe while waiting for the task to complete.</param>
        /// <returns>
        /// A task that represents the asynchronous operation.
        /// </returns>
        public virtual Task<ChatUserInfo> GetUserByUserNameAsync(ChatRoomInfo chatRoom, string userName, CancellationToken cancellationToken)
        {
            ThrowIfDisposed();
            var userStore = GetUserStore();
            if (chatRoom == null)
            {
                throw new ArgumentNullException("chatRoom");
            }
            if (string.IsNullOrEmpty(userName))
            {
                throw new ArgumentNullException("userName");
            }
            return userStore.GetUserByUserNameAsync(chatRoom, userName, cancellationToken);
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
        public virtual async Task AddUserAsync(ChatRoomInfo chatRoom, ChatUserInfo chatUser, CancellationToken cancellationToken)
        {
            ThrowIfDisposed();
            var userStore = GetUserStore();
            if (chatRoom == null)
            {
                throw new ArgumentNullException("chatRoom");
            }
            if (chatUser == null)
            {
                throw new ArgumentNullException("chatUser");
            }
            await userStore.AddUserAsync(chatRoom, chatUser, cancellationToken);
            await UpdateAsync(chatRoom, cancellationToken);
        }

        /// <summary>
        /// Removes a chat user.
        /// </summary>
        /// <param name="chatRoom">The chat site which contains the user.</param>
        /// <param name="chatUser">The chat user to remove.</param>
        /// <param name="cancellationToken">A token to observe while waiting for the task to complete.</param>
        /// <returns>
        /// A task that represents the asynchronous operation.
        /// </returns>
        public virtual async Task RemoveUserAsync(ChatRoomInfo chatRoom, ChatUserInfo chatUser, CancellationToken cancellationToken)
        {
            ThrowIfDisposed();
            var userStore = GetUserStore();
            if (chatRoom == null)
            {
                throw new ArgumentNullException("chatRoom");
            }
            if (chatUser == null)
            {
                throw new ArgumentNullException("chatUser");
            }
            await userStore.RemoveUserAsync(chatRoom, chatUser, cancellationToken);
            await UpdateAsync(chatRoom, cancellationToken);
        }

        /// <summary>
        /// Gets all the Connection IDs for the given chat room.
        /// </summary>
        /// <param name="chatRoom">The chat room.</param>
        /// <param name="cancellationToken">A token to observe while waiting for the task to complete.</param>
        /// <returns>
        /// A task that represents the asynchronous operation.
        /// </returns>
        public virtual Task<IList<string>> GetConnectionsAsync(ChatRoomInfo chatRoom, CancellationToken cancellationToken)
        {
            ThrowIfDisposed();
            var userConnectionStore = GetUserConnectionStore();
            if (chatRoom == null)
            {
                throw new ArgumentNullException("chatRoom");
            }
            return userConnectionStore.GetConnectionsAsync(chatRoom, cancellationToken);
        }

        /// <summary>
        /// Gets all the Connection IDs for the given chat user.
        /// </summary>
        /// <param name="chatUser">The chat user.</param>
        /// <param name="cancellationToken">A token to observe while waiting for the task to complete.</param>
        /// <returns>
        /// A task that represents the asynchronous operation.
        /// </returns>
        public virtual Task<IList<string>> GetConnectionsByUserAsync(ChatUserInfo chatUser, CancellationToken cancellationToken)
        {
            ThrowIfDisposed();
            var userConnectionStore = GetUserConnectionStore();
            if (chatUser == null)
            {
                throw new ArgumentNullException("chatUser");
            }
            return userConnectionStore.GetConnectionsByUserAsync(chatUser, cancellationToken);
        }

        /// <summary>
        /// Gets the chat user using the given connection ID.
        /// </summary>
        /// <param name="id">The connection ID.</param>
        /// <param name="cancellationToken">A token to observe while waiting for the task to complete.</param>
        /// <returns>
        /// A task that represents the asynchronous operation.
        /// </returns>
        public virtual Task<ChatConnection> GetConnectionByIdAsync(string id, CancellationToken cancellationToken)
        {
            ThrowIfDisposed();
            var userConnectionStore = GetUserConnectionStore();
            if (string.IsNullOrEmpty(id))
            {
                throw new ArgumentNullException("id");
            }
            return userConnectionStore.GetConnectionByIdAsync(id, cancellationToken);
        }

        /// <summary>
        /// Adds a Connection ID to the given chat user.
        /// </summary>
        /// <param name="connectionId">The Connection ID to add.</param>
        /// <param name="chatUser">The chat user.</param>
        /// <param name="cancellationToken">A token to observe while waiting for the task to complete.</param>
        /// <returns>
        /// A task that represents the asynchronous operation.
        /// </returns>
        public virtual async Task AddConnectionToUserAsync(string connectionId, ChatUserInfo chatUser, CancellationToken cancellationToken)
        {
            ThrowIfDisposed();
            var userConnectionStore = GetUserConnectionStore();
            if (chatUser == null)
            {
                throw new ArgumentNullException("chatUser");
            }
            if (string.IsNullOrEmpty(connectionId))
            {
                throw new ArgumentNullException("connectionId");
            }
            var chatRoom = await FindByIdAsync(chatUser.RoomId, cancellationToken);
            if (chatRoom == null)
            {
                throw new InvalidOperationException("The chat room was not found.");
            }
            await userConnectionStore.AddConnectionToUserAsync(connectionId, chatUser, cancellationToken);
            await UpdateAsync(chatRoom, cancellationToken);
        }

        /// <summary>
        /// Removes a Connection ID from the given chat user.
        /// </summary>
        /// <param name="connectionId">The Connection ID to remove.</param>
        /// <param name="chatUser">The chat user.</param>
        /// <param name="cancellationToken">A token to observe while waiting for the task to complete.</param>
        /// <returns>
        /// A task that represents the asynchronous operation.
        /// </returns>
        public virtual async Task RemoveConnectionFromUserAsync(string connectionId, ChatUserInfo chatUser, CancellationToken cancellationToken)
        {
            ThrowIfDisposed();
            var userConnectionStore = GetUserConnectionStore();
            if (chatUser == null)
            {
                throw new ArgumentNullException("chatUser");
            }
            if (string.IsNullOrEmpty(connectionId))
            {
                throw new ArgumentNullException("connectionId");
            }
            var chatRoom = await FindByIdAsync(chatUser.RoomId, cancellationToken);
            if (chatRoom == null)
            {
                throw new InvalidOperationException("The chat room was not found.");
            }
            await userConnectionStore.RemoveConnectionFromUserAsync(connectionId, chatUser, cancellationToken);
            await UpdateAsync(chatRoom, cancellationToken);
        }

        /// <summary>
        /// Gets a collection of event notifications with the given values.
        /// </summary>
        /// <param name="projectId">The project which owns the events occured.</param>
        /// <param name="clientId">The client who is the actor.</param>
        /// <param name="offset">The index of the page of results to return. Use 1 to indicate the first page.</param>
        /// <param name="limit">The size of the page of results to return. <paramref name="pageIndex" /> is non-zero-based.</param>
        /// <param name="cancellationToken">A token to observe while waiting for the task to complete.</param>
        /// <returns>
        /// A task that represents the asynchronous operation.
        /// </returns>
        public virtual async Task<IList<ChatMessageInfo>> GetMessagesAsync(int projectId, string clientId, int offset, int limit, CancellationToken cancellationToken)
        {
            ThrowIfDisposed();
            var manager = GetLogManager();
            if (string.IsNullOrEmpty(clientId))
            {
                throw new ArgumentNullException("clientId");
            }

            var chatMessages = new List<ChatMessageInfo>();
            var logMessages = await manager.FindAllMessagesAsync(projectId, clientId, offset, limit, cancellationToken);
            foreach (var logMessage in logMessages)
            {
                if (logMessage.CustomUri == null)
                {
                    continue;
                }
                ChatUserInfo from;
                bool admin = false;
                if (logMessage.CustomUri != null)
                {
                    admin = logMessage.CustomUri.EndsWith(">>", StringComparison.Ordinal);
                }
                if (logMessage.Contact != null)
                {
                    from = new ChatUserInfo { ContactId = logMessage.Contact.Id, UserName = logMessage.Contact.Email.Address, NickName = logMessage.Contact.NickName, Admin = admin };
                }
                else
                {
                    from = new ChatUserInfo { UserName = clientId, NickName = clientId, Admin = admin };
                }
                chatMessages.Add(new ChatMessageInfo { From = from, Message = logMessage.Message, CreatedDate = logMessage.StartDate });
            }
            return chatMessages;
        }

        /// <summary>
        /// Inserts a chat message.
        /// </summary>
        /// <param name="chatRoom">The chat room which contains the chat message.</param>
        /// <param name="chatMessage">The chat message to add.</param>
        /// <param name="cancellationToken">A token to observe while waiting for the task to complete.</param>
        /// <returns>
        /// A task that represents the asynchronous operation.
        /// </returns>
        public virtual async Task AddMessageAsync(ChatRoomInfo chatRoom, ChatMessageInfo chatMessage, CancellationToken cancellationToken)
        {
            ThrowIfDisposed();
            var userMessage = GetUserMessageStore();
            var eventManager = GetLogManager();
            if (chatRoom == null)
            {
                throw new ArgumentNullException("chatRoom");
            }
            if (chatMessage == null)
            {
                throw new ArgumentNullException("chatMessage");
            }
            await userMessage.AddMessageAsync(chatRoom, chatMessage, cancellationToken);
            await UpdateAsync(chatRoom, cancellationToken);
            await LogMessageAsync(chatRoom, chatMessage, cancellationToken);
        }

        /// <summary>
        /// Inserts a chat message.
        /// </summary>
        /// <param name="chatRoom">The chat room which contains the chat message.</param>
        /// <param name="chatMessage">The chat message to add.</param>
        /// <param name="cancellationToken">A token to observe while waiting for the task to complete.</param>
        /// <returns>
        /// A task that represents the asynchronous operation.
        /// </returns>
        protected virtual async Task LogMessageAsync(ChatRoomInfo chatRoom, ChatMessageInfo chatMessage, CancellationToken cancellationToken)
        {
            ThrowIfDisposed();
            var logManager = GetLogManager();
            if (chatRoom == null)
            {
                throw new ArgumentNullException("chatRoom");
            }
            if (chatMessage == null)
            {
                throw new ArgumentNullException("chatMessage");
            }
            if (chatMessage.From == null)
            {
                throw new InvalidOperationException("The user 'from' is unspecified.");
            }
            if (chatMessage.To == null)
            {
                throw new InvalidOperationException("The user 'to' is unspecified.");
            }
            if (chatMessage.From.Admin)
            {
                await logManager.LogAsync(new EventItem
                {
                    AnonymId = GuidUtility.NewSequentialGuid(),
                    ObjectType = ObjectType.Message,
                    ObjectId = chatRoom.PageId,
                    Contact = chatMessage.From.ContactId != null ? new AccountItem { Id = (int)chatMessage.From.ContactId } : null,
                    Project = chatRoom.ProjectId != null ? new UniqueItem { Id = (int)chatRoom.ProjectId } : null,
                    ClientId = chatMessage.To.IPAddress,
                    CustomUri = chatMessage.To.NickName + " >>",
                    Message = chatMessage.Message
                }, chatRoom.Users.Values, cancellationToken);
            }
            if (chatMessage.To.Admin)
            {
                await logManager.LogAsync(new EventItem
                {
                    AnonymId = GuidUtility.NewSequentialGuid(),
                    ObjectType = ObjectType.Message,
                    ObjectId = chatRoom.PageId,
                    Contact = chatMessage.From.ContactId != null ? new AccountItem { Id = (int)chatMessage.From.ContactId } : null,
                    Project = chatRoom.ProjectId != null ? new UniqueItem { Id = (int)chatRoom.ProjectId } : null,
                    ClientId = chatMessage.From.IPAddress,
                    CustomUri = "<< " + chatMessage.From.NickName,
                    Message = chatMessage.Message
                }, chatRoom.Users.Values, cancellationToken);
            }
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
        public virtual async Task RemoveMessageAsync(ChatRoomInfo chatRoom, ChatMessageInfo chatMessage, CancellationToken cancellationToken)
        {
            ThrowIfDisposed();
            var userMessage = GetUserMessageStore();
            if (chatRoom == null)
            {
                throw new ArgumentNullException("chatRoom");
            }
            if (chatMessage == null)
            {
                throw new ArgumentNullException("chatMessage");
            }
            await userMessage.RemoveMessageAsync(chatRoom, chatMessage, cancellationToken);
            await UpdateAsync(chatRoom, cancellationToken);
        }

        /// <summary>
        /// Ensures that the Store implements the <see cref="LogManager" /> interface.
        /// </summary>
        /// <returns>
        /// The <see cref="LogManager" /> object.
        /// </returns>
        private LogManager GetLogManager()
        {
            var manager = LogManagerFactory();
            if (manager == null)
            {
                throw new InvalidOperationException("The LogManager factory returns with null.");
            }
            return manager;
        }

        /// <summary>
        /// Ensures that the Store implements the <see cref="ProjectManager" /> interface.
        /// </summary>
        /// <returns>
        /// The <see cref="ProjectManager" /> object.
        /// </returns>
        private ProjectManager GetProjectManager()
        {
            var manager = ProjectManagerFactory();
            if (manager == null)
            {
                throw new InvalidOperationException("The ProjectManager factory returns with null.");
            }
            return manager;
        }

        /// <summary>
        /// Ensures that the Store implements the <see cref="PortalManager" /> interface.
        /// </summary>
        /// <returns>
        /// The <see cref="PortalManager" /> object.
        /// </returns>
        private PortalManager GetPortalManager()
        {
            var manager = PortalManagerFactory();
            if (manager == null)
            {
                throw new InvalidOperationException("The PortalManager factory returns with null.");
            }
            return manager;
        }

        /// <summary>
        /// Ensures that the Store implements the <see cref="IChatUserStore" /> interface.
        /// </summary>
        /// <returns>
        /// The <see cref="IChatUserStore" /> object.
        /// </returns>
        private IChatUserStore GetUserStore()
        {
            var store = Store as IChatUserStore;
            if (store == null)
            {
                throw new InvalidOperationException("The IChatSiteUserStore factory returns with null.");
            }
            return store;
        }

        /// <summary>
        /// Ensures that the Store implements the <see cref="IChatConnectionStore" /> interface.
        /// </summary>
        /// <returns>
        /// The <see cref="IChatConnectionStore" /> object.
        /// </returns>
        private IChatConnectionStore GetUserConnectionStore()
        {
            var store = Store as IChatConnectionStore;
            if (store == null)
            {
                throw new InvalidOperationException("The IChatSiteUserConnectionStore factory returns with null.");
            }
            return store;
        }

        /// <summary>
        /// Ensures that the Store implements the <see cref="IChatMessageStore" /> interface.
        /// </summary>
        /// <returns>
        /// The <see cref="IChatMessageStore" /> object.
        /// </returns>
        private IChatMessageStore GetUserMessageStore()
        {
            var store = Store as IChatMessageStore;
            if (store == null)
            {
                throw new InvalidOperationException("The IChatUserMessageStore factory returns with null.");
            }
            return store;
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
