// Copyright (c) János Janka. All rights reserved.

using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Partnerinfo.Project
{
    public interface IProjectContactStore : IProjectStore
    {
        /// <summary>
        /// Gets a list of <see cref="ContactItem" />s to be belonging to the specified <paramref name="project" /> as an asynchronous operation.
        /// </summary>
        /// <param name="project">The project whose associated contacts to retrieve.</param>
        /// <param name="name">The name provided by the <paramref name="name" /> to identify a contact. If this parameter is null, this predicate is ignored.</param>
        /// <param name="includeWithTags">A list of <see cref="BusinessTagItem" /> to be specified on all the contacts. If this parameter is null, this predicate is ignored.</param>
        /// <param name="excludeWithTags">A list of <see cref="BusinessTagItem" /> to be unspecified on all the contacts. If this parameter is null, this predicate is ignored.</param>
        /// <param name="orderBy">The order in which contacts are returned in a result set.</param>
        /// <param name="pageIndex">The index of the page of results to return. Use 1 to indicate the first page.</param>
        /// <param name="pageSize">The size of the page of results to return. <paramref name="pageIndex" /> is non-zero-based.</param>
        /// <param name="fields">The related fields to include in the query results.</param>
        /// <param name="cancellationToken">A token to observe while waiting for the task to complete.</param>
        /// <returns>
        /// The task object representing the asynchronous operation.
        /// </returns>
        Task<ListResult<ContactItem>> GetContactsAsync(ProjectItem project, string name, IEnumerable<int> includeWithTags, IEnumerable<int> excludeWithTags, ContactSortOrder orderBy, int pageIndex, int pageSize, ContactField fields, CancellationToken cancellationToken);

        /// <summary>
        /// Gets a list of <see cref="ContactItem" />s to be belonging to the specified <paramref name="project" /> as an asynchronous operation.
        /// </summary>
        /// <param name="project">The project whose associated contacts to retrieve.</param>
        /// <param name="ids">A list of <see cref="ContactItem" /> keys provided by the <paramref name="ids" /> to identify contacts.</param>
        /// <param name="orderBy">The order in which contacts are returned in a result set.</param>
        /// <param name="fields">The related fields to include in the query results.</param>
        /// <param name="cancellationToken">A token to observe while waiting for the task to complete.</param>
        /// <returns>
        /// A task that represents the asynchronous operation.
        /// </returns>
        Task<IList<ContactItem>> GetContactsAsync(ProjectItem project, IEnumerable<int> ids, ContactSortOrder orderBy, ContactField fields, CancellationToken cancellationToken);

        /// <summary>
        /// Retrieves the contact associated with the key, as an asynchronous operation.
        /// </summary>
        /// <param name="id">The id provided by the <paramref name="id" /> to identify a contact.</param>
        /// <param name="fields">The related fields to include in the query results.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken" /> used to propagate notifications that the operation should be canceled.</param>
        /// <returns>
        /// The <see cref="Task" /> for the asynchronous operation, containing the contact, if any which matched the specified key.
        /// </returns>
        Task<ContactItem> GetContactByIdAsync(int id, ContactField fields, CancellationToken cancellationToken);

        /// <summary>
        /// Retrieves the contact associated with the key, as an asynchronous operation.
        /// </summary>
        /// <param name="project">The project whose associated contacts to retrieve.</param>
        /// <param name="facebookId">The Facebook ID to search for.</param>
        /// <param name="fields">The related fields to include in the query results.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken" /> used to propagate notifications that the operation should be canceled.</param>
        /// <returns>
        /// The <see cref="Task" /> for the asynchronous operation, containing the contact, if any which matched the specified key.
        /// </returns>
        Task<ContactItem> GetContactByFacebookIdAsync(ProjectItem project, long facebookId, ContactField fields, CancellationToken cancellationToken);

        /// <summary>
        /// Retrieves the contact associated with the key, as an asynchronous operation.
        /// </summary>
        /// <param name="project">The project whose associated contacts to retrieve.</param>
        /// <param name="email">The email to search for.</param>
        /// <param name="fields">The related fields to include in the query results.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken" /> used to propagate notifications that the operation should be canceled.</param>
        /// <returns>
        /// The <see cref="Task" /> for the asynchronous operation, containing the contact, if any which matched the specified key.
        /// </returns>
        Task<ContactItem> GetContactByMailAsync(ProjectItem project, string email, ContactField fields, CancellationToken cancellationToken);

        /// <summary>
        /// Add contacts to a project as an asynchronous operation.
        /// </summary>
        /// <param name="project">The project to add the contact to.</param>
        /// <param name="contacts">The collection of <see cref="ContactItem" />s to add.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken" /> used to propagate notifications that the operation should be canceled.</param>
        /// <returns>
        /// The task object representing the asynchronous operation.
        /// </returns>
        Task AddContactsAsync(ProjectItem project, IEnumerable<ContactItem> contacts, CancellationToken cancellationToken);

        /// <summary>
        /// Add a contact to a project as an asynchronous operation.
        /// </summary>
        /// <param name="project">The project to add the contact to.</param>
        /// <param name="contact">The contact of <see cref="ContactItem" /> to add.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken" /> used to propagate notifications that the operation should be canceled.</param>
        /// <returns>
        /// The task object representing the asynchronous operation.
        /// </returns>
        Task<Func<int>> AddContactAsync(ProjectItem project, ContactItem contact, CancellationToken cancellationToken);

        /// <summary>
        /// Replaces the given <paramref name="contact" /> on the specified <paramref name="project" /> with the <paramref name="newContact" />.
        /// </summary>
        /// <param name="project">The project which owns the contact.</param>
        /// <param name="contact">The contact to replace.</param>
        /// <param name="newContact">The new contact to replace the existing <paramref name="contact" /> with.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken" /> used to propagate notifications that the operation should be canceled.</param>
        /// <returns>
        /// The task object representing the asynchronous operation.
        /// </returns>
        Task ReplaceContactAsync(ProjectItem project, ContactItem contact, ContactItem newContact, CancellationToken cancellationToken);

        /// <summary>
        /// Removes the specified <paramref name="contacts" /> from the given <paramref name="project" />.
        /// </summary>
        /// <param name="project">The project to remove the specified <paramref name="contacts" /> from.</param>
        /// <param name="contacts">A collection of <see cref="ContactItem" />s to remove.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken" /> used to propagate notifications that the operation should be canceled.</param>
        /// <returns>
        /// The task object representing the asynchronous operation.
        /// </returns>
        Task RemoveContactsAsync(ProjectItem project, IEnumerable<ContactItem> contacts, CancellationToken cancellationToken);
    }
}
