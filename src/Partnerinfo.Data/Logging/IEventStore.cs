// Copyright (c) János Janka. All rights reserved.

using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Partnerinfo.Logging
{
    public interface IEventStore
    {
        /// <summary>
        /// Inserts an event.
        /// </summary>
        /// <param name="eventItem">The event to insert.</param>
        /// <param name="users">The users.</param>
        /// <param name="cancellationToken">A token to observe while waiting for the task to complete.</param>
        /// <returns>
        /// A task that represents the asynchronous operation.
        /// </returns>
        Task<ValidationResult> CreateAsync(EventItem eventItem, IEnumerable<AccountItem> users, CancellationToken cancellationToken);

        /// <summary>
        /// Updates an event.
        /// </summary>
        /// <param name="eventItem">The event to update.</param>
        /// <param name="cancellationToken">A token to observe while waiting for the task to complete.</param>
        /// <returns>
        /// A task that represents the asynchronous operation.
        /// </returns>
        Task<ValidationResult> UpdateAsync(EventItem eventItem, CancellationToken cancellationToken);

        /// <summary>
        /// Deletes an event.
        /// </summary>
        /// <param name="eventItem">The event to delete.</param>
        /// <param name="cancellationToken">A token to observe while waiting for the task to complete.</param>
        /// <returns>
        /// A task that represents the asynchronous operation.
        /// </returns>
        Task<ValidationResult> DeleteAsync(EventItem eventItem, CancellationToken cancellationToken);

        /// <summary>
        /// Gets the number of unread events.
        /// </summary>
        Task<int> GetUnreadCountAsync(int ownerId, CancellationToken cancellationToken);

        /// <summary>
        /// Sets the event that correlates to the given event.
        /// </summary>
        /// <param name="id">The Event ID (Primary Key).</param>
        /// <param name="cancellationToken">A token to observe while waiting for the task to complete.</param>
        /// <returns>
        /// A task that represents the asynchronous operation.
        /// </returns>
        Task SetCorrelationAsync(int id, CancellationToken cancellationToken);

        /// <summary>
        /// Finishes an event
        /// </summary>
        Task SetFinishDateAsync(int id, DateTime? finishDate, CancellationToken cancellationToken);

        /// <summary>
        /// Marks events as read
        /// </summary>
        Task SetLastReadDateAsync(int userId, DateTime lastReadDate, CancellationToken cancellationToken);

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
        Task<ListResult<EventResult>> FindAllAsync(
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
            CancellationToken cancellationToken);

        /// <summary>
        /// Finds a collection of events with the given primary key value.
        /// </summary>
        /// <param name="id">Event ID (Primary Key).</param>
        /// <param name="cancellationToken">A token to observe while waiting for the task to complete.</param>
        /// <returns>
        /// A task that represents the asynchronous operation.
        /// </returns>
        Task<IList<EventResult>> FindAllByIdAsync(int id, CancellationToken cancellationToken);

        /// <summary>
        /// Gets a collection of event notifications with the given values.
        /// </summary>
        /// <param name="projectId">The project where the events occured.</param>
        /// <param name="clientId">The client who is the actor.</param>
        /// <param name="offset">The index of the page of results to return. Use 1 to indicate the first page.</param>
        /// <param name="limit">The size of the page of results to return. <paramref name="pageIndex" /> is non-zero-based.</param>
        /// <param name="cancellationToken">A token to observe while waiting for the task to complete.</param>
        /// <returns>
        /// A task that represents the asynchronous operation.
        /// </returns>
        Task<IList<MessageResult>> FindAllMessagesAsync(int projectId, string clientId, int offset, int limit, CancellationToken cancellationToken);
    }
}
