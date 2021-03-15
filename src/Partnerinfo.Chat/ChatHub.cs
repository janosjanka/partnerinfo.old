// Copyright (c) János Janka. All rights reserved.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNet.SignalR;
using Microsoft.AspNet.SignalR.Hubs;

namespace Partnerinfo.Chat
{
    [HubName("chat")]
    public class ChatHub : Hub
    {
        private ChatManager _chatManager;

        /// <summary>
        /// The chat manager that the <see cref="ChatHub" /> operates against.
        /// </summary>
        protected ChatManager ChatManager
        {
            get { return _chatManager ?? (_chatManager = GlobalHost.DependencyResolver.Resolve<ChatManager>()); }
        }

        /// <summary>
        /// Called when the connection connects to this hub instance.
        /// </summary>
        /// <returns>
        /// A <see cref="T:System.Threading.Tasks.Task" />.
        /// </returns>
        public override async Task OnConnected()
        {
            await base.OnConnected();
            await AddCurrentConnectionAsync(
                Context.QueryString["pl"],
                Context.QueryString["pg"],
                Context.QueryString["un"],
                Context.QueryString["an"]);
        }

        /// <summary>
        /// Called when the connection reconnects to this hub instance.
        /// </summary>
        /// <returns>
        /// A <see cref="T:System.Threading.Tasks.Task" />.
        /// </returns>
        public override async Task OnReconnected()
        {
            await base.OnReconnected();
            await AddCurrentConnectionAsync(
                Context.QueryString["pl"],
                Context.QueryString["pg"],
                Context.QueryString["un"],
                Context.QueryString["an"]);
        }

        /// <summary>
        /// Called when a connection disconnects from this hub gracefully or due to a timeout.
        /// </summary>
        /// <param name="stopCalled"><c>true</c>, if stop was called on the client closing the connection gracefully;
        /// <c>false</c>, if the connection has been lost for longer than the <see cref="Microsoft.AspNet.SignalR.Configuration.IConfigurationManager.DisconnectTimeout" />.
        /// Timeouts can be caused by clients reconnecting to another SignalR server in scaleout.</param>
        /// <returns>
        /// A <see cref="T:System.Threading.Tasks.Task" />.
        /// </returns>
        public override async Task OnDisconnected(bool stopCalled)
        {
            await RemoveCurrentConnectionAsync();
            await base.OnDisconnected(stopCalled);
        }

        /// <summary>
        /// Sends a message to the given user.
        /// </summary>
        /// <param name="state">The message to post.</param>
        /// <param name="userName">The recipient.</param>
        /// <returns>
        /// A <see cref="Task" />.
        /// </returns>
        public async Task NotifyStateChanged(int state, string userName = null)
        {
            var room = await GetCallerRoomAsync();
            if (room == null)
            {
                OnCallerError(ChatResources.RoomConnectionRequired);
                return;
            }
            var me = await GetCallerUserAsync(room);
            if (me == null)
            {
                OnCallerError(ChatResources.MessageSenderRequired);
                return;
            }
            var connections = new List<string>(await ChatManager.GetConnectionsByUserAsync(room.Admin, CancellationToken.None));
            if (userName != null)
            {
                var user = await ChatManager.GetUserByUserNameAsync(room, userName, CancellationToken.None);
                if (user != null)
                {
                    connections.AddRange(await ChatManager.GetConnectionsByUserAsync(user, CancellationToken.None));
                }
            }
            Clients.Clients(connections).statechanged(me.UserName, state);
        }

        /// <summary>
        /// Get all messages for the given user name.
        /// </summary>
        /// <param name="userName">The user name.</param>
        /// <returns>
        /// A <see cref="Task" />.
        /// </returns>
        public async Task<IList<ChatMessageInfo>> GetMessages(string userName)
        {
            var room = await GetCallerRoomAsync();
            if (room == null)
            {
                OnCallerError(ChatResources.RoomConnectionRequired);
                return new List<ChatMessageInfo>();
            }
            var me = await GetCallerUserAsync(room);
            if (me == null)
            {
                OnCallerError(ChatResources.MessageSenderRequired);
                return new List<ChatMessageInfo>();
            }
            var other = await ChatManager.GetUserByUserNameAsync(room, userName, CancellationToken.None);
            if (other == null)
            {
                OnCallerError(ChatResources.MessageRecipientRequired);
                return new List<ChatMessageInfo>();
            }
            if (room.ProjectId != null)
            {
                return await ChatManager.GetMessagesAsync((int)room.ProjectId, me.Admin ? other.ClientId : me.ClientId, 0, 100, CancellationToken.None);
            }
            return new List<ChatMessageInfo>();
        }

        /// <summary>
        /// Sends a message to the given user.
        /// </summary>
        /// <param name="userName">The recipient.</param>
        /// <param name="message">The message to post.</param>
        /// <returns>
        /// A <see cref="Task" />.
        /// </returns>
        public async Task SendMessage(string userName, string message)
        {
            var room = await GetCallerRoomAsync();
            if (room == null)
            {
                OnCallerError(ChatResources.RoomConnectionRequired);
                return;
            }
            var me = await GetCallerUserAsync(room);
            if (me == null)
            {
                OnCallerError(ChatResources.MessageSenderRequired);
                return;
            }
            var other = await ChatManager.GetUserByUserNameAsync(room, userName, CancellationToken.None);
            if (other == null)
            {
                OnCallerError(ChatResources.MessageRecipientRequired);
                return;
            }

            // Combine the connection ID lists because a user can connect
            // with other devices, browser tabs, and so on.
            var meConns = await ChatManager.GetConnectionsByUserAsync(me, CancellationToken.None);
            var otherConns = await ChatManager.GetConnectionsByUserAsync(other, CancellationToken.None);
            var connections = meConns.Union(otherConns).ToList();
            var chatMessage = new ChatMessageInfo { From = me, To = other, Message = message };
            await ChatManager.AddMessageAsync(room, chatMessage, CancellationToken.None);
            Clients.Clients(connections).messagereceived(chatMessage);
        }

        /// <summary>
        /// Calls an error handler on the client.
        /// </summary>
        /// <param name="message">The error message.</param>
        private void OnCallerError(string message)
        {
            Clients.Caller.error(message);
        }

        /// <summary>
        /// Joins a site conversation. This method adds the user to a SignalR group and notifies all clients.
        /// </summary>
        /// <param name="pageUri">The page.</param>
        /// <param name="userName">The user name.</param>
        /// <param name="adminNickName">The name of the admin user.</param>
        /// <param name="cancellationToken">A token to observe while waiting for the task to complete.</param>
        /// <returns>
        /// A <see cref="Task" />.
        /// </returns>
        private async Task AddCurrentConnectionAsync(string portalUri, string pageUri, string userName, string adminNickName,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            if (string.IsNullOrEmpty(portalUri) || string.IsNullOrEmpty(pageUri))
            {
                OnCallerError(ChatResources.RoomPageRequired);
                return;
            }

            var room = await GetOrCreateRoomAsync(portalUri, pageUri, adminNickName);
            var user = ChatManager.IsAdmin(room, Context.User.Identity.Name) ? room.Admin :
                await GetOrAddRoomUserAsync(room, userName, GetCallerIP(), cancellationToken);
            await ChatManager.AddConnectionToUserAsync(Context.ConnectionId, user, cancellationToken);

            var others = (await ChatManager.GetUsersAsync(room, cancellationToken))
                .Where(u => !string.Equals(u.UserName, user.UserName, StringComparison.OrdinalIgnoreCase));
            var otherConnections = (await ChatManager.GetConnectionsAsync(room, cancellationToken))
                .Where(c => !string.Equals(c, Context.ConnectionId, StringComparison.Ordinal));

            // We can store the IDs on the client and pass them across HTTP requests
            // without adding a constant room parameter to the 'SendMessage' method below
            Clients.Caller.room = room.Id;
            Clients.Caller.user = user.UserName;
            Clients.Caller.login(user, others);
            Clients.Clients(otherConnections.ToArray()).joined(user);
        }

        /// <summary>
        /// Removes a user with connection ID from the store.
        /// </summary>
        /// <param name="connectionId">The connection ID to remove.</param>
        /// <returns>
        /// A task that represents the asynchronous operation.
        /// </returns>
        private async Task RemoveCurrentConnectionAsync()
        {
            var userConnection = await ChatManager.GetConnectionByIdAsync(Context.ConnectionId, CancellationToken.None);
            if (userConnection == null)
            {
                return;
            }
            var room = await ChatManager.FindByIdAsync(userConnection.RoomId, CancellationToken.None);
            var user = await ChatManager.GetUserByUserNameAsync(room, userConnection.UserName, CancellationToken.None);

            // Remove the connection ID and check whether the user has other connection IDs
            // Note that we should not notify the clients if the user is still connected on a different device, browser tab, etc.
            await ChatManager.RemoveConnectionFromUserAsync(Context.ConnectionId, user, CancellationToken.None);
            var userConnections = await ChatManager.GetConnectionsByUserAsync(user, CancellationToken.None);
            if (userConnections.Count > 0)
            {
                return;
            }

            // At this stage, we can get all other connections
            var otherConnections = await ChatManager.GetConnectionsAsync(room, CancellationToken.None);

            // Then remove the user from the room
            await ChatManager.RemoveUserAsync(room, user, CancellationToken.None);

            // Then remove the room if that is empty
            if (otherConnections.Count == 0)
            {
                await ChatManager.DeleteAsync(room, CancellationToken.None);
            }

            Clients.Clients(userConnections.Union(otherConnections).ToList()).left(user.UserName);
        }

        /// <summary>
        /// Gets or creates a room for the given page.
        /// </summary>
        /// <param name="pageUri">The Page ID (Primary Key).</param>
        /// <param name="adminNickName">The name of the admin user.</param>
        /// <param name="cancellationToken">A token to observe while waiting for the task to complete.</param>
        /// <returns>
        /// The chat room.
        /// </returns>
        private async Task<ChatRoomInfo> GetOrCreateRoomAsync(string portalUri, string pageUri, string adminNickName,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            var room = await ChatManager.CreateByPageUriAsync(portalUri, pageUri, cancellationToken);
            if (room.Admin == null)
            {
                // Add and store a reference to the admin user
                room.Admin = new ChatUserInfo
                {
                    UserName = "portal-" + room.PortalId,
                    NickName = adminNickName,
                    Admin = true
                };
                await ChatManager.AddUserAsync(room, room.Admin, cancellationToken);
            }
            return room;
        }

        /// <summary>
        /// Gets or adds a new user to the given room.
        /// </summary>
        /// <param name="room">The room.</param>
        /// <param name="userName">The user name.</param>
        /// <param name="nickName">The nick name.</param>
        /// <param name="ipAddress">The IP address.</param>
        /// <param name="cancellationToken">A token to observe while waiting for the task to complete.</param>
        /// <returns>
        /// The chat user.
        /// </returns>
        private async Task<ChatUserInfo> GetOrAddRoomUserAsync(ChatRoomInfo room, string userName, string ipAddress,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            int? contactId = null;
            string nickName = null;
            if (!string.IsNullOrEmpty(userName))
            {
                var contact = await ChatManager.GetContactByUserNameAsync(room, userName, cancellationToken);
                if (contact != null)
                {
                    contactId = contact.Id;
                    userName = contact.Email.Address;
                    nickName = contact.NickName ?? contact.Email.Name ?? contact.FirstName ?? contact.LastName;
                }
            }
            if (string.IsNullOrEmpty(userName))
            {
                userName = ChatManager.GenerateClientId(ipAddress);
                nickName = nickName ?? userName;
            }
            var user = await ChatManager.GetUserByUserNameAsync(room, userName, cancellationToken);
            if (user == null)
            {
                user = new ChatUserInfo
                {
                    ContactId = contactId,
                    ClientId = ChatManager.GenerateClientId(ipAddress),
                    IPAddress = ipAddress,
                    UserName = userName,
                    NickName = nickName
                };
                await ChatManager.AddUserAsync(room, user, cancellationToken);
            }
            return user;
        }

        /// <summary>
        /// Gets the chat room for the caller.
        /// </summary>
        /// <param name="cancellationToken">A token to observe while waiting for the task to complete.</param>
        /// <returns>
        /// The chat room.
        /// </returns>
        private Task<ChatRoomInfo> GetCallerRoomAsync(
            CancellationToken cancellationToken = default(CancellationToken))
        {
            string room = Clients.Caller.room;
            if (room != null)
            {
                return ChatManager.FindByIdAsync(room, cancellationToken);
            }
            return null;
        }

        /// <summary>
        /// Gets the chat user for the caller.
        /// </summary>
        /// <param name="chatRoom">The chat room.</param>
        /// <param name="cancellationToken">A token to observe while waiting for the task to complete.</param>
        /// <returns>
        /// The chat user.
        /// </returns>
        private async Task<ChatUserInfo> GetCallerUserAsync(ChatRoomInfo chatRoom = null,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            if (chatRoom == null)
            {
                chatRoom = await GetCallerRoomAsync(cancellationToken);
            }
            if (chatRoom == null)
            {
                throw new ArgumentNullException("chatRoom");
            }
            string user = Clients.Caller.user;
            if (user != null)
            {
                return await ChatManager.GetUserByUserNameAsync(chatRoom, user, cancellationToken);
            }
            return null;
        }

        /// <summary>
        /// Gets the IP Address from the request object.
        /// </summary>
        /// <returns>
        /// The IP Address.
        /// </returns>
        private string GetCallerIP()
        {
            object ip;
            return Context.Request.Environment.TryGetValue("server.RemoteIpAddress", out ip) ? (string)ip : null;
        }
    }
}
