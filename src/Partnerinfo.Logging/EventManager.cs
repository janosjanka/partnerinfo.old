// Copyright (c) János Janka. All rights reserved.

using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Partnerinfo.Security;

namespace Partnerinfo.Logging
{
    public class EventManager : IDisposable
    {
        private bool _disposed;

        /// <summary>
        /// Initializes a new instance of the <see cref="EventManager"/> class.
        /// </summary>
        /// <param name="securityManager">The security manager that that <see cref="EventManager" /> operates against.</param>
        /// <param name="store">The store that the <see cref="EventManager" /> operates against.</param>
        public EventManager(SecurityManager securityManager, IEventStore store)
        {
            if (securityManager == null)
            {
                throw new ArgumentNullException(nameof(securityManager));
            }
            if (store == null)
            {
                throw new ArgumentNullException(nameof(store));
            }
            SecurityManager = securityManager;
            Store = store;
        }

        /// <summary>
        /// Gets or sets the security manager that that <see cref="EventManager" /> operates against.
        /// </summary>
        /// <value>
        /// The security manager.
        /// </value>
        protected SecurityManager SecurityManager { get; set; }

        /// <summary>
        /// The store that the <see cref="EventManager" /> operates against
        /// </summary>
        protected IEventStore Store { get; set; }

        /// <summary>
        /// Client ID Provider
        /// </summary>
        public IClientIdProvider ClientIdProvider { get; set; } = Logging.ClientIdProvider.Default;

        /// <summary>
        /// URL Provider
        /// </summary>
        public IUrlProvider UrlProvider { get; set; } = Logging.UrlProvider.Default;

        /// <summary>
        /// Event report generator
        /// </summary>
        public IEventReport ReportGenerator { get; set; }

        /// <summary>
        /// Inserts an event.
        /// </summary>
        /// <param name="eventItem">The event to insert.</param>
        /// <param name="cancellationToken">A token to observe while waiting for the task to complete.</param>
        /// <returns>
        /// A task that represents the asynchronous operation.
        /// </returns>
        public virtual async Task<ValidationResult> CreateAsync(EventItem eventItem, IEnumerable<AccountItem> users, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            ThrowIfDisposed();
            if (eventItem == null)
            {
                throw new ArgumentNullException(nameof(eventItem));
            }
            if (users == null)
            {
                throw new ArgumentNullException(nameof(users));
            }

            eventItem.ClientId = ClientIdProvider.Generate(eventItem.ClientId);
            eventItem.ReferrerUrl = UrlProvider.Generate(eventItem.ReferrerUrl);
            return await Store.CreateAsync(eventItem, users, cancellationToken);
        }

        /// <summary>
        /// Gets the number of unread events.
        /// </summary>
        public virtual Task<int> GetUnreadCountAsync(int ownerId, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            ThrowIfDisposed();
            return Store.GetUnreadCountAsync(ownerId, cancellationToken);
        }

        /// <summary>
        /// Sets the event that correlates to the given event.
        /// </summary>
        /// <param name="id">The Event ID (Primary Key).</param>
        /// <param name="cancellationToken">A token to observe while waiting for the task to complete.</param>
        /// <returns>
        /// A task that represents the asynchronous operation.
        /// </returns>
        public virtual Task SetCorrelationAsync(int id, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            ThrowIfDisposed();
            return Store.SetCorrelationAsync(id, cancellationToken);
        }

        /// <summary>
        /// Finishes an event
        /// </summary>
        public virtual Task SetEventFinishDateAsync(int id, DateTime? finishDate, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            ThrowIfDisposed();
            return Store.SetFinishDateAsync(id, finishDate, cancellationToken);
        }

        /// <summary>
        /// Marks events as read
        /// </summary>
        public virtual Task SetLastReadDateAsync(int userId, DateTime lastReadDate, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            ThrowIfDisposed();
            return Store.SetLastReadDateAsync(userId, lastReadDate, cancellationToken);
        }

        /// <summary>
        /// Finds a collection of events with the given values
        /// </summary>
        /// <param name="ownerId">Owner ID (Foreign Key)</param>
        /// <param name="categoryId">Category ID (Foreign Key)</param>
        /// <param name="projectId">Project ID (Foreign Key)</param>
        /// <param name="actionId">Action ID (Foreign Key)</param>
        /// <param name="pageId">Page ID (Foreign Key)</param>
        /// <param name="contactState">Contact state flag</param>
        /// <param name="dateFrom"></param>
        /// <param name="dateTo"></param>
        /// <param name="emails">A collection of contact emails.</param>
        /// <param name="clients">A collection of Client IDs.</param>
        /// <param name="pageIndex">The index of the page of results to return. Use 1 to indicate the first page.</param>
        /// <param name="pageSize">The size of the page of results to return. <paramref name="pageIndex" /> is non-zero-based.</param>
        /// <param name="cancellationToken">A token to observe while waiting for the task to complete.</param>
        /// <returns>
        /// A task that represents the asynchronous operation.
        /// </returns>
        public virtual Task<ListResult<EventResult>> FindAllAsync(
            int userId,
            int? categoryId,
            DateTime? dateFrom,
            DateTime? dateTo,
            ObjectType? objectType,
            int? objectId,
            int? contactId,
            ObjectState? contactState,
            int? projectId,
            string customUri,
            IEnumerable<string> emails,
            IEnumerable<string> clients,
            int pageIndex,
            int pageSize,
            CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            ThrowIfDisposed();
            return Store.FindAllAsync(
                userId,
                categoryId,
                dateFrom,
                dateTo,
                objectType,
                objectId,
                contactId,
                contactState,
                projectId,
                customUri,
                emails,
                clients,
                pageIndex,
                pageSize,
                cancellationToken);
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
            ThrowIfDisposed();
            return Store.FindAllMessagesAsync(projectId, clientId, offset, limit, cancellationToken);
        }

        /// <summary>
        /// Finds a collection of events with the given primary key value.
        /// </summary>
        /// <param name="id">Event ID (Primary Key).</param>
        /// <param name="cancellationToken">A token to observe while waiting for the task to complete.</param>
        /// <returns>
        /// A task that represents the asynchronous operation.
        /// </returns>
        public virtual Task<IList<EventResult>> FindAllByIdAsync(int id, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            ThrowIfDisposed();
            return Store.FindAllByIdAsync(id, cancellationToken);
        }

        //
        // Batch Operations
        //

        /// <summary>
        /// Gets a group of primary keys of events with the given values.
        /// </summary>
        /// <param name="ownerId">Owner ID (Foreign Key)</param>
        /// <param name="categoryId">Category ID (Foreign Key)</param>
        /// <param name="projectId">Project ID (Foreign Key)</param>
        /// <param name="actionId">Action ID (Foreign Key)</param>
        /// <param name="pageId">Page ID (Foreign Key)</param>
        /// <param name="contactState">Contact state flag</param>
        /// <param name="dateFrom"></param>
        /// <param name="dateTo"></param>
        /// <param name="emails">A collection of contact emails.</param>
        /// <param name="clients">A collection of Client IDs.</param>
        /// <param name="cancellationToken">A token to observe while waiting for the task to complete.</param>
        /// <returns>
        /// A task that represents the asynchronous operation.
        /// </returns>
        public virtual Task<IList<int>> GetIdsAsync(
            int userId,
            int? categoryId,
            DateTime? dateFrom,
            DateTime? dateTo,
            ObjectType? objectType,
            int? objectId,
            int? contactId,
            ObjectState? contactState,
            int? projectId,
            string customUri,
            IEnumerable<string> emails,
            IEnumerable<string> clients,
            CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            ThrowIfDisposed();
            return GetBulkOperationsStore().GetIdsAsync(
                userId,
                categoryId,
                dateFrom,
                dateTo,
                objectType,
                objectId,
                contactId,
                contactState,
                projectId,
                customUri,
                emails,
                clients,
                cancellationToken);
        }

        /// <summary>
        /// Sets the given category for a group of events.
        /// </summary>
        /// <param name="userId">The User ID (Foreign Key).</param>
        /// <param name="eventIds">A collection of primary keys of events.</param>
        /// <param name="categoryId">The Category ID (Foreign Key).</param>
        /// <param name="cancellationToken">A token to observe while waiting for the task to complete.</param>
        /// <returns>
        /// A task that represents the asynchronous operation.
        /// </returns>
        public virtual Task BulkSetCategoriesAsync(int userId, IEnumerable<int> eventIds, int? categoryId, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            ThrowIfDisposed();
            var batchStore = GetBulkOperationsStore();
            if (eventIds == null)
            {
                throw new ArgumentNullException(nameof(eventIds));
            }
            return batchStore.BulkSetCategoriesAsync(userId, eventIds, categoryId, cancellationToken);
        }

        /// <summary>
        /// Sets the given contact for a group of events.
        /// </summary>
        /// <param name="sessionId">The Session ID (Foreign Key).</param>
        /// <param name="contactId">The Contact ID (Foreign Key).</param>
        /// <param name="cancellationToken">A token to observe while waiting for the task to complete.</param>
        /// <returns>
        /// A task that represents the asynchronous operation.
        /// </returns>
        public virtual Task BulkSetContactsAsync(Guid sessionId, int contactId, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            ThrowIfDisposed();
            var batchStore = GetBulkOperationsStore();
            if (sessionId == null)
            {
                throw new ArgumentNullException("sessionId");
            }
            return batchStore.BulkSetContactsAsync(sessionId, contactId, cancellationToken);
        }

        /// <summary>
        /// Deletes a group of events.
        /// </summary>
        /// <param name="userId">The User ID (Foreign Key).</param>
        /// <param name="eventIds">A collection of primary keys of events.</param>
        /// <param name="cancellationToken">A token to observe while waiting for the task to complete.</param>
        /// <returns>
        /// A task that represents the asynchronous operation.
        /// </returns>
        public virtual Task BulkDeleteAsync(int userId, IEnumerable<int> eventIds, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            ThrowIfDisposed();
            var batchStore = GetBulkOperationsStore();
            if (eventIds == null)
            {
                throw new ArgumentNullException("eventIds");
            }
            return batchStore.BulkDeleteAsync(userId, eventIds, cancellationToken);
        }

        //
        // Private Members
        //

        /// <summary>
        /// Validates project and update. Called by other ProjectManager methods.
        /// </summary>
        private async Task<ValidationResult> UpdateEventAsync(EventItem eventItem, CancellationToken cancellationToken)
        {
            return await Store.UpdateAsync(eventItem, cancellationToken);
        }

        private IEventBulkOperations GetBulkOperationsStore()
        {
            var store = Store as IEventBulkOperations;
            if (store == null)
            {
                throw new NotSupportedException($"Store is not an {nameof(IEventBulkOperations)}.");
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
                // Store.Dispose();
            }
            _disposed = true;
            Store = null;
        }
    }
}
