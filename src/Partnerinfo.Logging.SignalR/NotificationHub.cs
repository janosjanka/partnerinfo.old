// Copyright (c) János Janka. All rights reserved.

using System.Threading.Tasks;
using Microsoft.AspNet.SignalR;
using Microsoft.AspNet.SignalR.Hubs;

namespace Partnerinfo.Logging
{
    [HubName("notification")]
    public class NotificationHub : Hub
    {
        private readonly EventTicker _ticker;

        /// <summary>
        /// Initializes a new instance of the <see cref="NotificationHub" /> class.
        /// </summary>
        public NotificationHub()
            : this(EventTicker.Instance)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="NotificationHub" /> class.
        /// </summary>
        /// <param name="ticker">The ticker.</param>
        public NotificationHub(EventTicker ticker)
        {
            _ticker = ticker;
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
            _ticker.Start();
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
            _ticker.Start();
        }

        /// <summary>
        /// Called when a connection disconnects from this hub gracefully or due to a timeout.
        /// </summary>
        /// <param name="stopCalled">
        /// <c>true</c>, if stop was called on the client closing the connection gracefully;
        /// <c>false</c>, if the connection has been lost for longer than the Microsoft.AspNet.SignalR.Configuration.IConfigurationManager.DisconnectTimeout.</param>
        /// Timeouts can be caused by clients reconnecting to another SignalR server in scaleout.
        /// <returns>
        /// A <see cref="T:System.Threading.Tasks.Task" />.
        /// </returns>
        public override Task OnDisconnected(bool stopCalled)
        {
            return base.OnDisconnected(stopCalled);
        }
    }
}
