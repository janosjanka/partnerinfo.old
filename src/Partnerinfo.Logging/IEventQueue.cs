// Copyright (c) János Janka. All rights reserved.

namespace Partnerinfo.Logging
{
    public interface IEventQueue
    {
        /// <summary>
        /// Enqueues the given event entry.
        /// </summary>
        /// <param name="eventItem">The event to enqueue.</param>
        void Enqueue(EventResult eventItem);

        /// <summary>
        /// Dequeues a log event entry from the queue.
        /// </summary>
        /// <returns>The event log entry.</returns>
        EventResult Dequeue();
    }
}
