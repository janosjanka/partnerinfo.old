// Copyright (c) János Janka. All rights reserved.

using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Partnerinfo.Logging
{
    public interface IEventBulkOperations : IEventStore
    {
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
        Task<IList<int>> GetIdsAsync(
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
            CancellationToken cancellationToken);

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
        Task BulkSetCategoriesAsync(int userId, IEnumerable<int> eventIds, int? categoryId, CancellationToken cancellationToken);

        /// <summary>
        /// Sets the given contact for all the events.
        /// </summary>
        /// <param name="anonymId">The Anonymous User ID to identify the event(s).</param>
        /// <param name="contactId">The Contact ID (Foreign Key) to associate with the event(s).</param>
        /// <param name="cancellationToken">A token to observe while waiting for the task to complete.</param>
        /// <returns>
        /// A task that represents the asynchronous operation.
        /// </returns>
        Task BulkSetContactsAsync(Guid anonymId, int contactId, CancellationToken cancellationToken);

        /// <summary>
        /// Deletes a group of events.
        /// </summary>
        /// <param name="userId">The User ID (Foreign Key).</param>
        /// <param name="eventIds">A collection of primary keys of events.</param>
        /// <param name="cancellationToken">A token to observe while waiting for the task to complete.</param>
        /// <returns>
        /// A task that represents the asynchronous operation.
        /// </returns>
        Task BulkDeleteAsync(int userId, IEnumerable<int> eventIds, CancellationToken cancellationToken);
    }
}
