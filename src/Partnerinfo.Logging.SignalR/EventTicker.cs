// Copyright (c) János Janka. All rights reserved.

using System;
using System.Threading;
using Microsoft.AspNet.SignalR;
using Microsoft.AspNet.SignalR.Hubs;

namespace Partnerinfo.Logging
{
    public class EventTicker : IDisposable
    {
        private static readonly EventTicker s_instance = new EventTicker(
            GlobalHost.ConnectionManager.GetHubContext<NotificationHub>().Clients,
            GlobalHost.DependencyResolver.Resolve<LogManager>());

        private readonly object _syncRoot = new object();
        private readonly TimeSpan _timeInterval = TimeSpan.FromMilliseconds(2000);
        private Timer _timer;

        /// <summary>
        /// Initializes a new instance of the <see cref="EventTicker" /> class.
        /// </summary>
        /// <param name="clients">The clients.</param>
        /// <param name="logManager">The log manager.</param>
        private EventTicker(IHubConnectionContext<dynamic> clients, LogManager logManager)
        {
            Clients = clients;
            LogManager = logManager;
            MessageLimitPerCycle = 10;
        }

        /// <summary>
        /// Singleton instance
        /// </summary>
        public static EventTicker Instance
        {
            get { return s_instance; }
        }

        /// <summary>
        /// Maximum number of notifications per cycle
        /// </summary>
        public int MessageLimitPerCycle { get; set; }

        /// <summary>
        /// Clients
        /// </summary>
        protected IHubConnectionContext<dynamic> Clients { get; }

        /// <summary>
        /// The log manager that the <see cref="ChatHub" /> operates against.
        /// </summary>
        protected LogManager LogManager { get; }

        /// <summary>
        /// Starts the ticker
        /// </summary>
        public virtual void Start()
        {
            lock (_syncRoot)
            {
                if (_timer == null)
                {
                    _timer = new Timer(o => Broadcast(), null, _timeInterval, _timeInterval);
                }
            }
        }

        /// <summary>
        /// Broadcasts events from the queue
        /// </summary>
        public virtual void Broadcast()
        {
            for (int i = MessageLimitPerCycle; --i >= 0;)
            {
                var eventItem = LogManager.Queue.Dequeue();
                if (eventItem == null)
                {
                    return;
                }

                // Call the eventReceived callback on the client.
                Clients.User(eventItem.User.Email.Address).eventReceived(eventItem);
            }
        }

        /// <summary>
        /// Stops the ticker
        /// </summary>
        public virtual void Stop()
        {
            lock (_syncRoot)
            {
                if (_timer != null)
                {
                    _timer.Dispose();
                    _timer = null;
                }
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
            if (disposing)
            {
                Stop();
            }
        }
    }
}
