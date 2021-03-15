// Copyright (c) János Janka. All rights reserved.

using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Partnerinfo.Logging.Rules;

namespace Partnerinfo.Logging
{
    public class LogManager : IDisposable
    {
        private bool _disposed;

        /// <summary>
        /// Initializes a new instance of the <see cref="LogManager"/> class.
        /// </summary>
        public LogManager(EventManager eventManager, RuleManager ruleManager)
        {
            if (eventManager == null)
            {
                throw new ArgumentNullException(nameof(eventManager));
            }
            if (ruleManager == null)
            {
                throw new ArgumentNullException(nameof(ruleManager));
            }
            EventManager = eventManager;
            RuleManager = ruleManager;
        }

        protected EventManager EventManager { get; set; }

        protected RuleManager RuleManager { get; set; }

        /// <summary>
        /// Gets or sets the rule invoker that the <see cref="LogManager" /> operates against.
        /// </summary>
        /// <value>
        /// The rule invoker.
        /// </value>
        public RuleInvoker RuleInvoker { get; set; }

        /// <summary>
        /// Gets or sets the queue that the <see cref="EventManager" /> operates against.
        /// </summary>
        /// <value>
        /// The queue.
        /// </value>
        public IEventQueue Queue { get; set; } = EventQueue.Default;

        /// <summary>
        /// Inserts an event.
        /// </summary>
        /// <param name="eventItem">The event to insert.</param>
        /// <param name="cancellationToken">A token to observe while waiting for the task to complete.</param>
        /// <returns>
        /// A task that represents the asynchronous operation.
        /// </returns>
        public virtual async Task<ValidationResult> LogAsync(EventItem eventItem, IEnumerable<AccountItem> users, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            if (eventItem == null)
            {
                throw new ArgumentNullException(nameof(eventItem));
            }
            if (users == null)
            {
                throw new ArgumentNullException(nameof(users));
            }

            var validationResult = await EventManager.CreateAsync(eventItem, users, cancellationToken);
            if (validationResult.Succeeded)
            {
                // TODO: Post process operations can be optimized out to a background job
                if (eventItem.AnonymId != null && eventItem.Contact != null)
                {
                    await EventManager.BulkSetContactsAsync((Guid)eventItem.AnonymId, eventItem.Contact.Id, cancellationToken);
                }

                await EventManager.SetCorrelationAsync(eventItem.Id, cancellationToken);

                // Add the event to the queue that can be even read by a SignalR Hub
                // Of course, it is worth supporting some eviction technics avoiding memory wasting
                var eventResults = await EventManager.FindAllByIdAsync(eventItem.Id, cancellationToken);
                foreach (var eventResult in eventResults)
                {
                    await ApplyRules(eventResult, cancellationToken);
                    Queue.Enqueue(eventResult);
                }
            }
            return validationResult;
        }

        /// <summary>
        /// Applies user-defined rules to the specified event item.
        /// </summary>
        /// <param name="eventResult">The event result.</param>
        /// <param name="cancellationToken">A token to observe while waiting for the task to complete.</param>
        /// <returns>
        /// A task that represents the asynchronous operation.
        /// </returns>
        public virtual async Task ApplyRules(EventResult eventResult, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            ThrowIfDisposed();
            if (eventResult == null)
            {
                throw new ArgumentNullException(nameof(eventResult));
            }
            if (eventResult.User == null)
            {
                throw new InvalidOperationException("Rules cannot be applied without an owner.");
            }
            var rules = await RuleManager.FindAllAsync(eventResult.User.Id, RuleField.None, cancellationToken);
            await RuleInvoker.InvokeAsync(rules.Data, eventResult, cancellationToken);
        }

        /// <summary>
        /// Gets a collection of event notifications with the given values.
        /// </summary>
        /// <param name="projectId">The site where the events occured.</param>
        /// <param name="clientId">The client who is the actor.</param>
        /// <param name="offset">The index of the page of results to return. Use 1 to indicate the first page.</param>
        /// <param name="limit">The size of the page of results to return. <paramref name="pageIndex" /> is non-zero-based.</param>
        /// <param name="cancellationToken">A token to observe while waiting for the task to complete.</param>
        /// <returns>
        /// A task that represents the asynchronous operation.
        /// </returns>
        public virtual Task<IList<MessageResult>> FindAllMessagesAsync(int projectId, string clientId, int offset, int limit, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            return EventManager.FindAllMessagesAsync(projectId, clientId, offset, limit, cancellationToken);
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
            _disposed = true;
        }
    }
}
