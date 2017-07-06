// Copyright (c) János Janka. All rights reserved.

using System.Collections.Concurrent;

namespace Partnerinfo.Logging
{
    public class EventQueue : IEventQueue
    {
        /// <summary>
        /// An instance of the <see cref="EventQueue" /> that is used by the <see cref="LogManager" />.
        /// </summary>
        public static readonly IEventQueue Default = new EventQueue();

        /// <summary>
        /// Event queue
        /// </summary>
        private static readonly ConcurrentQueue<EventResult> s_queue = new ConcurrentQueue<EventResult>();

        /// <summary>
        /// Maximum numbers of events can be kept in memory
        /// </summary>
        public int QueueLimit { get; set; } = 100;

        /// <summary>
        /// Enqueues the given log event entry.
        /// </summary>
        /// <param name="logEvent">The log event to enqueue.</param>
        public virtual void Enqueue(EventResult logEvent)
        {
            s_queue.Enqueue(logEvent);

            if (s_queue.Count > QueueLimit)
            {
                s_queue.TryDequeue(out logEvent);
            }
        }

        /// <summary>
        /// Dequeues a log event entry from the queue.
        /// </summary>
        /// <returns>The event log entry.</returns>
        public virtual EventResult Dequeue()
        {
            EventResult result;
            return s_queue.TryDequeue(out result) ? result : null;
        }
    }
}
