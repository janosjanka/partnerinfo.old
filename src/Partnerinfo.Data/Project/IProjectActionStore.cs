// Copyright (c) János Janka. All rights reserved.

using System;
using System.Threading;
using System.Threading.Tasks;

namespace Partnerinfo.Project
{
    public interface IProjectActionStore : IProjectStore
    {
        /// <summary>
        /// Gets a list of <see cref="ActionResult" />s to be belonging to the specified <paramref name="project" /> as an asynchronous operation.
        /// </summary>
        /// <param name="project">The project whose associated business tags to retrieve.</param>
        /// <param name="name">The name to be found.</param>
        /// <param name="orderBy">The order in which items are returned in a result set.</param>
        /// <param name="pageIndex">The index of the page of results to return. Use 1 to indicate the first page.</param>
        /// <param name="pageSize">The size of the page of results to return. <paramref name="pageIndex" /> is non-zero-based.</param>
        /// <param name="fields">The related fields to include in the query results.</param>
        /// <param name="cancellationToken">A token to observe while waiting for the task to complete.</param>
        /// <returns>
        /// The task object representing the asynchronous operation.
        /// </returns>
        Task<ListResult<ActionItem>> GetActionsAsync(ProjectItem project, string name, ActionSortOrder orderBy, int pageIndex, int pageSize, ActionField fields, CancellationToken cancellationToken);

        /// <summary>
        /// Gets the information from the data source for the action.
        /// </summary>
        /// <param name="id">The action ID to be found.</param>
        /// <param name="cancellationToken">A token to observe while waiting for the task to complete.</param>
        /// <returns>
        /// A task that represents the asynchronous operation.
        /// </returns>
        Task<ActionItem> GetActionByIdAsync(int id, CancellationToken cancellationToken);

        /// <summary>
        /// Gets the information from the data source for the action.
        /// </summary>
        /// <param name="actionLinkId">The unique action identifier from the data source for the action link.</param>
        /// <param name="cancellationToken">A <see cref="System.Threading.CancellationToken" /> to observe while waiting for the task to complete.</param>
        /// <returns>
        /// Returns the action associated with the specified unique identifier.
        /// </returns>
        Task<ActionItem> GetActionByLinkIdAsync(int actionLinkId, CancellationToken cancellationToken);

        /// <summary>
        /// Adds a new action.
        /// </summary>
        /// <param name="project">The project which owns the action.</param>
        /// <param name="parentAction">The parent action. If this parameter is null, the action will be a root action.</param>
        /// <param name="action">The action to insert.</param>
        /// <param name="cancellationToken">A token to observe while waiting for the task to complete.</param>
        /// <returns>
        /// A task that represents the asynchronous operation.
        /// </returns>
        Task<Func<int>> AddActionAsync(ProjectItem project, ActionItem parentAction, ActionItem action, CancellationToken cancellationToken);

        /// <summary>
        /// Updates the given action information with the given new action information.
        /// </summary>
        /// <param name="project">The project which owns the action.</param>
        /// <param name="action">The action.</param>
        /// <param name="newAction">The new action.</param>
        /// <param name="cancellationToken">A token to observe while waiting for the task to complete.</param>
        /// <returns>
        /// A task that represents the asynchronous operation.
        /// </returns>
        Task ReplaceActionAsync(ProjectItem project, ActionItem action, ActionItem newAction, CancellationToken cancellationToken);

        /// <summary>
        /// Copies the specified action.
        /// </summary>
        /// <param name="project">The project which owns the action.</param>
        /// <param name="action">The action.</param>
        /// <param name="cancellationToken">A token to observe while waiting for the task to complete.</param>
        /// <returns>
        /// A task that represents the asynchronous operation.
        /// </returns>
        Task<Func<int>> CopyActionAsync(ProjectItem project, ActionItem action, CancellationToken cancellationToken);

        /// <summary>
        /// Moves the specified action before a reference action as a child of the current action.
        /// </summary>
        /// <param name="project">The project which owns the action.</param>
        /// <param name="action">The action.</param>
        /// <param name="referenceAction">The reference action. If null, <paramref name="action" /> is inserted at the end of the list of child nodes.</param>
        /// <param name="cancellationToken">A token to observe while waiting for the task to complete.</param>
        /// <returns>
        /// A task that represents the asynchronous operation.
        /// </returns>
        Task MoveActionBeforeAsync(ProjectItem project, ActionItem action, ActionItem referenceAction, CancellationToken cancellationToken);

        /// <summary>
        /// Removes a action.
        /// </summary>
        /// <param name="project">The project which owns the action.</param>
        /// <param name="action">The action to remove.</param>
        /// <param name="cancellationToken">A token to observe while waiting for the task to complete.</param>
        /// <returns>
        /// A task that represents the asynchronous operation.
        /// </returns>
        Task RemoveActionAsync(ProjectItem project, ActionItem action, CancellationToken cancellationToken);
    }
}
