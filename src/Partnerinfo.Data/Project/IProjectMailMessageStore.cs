// Copyright (c) János Janka. All rights reserved.

using System;
using System.Threading;
using System.Threading.Tasks;

namespace Partnerinfo.Project
{
    public interface IProjectMailMessageStore : IProjectStore
    {
        /// <summary>
        /// Finds a collection of mail messages with the given values.
        /// </summary>
        /// <param name="project">The project which owns the mail messages.</param>
        /// <param name="subject">The subject to be found.</param>
        /// <param name="orderBy">The order in which contacts are returned in a result set.</param>
        /// <param name="pageIndex">The index of the page of results to return. Use 1 to indicate the first page.</param>
        /// <param name="pageSize">The size of the page of results to return. <paramref name="pageIndex" /> is non-zero-based.</param>
        /// <param name="fields">The related fields to include in the query results.</param>
        /// <param name="cancellationToken">A token to observe while waiting for the task to complete.</param>
        /// <returns>
        /// A task that represents the asynchronous operation.
        /// </returns>
        Task<ListResult<MailMessageItem>> GetMailMessagesAsync(ProjectItem project, string subject, MailMessageSortOrder orderBy, int pageIndex, int pageSize, MailMessageField fields, CancellationToken cancellationToken);

        /// <summary>
        /// Gets the information from the data source for the mail message.
        /// </summary>
        /// <param name="id">The mail message ID to be found.</param>
        /// <param name="fields">The related fields to include in the query results.</param>
        /// <param name="cancellationToken">A token to observe while waiting for the task to complete.</param>
        /// <returns>
        /// A task that represents the asynchronous operation.
        /// </returns>
        Task<MailMessageItem> GetMailMessageByIdAsync(int id, MailMessageField fields, CancellationToken cancellationToken);

        /// <summary>
        /// Adds a new mail message.
        /// </summary>
        /// <param name="project">The project which owns the mail message.</param>
        /// <param name="mailMessage">The mail message to insert.</param>
        /// <param name="cancellationToken">A token to observe while waiting for the task to complete.</param>
        /// <returns>
        /// A task that represents the asynchronous operation.
        /// </returns>
        Task<Func<int>> AddMailMessageAsync(ProjectItem project, MailMessageItem mailMessage, CancellationToken cancellationToken);

        /// <summary>
        /// Updates the given mail message information with the given new mail message information
        /// </summary>
        /// <param name="project">The project which owns the mail message.</param>
        /// <param name="mailMessage">The old mail message.</param>
        /// <param name="newMailMessage">The new mail message.</param>
        /// <param name="cancellationToken">A token to observe while waiting for the task to complete.</param>
        /// <returns>
        /// A task that represents the asynchronous operation.
        /// </returns>
        Task ReplaceMailMessageAsync(ProjectItem project, MailMessageItem mailMessage, MailMessageItem newMailMessage, CancellationToken cancellationToken);

        /// <summary>
        /// Removes a mail message.
        /// </summary>
        /// <param name="project">The project which owns the mail message.</param>
        /// <param name="mailMessage">The mail message to remove.</param>
        /// <param name="cancellationToken">A token to observe while waiting for the task to complete.</param>
        /// <returns>
        /// A task that represents the asynchronous operation.
        /// </returns>
        Task RemoveMailMessageAsync(ProjectItem project, MailMessageItem mailMessage, CancellationToken cancellationToken);
    }
}
