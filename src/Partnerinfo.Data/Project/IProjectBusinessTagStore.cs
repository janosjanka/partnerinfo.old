// Copyright (c) János Janka. All rights reserved.

using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Partnerinfo.Project
{
    public interface IProjectBusinessTagStore : IProjectStore
    {
        /// <summary>
        /// Gets a list of <see cref="BusinessTagResult"/>s to be belonging to the specified <paramref name="project"/> as an asynchronous operation.
        /// </summary>
        /// <param name="project">The project whose associated business tags to retrieve.</param>
        /// <param name="cancellationToken">A token to observe while waiting for the task to complete.</param>
        /// <returns>
        /// The task object representing the asynchronous operation.
        /// </returns>
        Task<ListResult<BusinessTagItem>> GetBusinessTagsAsync(ProjectItem project, CancellationToken cancellationToken);

        /// <summary>
        /// Retrieves the business tag associated with the key, as an asynchronous operation.
        /// </summary>
        /// <param name="id">The id provided by the <paramref name="id"/> to identify a business tag.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/> used to propagate notifications that the operation should be canceled.</param>
        /// <returns>
        /// The <see cref="Task"/> for the asynchronous operation, containing the business tag, if any which matched the specified key.
        /// </returns>
        Task<BusinessTagItem> GetBusinessTagByIdAsync(int id, CancellationToken cancellationToken);

        /// <summary>
        /// Retrieves the business tag associated with the name, as an asynchronous operation.
        /// </summary>
        /// <param name="project">The project whose associated business tag to retrieve.</param>
        /// <param name="name">The name provided by the <paramref name="name"/> to identify a business tag.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/> used to propagate notifications that the operation should be canceled.</param>
        /// <returns>
        /// The <see cref="Task"/> for the asynchronous operation, containing the business tag, if any which matched the specified name.
        /// </returns>
        Task<BusinessTagItem> GetBusinessTagByNameAsync(ProjectItem project, string name, CancellationToken cancellationToken);

        /// <summary>
        /// Add business tags to a project as an asynchronous operation.
        /// </summary>
        /// <param name="project">The project to add the business tag to.</param>
        /// <param name="businessTags">The collection of <see cref="BusinessTagItem"/>s to add.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/> used to propagate notifications that the operation should be canceled.</param>
        /// <returns>
        /// The task object representing the asynchronous operation.
        /// </returns>
        Task AddBusinessTagsAsync(ProjectItem project, IEnumerable<BusinessTagItem> businessTags, CancellationToken cancellationToken);

        /// <summary>
        /// Add a business tag to a project as an asynchronous operation.
        /// </summary>
        /// <param name="project">The project to add the business tag to.</param>
        /// <param name="businessTag">The business tag to add.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/> used to propagate notifications that the operation should be canceled.</param>
        /// <returns>
        /// The task object representing the asynchronous operation.
        /// </returns>
        Task<Func<int>> AddBusinessTagAsync(ProjectItem project, BusinessTagItem businessTag, CancellationToken cancellationToken);

        /// <summary>
        /// Replaces the given <paramref name="businessTag"/> on the specified <paramref name="project"/> with the <paramref name="newBusinessTag"/>
        /// </summary>
        /// <param name="project">The project which owns the business tag.</param>
        /// <param name="businessTag">The business tag to replace.</param>
        /// <param name="newBusinessTag">The new business tag to replace the existing <paramref name="businessTag"/> with.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/> used to propagate notifications that the operation should be canceled.</param>
        /// <returns>
        /// The task object representing the asynchronous operation.
        /// </returns>
        Task ReplaceBusinessTagAsync(ProjectItem project, BusinessTagItem businessTag, BusinessTagItem newBusinessTag, CancellationToken cancellationToken);

        /// <summary>
        /// Removes the specified <paramref name="businessTags"/> from the given <paramref name="project"/>.
        /// </summary>
        /// <param name="project">The project to remove the specified <paramref name="businessTags"/> from.</param>
        /// <param name="businessTags">A collection of <see cref="BusinessTagItem"/>s to remove.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/> used to propagate notifications that the operation should be canceled.</param>
        /// <returns>
        /// The task object representing the asynchronous operation.
        /// </returns>
        Task RemoveBusinessTagsAsync(ProjectItem project, IEnumerable<BusinessTagItem> businessTags, CancellationToken cancellationToken);
    }
}
