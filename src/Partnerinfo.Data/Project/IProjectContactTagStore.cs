// Copyright (c) János Janka. All rights reserved.

using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Partnerinfo.Project
{
    public interface IProjectContactTagStore : IProjectContactStore
    {
        /// <summary>
        /// Add business tags to a contact as an asynchronous operation.
        /// </summary>
        /// <param name="contact">The contact to add the business tag to.</param>
        /// <param name="businessTags">A list of <see cref="BusinessTagItem"/>s to add.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/> used to propagate notifications that the operation should be canceled.</param>
        /// <returns>
        /// The task object representing the asynchronous operation.
        /// </returns>
        // Task AddBusinessTagsAsync(ContactItem contact, IEnumerable<BusinessTagItem> businessTags, CancellationToken cancellationToken);

        /// <summary>
        /// Removes the specified <paramref name="businessTags"/> from the given <paramref name="contact"/>.
        /// </summary>
        /// <param name="contact">The contact to remove the specified <paramref name="businessTags"/> from.</param>
        /// <param name="businessTags">A collection of <see cref="BusinessTagItem"/>s to remove.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/> used to propagate notifications that the operation should be canceled.</param>
        /// <returns>
        /// The task object representing the asynchronous operation.
        /// </returns>
        // Task RemoveBusinessTagsAsync(ContactItem contact, IEnumerable<BusinessTagItem> businessTags, CancellationToken cancellationToken);

        /// <summary>
        /// Sets the specified <paramref name="businessTags"/> for the specified <paramref name="contacts"/>.
        /// </summary>
        /// <param name="contacts">The contact to remove the specified <paramref name="businessTags"/> from.</param>
        /// <param name="tagsToAdd">A list of <see cref="BusinessTagItem"/>s to be specified on all the contacts.</param>
        /// <param name="tagsToRemove">A list of <see cref="BusinessTagItem"/>s to be unspecified on all the contacts</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/> used to propagate notifications that the operation should be canceled.</param>
        /// <returns>
        /// The task object representing the asynchronous operation.
        /// </returns>
        Task SetBusinessTagsAsync(IEnumerable<int> contacts, IEnumerable<int> tagsToAdd, IEnumerable<int> tagsToRemove, CancellationToken cancellationToken);

        /// <summary>
        /// Returns a value whether the contact is associated with the given tags or not.
        /// </summary>
        /// <param name="contact">The contact that is associated with the given tags.</param>
        /// <param name="includeWithTags">A list of <see cref="BusinessTagItem"/>s to be specified on all the contacts.</param>
        /// <param name="excludeWithTags">A list of <see cref="BusinessTagItem"/>s to be unspecified on all the contacts</param>
        /// <param name="cancellationToken">A token to observe while waiting for the task to complete.</param>
        /// <returns>
        /// A task that represents the asynchronous operation.
        /// </returns>
        Task<bool> HasBusinessTagsAsync(ContactItem contact, IEnumerable<int> includeWithTags, IEnumerable<int> excludeWithTags, CancellationToken cancellationToken);
    }
}
