// Copyright (c) János Janka. All rights reserved.

using System.Threading;
using System.Threading.Tasks;

namespace Partnerinfo.Logging
{
    public interface IEventCategoryStore : IEventStore
    {
        /// <summary>
        /// Add a the specified <paramref name="eventItem" /> to the named category, as an asynchronous operation.
        /// </summary>
        /// <param name="eventItem">The event to add to the named category.</param>
        /// <param name="categoryName">The name of the category to add the event to.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken" /> used to propagate notifications that the operation should be canceled.</param>
        /// <returns>
        /// The <see cref="Task" /> that represents the asynchronous operation.
        /// </returns>
        Task AddToCategoryAsync(EventItem eventItem, string categoryName, CancellationToken cancellationToken);

        /// <summary>
        /// Add a the specified <paramref name="eventItem" /> from the named category, as an asynchronous operation.
        /// </summary>
        /// <param name="eventItem">The event to remove the named category from.</param>
        /// <param name="categoryName">The name of the category to remove.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken" /> used to propagate notifications that the operation should be canceled.</param>
        /// <returns>
        /// The <see cref="Task" /> that represents the asynchronous operation.
        /// </returns>
        Task RemoveFromCategoryAsync(EventItem eventItem, string categoryName, CancellationToken cancellationToken);
    }
}
