// Copyright (c) János Janka. All rights reserved.

using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Partnerinfo.Security;

namespace Partnerinfo.Project
{
    public class ProjectManager : IDisposable
    {
        private bool _disposed;

        /// <summary>
        /// Initializes a new instance of the <see cref="ProjectManager" /> class
        /// </summary>
        /// <param name="securityManager">The security manager that that <see cref="ProjectManager" /> operates against.</param>
        /// <param name="store">The store that the <see cref="ProjectManager" /> operates against.</param>
        /// <param name="cache">The cache that the <see cref="ProjectManager" /> operates against.</param>
        /// <param name="validator">A list of <see cref="IProjectValidator" />.</param>
        /// <exception cref="System.ArgumentNullException">
        /// </exception>
        public ProjectManager(
            SecurityManager securityManager,
            IProjectStore store,
            IProjectCache cache,
            IProjectValidator validator)
        {
            if (securityManager == null)
            {
                throw new ArgumentNullException(nameof(securityManager));
            }
            if (store == null)
            {
                throw new ArgumentNullException(nameof(store));
            }
            if (cache == null)
            {
                throw new ArgumentNullException(nameof(cache));
            }
            SecurityManager = securityManager;
            Store = store;
            Cache = cache;
            Validator = validator;
        }

        /// <summary>
        /// Gets or sets the security manager that that <see cref="ProjectManager" /> operates against.
        /// </summary>
        /// <value>
        /// The security manager.
        /// </value>
        protected SecurityManager SecurityManager { get; set; }

        /// <summary>
        /// Gets or sets the store that the <see cref="ProjectManager" /> operates against.
        /// </summary>
        /// <value>
        /// The store.
        /// </value>
        protected IProjectStore Store { get; set; }

        /// <summary>
        /// Gets or sets the cache that the <see cref="ProjectManager" /> operates against.
        /// </summary>
        /// <value>
        /// The cache.
        /// </value>
        protected IProjectCache Cache { get; set; }

        /// <summary>
        /// Gets a list of <see cref="IProjectValidator" />s.
        /// </summary>
        /// <value>
        /// A list of <see cref="IProjectValidator" />s.
        /// </value>
        internal IProjectValidator Validator { get; }

        /// <summary>
        /// Finds a collection of projects with the given values.
        /// </summary>
        /// <param name="userId">The user identifier for the item to be found.</param>
        /// <param name="name">The name for the item to be found.</param>
        /// <param name="orderBy">The order in which items are returned in a result set.</param>
        /// <param name="pageIndex">The index of the page of results to return. Use 1 to indicate the first page.</param>
        /// <param name="pageSize">The size of the page of results to return. <paramref name="pageIndex" /> is non-zero-based.</param>
        /// <param name="fields">The related fields to include in the query results.</param>
        /// <param name="cancellationToken">A token to observe while waiting for the task to complete.</param>
        /// <returns>
        /// A task that represents the asynchronous operation.
        /// </returns>
        /// <exception cref="System.ObjectDisposedException"></exception>
        public virtual Task<ListResult<ProjectItem>> FindAllAsync(int userId, string name, ProjectSortOrder orderBy, int pageIndex, int pageSize, ProjectField fields, CancellationToken cancellationToken)
        {
            ThrowIfDisposed();
            return Store.FindAllAsync(userId, name, orderBy, pageIndex, pageSize, fields, cancellationToken);
        }

        /// <summary>
        /// Finds a project with the given primary key value
        /// </summary>
        /// <param name="id">The primary key for the item to be found</param>
        /// <param name="cancellationToken">A token to observe while waiting for the task to complete</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// </returns>
        /// <exception cref="System.ObjectDisposedException"></exception>
        public virtual async Task<ProjectItem> FindByIdAsync(int id, CancellationToken cancellationToken)
        {
            ThrowIfDisposed();
            var project = Cache.GetById(id);
            if (project == null)
            {
                // Load and materialize the project. It is much more expensive than using a L2 cache.
                project = await Store.FindByIdAsync(id, cancellationToken);
                if (project != null)
                {
                    Cache.Put(project);
                }
            }
            return project;
        }

        /// <summary>
        /// Inserts a project
        /// </summary>
        /// <param name="project">The project to insert</param>
        /// <param name="owners">The owners.</param>
        /// <param name="cancellationToken">A token to observe while waiting for the task to complete</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// </returns>
        /// <exception cref="System.ArgumentNullException">
        /// </exception>
        /// <exception cref="System.ObjectDisposedException"></exception>
        public virtual async Task<ValidationResult> CreateAsync(ProjectItem project, IEnumerable<AccountItem> owners, CancellationToken cancellationToken)
        {
            ThrowIfDisposed();
            if (project == null)
            {
                throw new ArgumentNullException(nameof(project));
            }
            if (owners == null)
            {
                throw new ArgumentNullException(nameof(owners));
            }
            var validationResult = await ValidateProjectAsync(project, cancellationToken);
            if (!validationResult.Succeeded)
            {
                return validationResult;
            }
            validationResult = await Store.CreateAsync(project, cancellationToken);
            if (!validationResult.Succeeded)
            {
                return validationResult;
            }
            foreach (var owner in owners)
            {
                await SecurityManager.SetAccessRuleAsync(project, new AccessRuleItem { User = owner, Permission = AccessPermission.IsOwner }, cancellationToken);
            }
            return ValidationResult.Success;
        }

        /// <summary>
        /// Updates a project
        /// </summary>
        /// <param name="project">The project to update</param>
        /// <param name="cancellationToken">A token to observe while waiting for the task to complete</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// </returns>
        /// <exception cref="System.ArgumentNullException"></exception>
        /// <exception cref="System.ObjectDisposedException"></exception>
        public virtual async Task<ValidationResult> UpdateAsync(ProjectItem project, CancellationToken cancellationToken)
        {
            ThrowIfDisposed();
            if (project == null)
            {
                throw new ArgumentNullException(nameof(project));
            }
            var validationResult = await ValidateProjectAsync(project, cancellationToken);
            if (!validationResult.Succeeded)
            {
                return validationResult;
            }
            validationResult = await Store.UpdateAsync(project, cancellationToken);
            InvalidateProjectCache(project);
            return validationResult;
        }

        /// <summary>
        /// Deletes a project
        /// </summary>
        /// <param name="project">The project to delete</param>
        /// <param name="cancellationToken">A token to observe while waiting for the task to complete</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// </returns>
        /// <exception cref="System.ArgumentNullException"></exception>
        /// <exception cref="System.ObjectDisposedException"></exception>
        public virtual async Task<ValidationResult> DeleteAsync(ProjectItem project, CancellationToken cancellationToken)
        {
            ThrowIfDisposed();
            if (project == null)
            {
                throw new ArgumentNullException(nameof(project));
            }
            var validationResult = await Store.DeleteAsync(project, cancellationToken);
            if (validationResult.Succeeded)
            {
                await SecurityManager.RemoveAccessRulesAsync(project, cancellationToken);
            }
            InvalidateProjectCache(project);
            return validationResult;
        }

        /// <summary>
        /// Finds a collection of actions with the given values.
        /// </summary>
        /// <param name="projectId">The project which owns the actions.</param>
        /// <param name="name">The name to be found.</param>
        /// <param name="orderBy">The order in which items are returned in a result set.</param>
        /// <param name="pageIndex">The index of the page of results to return. Use 1 to indicate the first page.</param>
        /// <param name="pageSize">The size of the page of results to return. <paramref name="pageIndex" /> is non-zero-based.</param>
        /// <param name="fields">The related fields to include in the query results.</param>
        /// <param name="cancellationToken">A token to observe while waiting for the task to complete.</param>
        /// <returns>
        /// A task that represents the asynchronous operation.
        /// </returns>
        /// <exception cref="System.InvalidOperationException">The project does not exist: {0}</exception>
        /// <exception cref="System.ObjectDisposedException"></exception>
        public virtual async Task<ListResult<ActionItem>> GetActionsAsync(
            int projectId, string name, ActionSortOrder orderBy, int pageIndex, int pageSize, ActionField fields, CancellationToken cancellationToken)
        {
            ThrowIfDisposed();
            var actionStore = GetActionStore();
            var project = await FindByIdAsync(projectId, cancellationToken);
            if (project == null)
            {
                throw new InvalidOperationException("The project does not exist: {0}");
            }
            return await actionStore.GetActionsAsync(project, name, orderBy, pageIndex, pageSize, fields, cancellationToken);
        }

        /// <summary>
        /// Gets the information from the data source for the action.
        /// </summary>
        /// <param name="id">The action ID to be found.</param>
        /// <param name="cancellationToken">A token to observe while waiting for the task to complete.</param>
        /// <returns>
        /// A task that represents the asynchronous operation.
        /// </returns>
        public virtual Task<ActionItem> GetActionByIdAsync(int id, CancellationToken cancellationToken)
        {
            ThrowIfDisposed();
            return GetActionStore().GetActionByIdAsync(id, cancellationToken);
        }

        /// <summary>
        /// Gets the information from the data source for the action.
        /// </summary>
        /// <param name="actionLinkId">The unique action identifier from the data source for the action.</param>
        /// <param name="cancellationToken">A <see cref="System.Threading.CancellationToken" /> to observe while waiting for the task to complete.</param>
        /// <returns>
        /// Returns the action associated with the specified unique identifier.
        /// </returns>
        public virtual Task<ActionItem> GetActionByLinkIdAsync(int actionLinkId, CancellationToken cancellationToken)
        {
            ThrowIfDisposed();
            return GetActionStore().GetActionByLinkIdAsync(actionLinkId, cancellationToken);
        }

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
        /// <exception cref="System.ArgumentNullException">
        /// project
        /// or
        /// </exception>
        public virtual async Task<ValidationResult> AddActionAsync(ProjectItem project, ActionItem parentAction, ActionItem action, CancellationToken cancellationToken)
        {
            ThrowIfDisposed();
            var actionStore = GetActionStore();
            if (project == null)
            {
                throw new ArgumentNullException(nameof(project));
            }
            if (action == null)
            {
                throw new ArgumentNullException(nameof(action));
            }
            var idAccessor = await actionStore.AddActionAsync(project, parentAction, action, cancellationToken);
            var validationResult = await UpdateProjectAsync(project, cancellationToken);
            if (validationResult.Succeeded)
            {
                action.Id = idAccessor();
            }
            return validationResult;
        }

        /// <summary>
        /// Updates the given action information with the given new action information
        /// </summary>
        /// <param name="project">The project which owns the action.</param>
        /// <param name="action">The action.</param>
        /// <param name="newAction">The new action.</param>
        /// <param name="cancellationToken">A token to observe while waiting for the task to complete.</param>
        /// <returns>
        /// A task that represents the asynchronous operation.
        /// </returns>
        /// <exception cref="System.ArgumentNullException">
        /// </exception>
        public virtual async Task<ValidationResult> ReplaceActionAsync(ProjectItem project, ActionItem action, ActionItem newAction, CancellationToken cancellationToken)
        {
            ThrowIfDisposed();
            var actionStore = GetActionStore();
            if (project == null)
            {
                throw new ArgumentNullException(nameof(project));
            }
            if (action == null)
            {
                throw new ArgumentNullException(nameof(action));
            }
            if (newAction == null)
            {
                throw new ArgumentNullException(nameof(newAction));
            }
            await actionStore.ReplaceActionAsync(project, action, newAction, cancellationToken);
            return await UpdateProjectAsync(project, cancellationToken);
        }

        /// <summary>
        /// Copies an action.
        /// </summary>
        /// <param name="project">The project which owns the action.</param>
        /// <param name="action">The action to copy.</param>
        /// <param name="referenceAction">The reference action. If null, <paramref name="action" /> is inserted at the end of the list of child nodes.</param>
        /// <param name="cancellationToken">A token to observe while waiting for the task to complete.</param>
        /// <returns>
        /// A task that represents the asynchronous operation.
        /// </returns>
        /// <exception cref="System.ArgumentNullException">
        /// </exception>
        public virtual async Task<ValidationResult> CopyActionBeforeAsync(ProjectItem project, ActionItem action, ActionItem referenceAction, CancellationToken cancellationToken)
        {
            ThrowIfDisposed();
            var actionStore = GetActionStore();
            if (project == null)
            {
                throw new ArgumentNullException(nameof(project));
            }
            if (action == null)
            {
                throw new ArgumentNullException(nameof(action));
            }
            var idAccessor = await actionStore.CopyActionAsync(project, action, cancellationToken);
            var validationResult = await UpdateProjectAsync(project, cancellationToken);
            if (validationResult.Succeeded)
            {
                action.Id = idAccessor();
                return await MoveActionBeforeAsync(project, action, referenceAction, cancellationToken);
            }
            return validationResult;
        }

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
        /// <exception cref="System.ArgumentNullException">
        /// </exception>
        public virtual async Task<ValidationResult> MoveActionBeforeAsync(ProjectItem project, ActionItem action, ActionItem referenceAction, CancellationToken cancellationToken)
        {
            ThrowIfDisposed();
            var actionStore = GetActionStore();
            if (project == null)
            {
                throw new ArgumentNullException(nameof(project));
            }
            if (action == null)
            {
                throw new ArgumentNullException(nameof(action));
            }
            await actionStore.MoveActionBeforeAsync(project, action, referenceAction, cancellationToken);
            return await UpdateProjectAsync(project, cancellationToken);
        }

        /// <summary>
        /// Removes a action.
        /// </summary>
        /// <param name="project">The project which owns the action.</param>
        /// <param name="action">The action to remove.</param>
        /// <param name="cancellationToken">A token to observe while waiting for the task to complete.</param>
        /// <returns>
        /// A task that represents the asynchronous operation.
        /// </returns>
        /// <exception cref="System.ArgumentNullException">
        /// </exception>
        public virtual async Task<ValidationResult> RemoveActionAsync(ProjectItem project, ActionItem action, CancellationToken cancellationToken)
        {
            ThrowIfDisposed();
            var actionStore = GetActionStore();
            if (project == null)
            {
                throw new ArgumentNullException(nameof(project));
            }
            if (action == null)
            {
                throw new ArgumentNullException(nameof(action));
            }
            await actionStore.RemoveActionAsync(project, action, cancellationToken);
            return await UpdateProjectAsync(project, cancellationToken);
        }

        /// <summary>
        /// Finds a collection of business tags with the given values.
        /// </summary>
        /// <param name="project">The project which owns the actions.</param>
        /// <param name="cancellationToken">A token to observe while waiting for the task to complete.</param>
        /// <returns>
        /// The task object representing the asynchronous operation.
        /// </returns>
        /// <exception cref="System.ArgumentNullException"></exception>
        public virtual Task<ListResult<BusinessTagItem>> GetBusinessTagsAsync(ProjectItem project, CancellationToken cancellationToken)
        {
            ThrowIfDisposed();
            var businessTagStore = GetBusinessTagStore();
            if (project == null)
            {
                throw new ArgumentNullException(nameof(project));
            }
            return businessTagStore.GetBusinessTagsAsync(project, cancellationToken);
        }

        /// <summary>
        /// Retrieves the business tag associated with the key, as an asynchronous operation.
        /// </summary>
        /// <param name="id">The id provided by the <paramref name="id" /> to identify a business tag.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken" /> used to propagate notifications that the operation should be canceled.</param>
        /// <returns>
        /// The <see cref="Task" /> for the asynchronous operation, containing the business tag, if any which matched the specified key.
        /// </returns>
        public virtual Task<BusinessTagItem> GetBusinessTagByIdAsync(int id, CancellationToken cancellationToken)
        {
            ThrowIfDisposed();
            return GetBusinessTagStore().GetBusinessTagByIdAsync(id, cancellationToken);
        }

        /// <summary>
        /// Retrieves the business tag associated with the name, as an asynchronous operation.
        /// </summary>
        /// <param name="project">The project whose associated business tag to retrieve.</param>
        /// <param name="name">The name provided by the <paramref name="name" /> to identify a business tag.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken" /> used to propagate notifications that the operation should be canceled.</param>
        /// <returns>
        /// The <see cref="Task" /> for the asynchronous operation, containing the business tag, if any which matched the specified name.
        /// </returns>
        /// <exception cref="System.ArgumentNullException">project</exception>
        public virtual Task<BusinessTagItem> GetBusinessTagByNameAsync(ProjectItem project, string name, CancellationToken cancellationToken)
        {
            ThrowIfDisposed();
            var businessTagStore = GetBusinessTagStore();
            if (project == null)
            {
                throw new ArgumentNullException(nameof(project));
            }
            return businessTagStore.GetBusinessTagByNameAsync(project, name, cancellationToken);
        }

        /// <summary>
        /// Adds the specified <paramref name="project" /> to the <paramref name="project" />, as an asynchronous operation.
        /// </summary>
        /// <param name="project">The project to add the business tag to.</param>
        /// <param name="businessTag">The business tag to add.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken" /> used to propagate notifications that the operation should be canceled.</param>
        /// <returns>
        /// The <see cref="Task" /> that represents the asynchronous operation, containing the <see cref="ValidationResult" /> of the operation.
        /// </returns>
        /// <exception cref="System.ArgumentNullException">project</exception>
        public virtual async Task<ValidationResult> AddBusinessTagAsync(ProjectItem project, BusinessTagItem businessTag, CancellationToken cancellationToken)
        {
            ThrowIfDisposed();
            var businessTagStore = GetBusinessTagStore();
            if (project == null)
            {
                throw new ArgumentNullException(nameof(project));
            }
            var idAccessor = await businessTagStore.AddBusinessTagAsync(project, businessTag, cancellationToken);
            var validationResult = await UpdateProjectAsync(project, cancellationToken);
            businessTag.Id = idAccessor();
            return validationResult;
        }

        /// <summary>
        /// Add business tags to a project as an asynchronous operation.
        /// </summary>
        /// <param name="project">The project to add the business tag to.</param>
        /// <param name="businessTags">The collection of <see cref="BusinessTagItem" />s to add.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken" /> used to propagate notifications that the operation should be canceled.</param>
        /// <returns>
        /// The task object representing the asynchronous operation.
        /// </returns>
        /// <exception cref="System.ArgumentNullException">project</exception>
        public virtual async Task<ValidationResult> AddBusinessTagsAsync(ProjectItem project, IEnumerable<BusinessTagItem> businessTags, CancellationToken cancellationToken)
        {
            ThrowIfDisposed();
            var businessTagStore = GetBusinessTagStore();
            if (project == null)
            {
                throw new ArgumentNullException(nameof(project));
            }
            await businessTagStore.AddBusinessTagsAsync(project, businessTags, cancellationToken);
            return await UpdateProjectAsync(project, cancellationToken);
        }

        /// <summary>
        /// Replaces the given <paramref name="businessTag" /> on the specified <paramref name="project" /> with the <paramref name="newBusinessTag" />
        /// </summary>
        /// <param name="project">The project which owns the business tag.</param>
        /// <param name="businessTag">The business tag to replace.</param>
        /// <param name="newBusinessTag">The new business tag to replace the existing <paramref name="businessTag" /> with.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken" /> used to propagate notifications that the operation should be canceled.</param>
        /// <returns>
        /// The task object representing the asynchronous operation.
        /// </returns>
        /// <exception cref="System.ArgumentNullException">project</exception>
        public virtual async Task<ValidationResult> ReplaceBusinessTagAsync(ProjectItem project, BusinessTagItem businessTag, BusinessTagItem newBusinessTag, CancellationToken cancellationToken)
        {
            ThrowIfDisposed();
            var businessTagStore = GetBusinessTagStore();
            if (project == null)
            {
                throw new ArgumentNullException(nameof(project));
            }
            await businessTagStore.ReplaceBusinessTagAsync(project, businessTag, newBusinessTag, cancellationToken);
            return await UpdateProjectAsync(project, cancellationToken);
        }

        /// <summary>
        /// Removes the specified <paramref name="businessTag" /> from the given <paramref name="project" />.
        /// </summary>
        /// <param name="project">The project to remove the specified <paramref name="businessTag" /> from.</param>
        /// <param name="businessTag">The <see cref="BusinessTagItem" /> to remove.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken" /> used to propagate notifications that the operation should be canceled.</param>
        /// <returns>
        /// The <see cref="Task" /> that represents the asynchronous operation, containing the <see cref="ValidationResult" /> of the operation.
        /// </returns>
        public virtual Task<ValidationResult> RemoveBusinessTagAsync(ProjectItem project, BusinessTagItem businessTag, CancellationToken cancellationToken)
        {
            return RemoveBusinessTagsAsync(project, new[] { businessTag }, cancellationToken);
        }

        /// <summary>
        /// Removes the specified <paramref name="businessTags" /> from the given <paramref name="project" />.
        /// </summary>
        /// <param name="project">The project to remove the specified <paramref name="businessTags" /> from.</param>
        /// <param name="businessTags">A collection of <see cref="BusinessTagItem" />s to remove.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken" /> used to propagate notifications that the operation should be canceled.</param>
        /// <returns>
        /// The task object representing the asynchronous operation.
        /// </returns>
        /// <exception cref="System.ArgumentNullException">project</exception>
        public virtual async Task<ValidationResult> RemoveBusinessTagsAsync(ProjectItem project, IEnumerable<BusinessTagItem> businessTags, CancellationToken cancellationToken)
        {
            ThrowIfDisposed();
            var businessTagStore = GetBusinessTagStore();
            if (project == null)
            {
                throw new ArgumentNullException(nameof(project));
            }
            await businessTagStore.RemoveBusinessTagsAsync(project, businessTags, cancellationToken);
            return await UpdateProjectAsync(project, cancellationToken);
        }

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
        /// <exception cref="System.ArgumentNullException">
        /// project
        /// or
        /// or
        /// </exception>
        public virtual Task<ListResult<ContactItem>> GetContactsAsync(ProjectItem project, string name, IEnumerable<int> includeWithTags, IEnumerable<int> excludeWithTags, ContactSortOrder orderBy, int pageIndex, int pageSize, ContactField fields, CancellationToken cancellationToken)
        {
            ThrowIfDisposed();
            var contactStore = GetContactStore();
            if (project == null)
            {
                throw new ArgumentNullException(nameof(project));
            }
            if (includeWithTags == null)
            {
                throw new ArgumentNullException(nameof(includeWithTags));
            }
            if (excludeWithTags == null)
            {
                throw new ArgumentNullException(nameof(excludeWithTags));
            }
            return contactStore.GetContactsAsync(project, name, includeWithTags, excludeWithTags, orderBy, pageIndex, pageSize, fields, cancellationToken);
        }

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
        /// <exception cref="System.ArgumentNullException">
        /// project
        /// or
        /// </exception>
        public virtual Task<IList<ContactItem>> GetContactsAsync(ProjectItem project, IEnumerable<int> ids, ContactSortOrder orderBy, ContactField fields, CancellationToken cancellationToken)
        {
            ThrowIfDisposed();
            var contactStore = GetContactStore();
            if (project == null)
            {
                throw new ArgumentNullException(nameof(project));
            }
            if (ids == null)
            {
                throw new ArgumentNullException(nameof(ids));
            }
            return contactStore.GetContactsAsync(project, ids, orderBy, fields, cancellationToken);
        }

        /// <summary>
        /// Retrieves the contact associated with the key, as an asynchronous operation.
        /// </summary>
        /// <param name="id">The id provided by the <paramref name="id" /> to identify a contact.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken" /> used to propagate notifications that the operation should be canceled.</param>
        /// <returns>
        /// The <see cref="Task" /> for the asynchronous operation, containing the contact, if any which matched the specified key.
        /// </returns>
        public virtual Task<ContactItem> GetContactByIdAsync(int id, CancellationToken cancellationToken)
        {
            ThrowIfDisposed();
            return GetContactStore().GetContactByIdAsync(id, ContactField.None, cancellationToken);
        }

        /// <summary>
        /// Retrieves the contact associated with the key, as an asynchronous operation.
        /// </summary>
        /// <param name="id">The id provided by the <paramref name="id" /> to identify a contact.</param>
        /// <param name="fields">The related fields to include in the query results.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken" /> used to propagate notifications that the operation should be canceled.</param>
        /// <returns>
        /// The <see cref="Task" /> for the asynchronous operation, containing the contact, if any which matched the specified key.
        /// </returns>
        public virtual Task<ContactItem> GetContactByIdAsync(int id, ContactField fields, CancellationToken cancellationToken)
        {
            ThrowIfDisposed();
            return GetContactStore().GetContactByIdAsync(id, fields, cancellationToken);
        }

        /// <summary>
        /// Retrieves the contact associated with the key, as an asynchronous operation.
        /// </summary>
        /// <param name="project">The project whose associated contacts to retrieve.</param>
        /// <param name="facebookId">The Facebook ID to search for.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken" /> used to propagate notifications that the operation should be canceled.</param>
        /// <returns>
        /// The <see cref="Task" /> for the asynchronous operation, containing the contact, if any which matched the specified key.
        /// </returns>
        /// <exception cref="System.ArgumentNullException"></exception>
        public virtual Task<ContactItem> GetContactByFacebookIdAsync(ProjectItem project, long facebookId, CancellationToken cancellationToken)
        {
            ThrowIfDisposed();
            var contactStore = GetContactStore();
            if (project == null)
            {
                throw new ArgumentNullException(nameof(project));
            }
            return contactStore.GetContactByFacebookIdAsync(project, facebookId, ContactField.None, cancellationToken);
        }

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
        /// <exception cref="System.ArgumentNullException"></exception>
        public virtual Task<ContactItem> GetContactByFacebookIdAsync(ProjectItem project, long facebookId, ContactField fields, CancellationToken cancellationToken)
        {
            ThrowIfDisposed();
            var contactStore = GetContactStore();
            if (project == null)
            {
                throw new ArgumentNullException(nameof(project));
            }
            return contactStore.GetContactByFacebookIdAsync(project, facebookId, fields, cancellationToken);
        }

        /// <summary>
        /// Retrieves the contact associated with the key, as an asynchronous operation.
        /// </summary>
        /// <param name="project">The project whose associated contacts to retrieve.</param>
        /// <param name="email">The email to search for.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken" /> used to propagate notifications that the operation should be canceled.</param>
        /// <returns>
        /// The <see cref="Task" /> for the asynchronous operation, containing the contact, if any which matched the specified key.
        /// </returns>
        /// <exception cref="System.ArgumentNullException">project</exception>
        public virtual Task<ContactItem> GetContactByMailAsync(ProjectItem project, string email, CancellationToken cancellationToken)
        {
            ThrowIfDisposed();
            var contactStore = GetContactStore();
            if (project == null)
            {
                throw new ArgumentNullException(nameof(project));
            }
            return contactStore.GetContactByMailAsync(project, email, ContactField.None, cancellationToken);
        }

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
        /// <exception cref="System.ArgumentNullException">project</exception>
        public virtual Task<ContactItem> GetContactByMailAsync(ProjectItem project, string email, ContactField fields, CancellationToken cancellationToken)
        {
            ThrowIfDisposed();
            var contactStore = GetContactStore();
            if (project == null)
            {
                throw new ArgumentNullException(nameof(project));
            }
            return contactStore.GetContactByMailAsync(project, email, fields, cancellationToken);
        }

        /// <summary>
        /// Add a contact to a project as an asynchronous operation.
        /// </summary>
        /// <param name="project">The project to add the contact to.</param>
        /// <param name="contact">The contact of <see cref="ContactItem" /> to add.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken" /> used to propagate notifications that the operation should be canceled.</param>
        /// <returns>
        /// The task object representing the asynchronous operation.
        /// </returns>
        /// <exception cref="System.ArgumentNullException">
        /// project
        /// or
        /// </exception>
        public virtual async Task<ValidationResult> AddContactAsync(ProjectItem project, ContactItem contact, CancellationToken cancellationToken)
        {
            ThrowIfDisposed();
            var contactStore = GetContactStore();
            if (project == null)
            {
                throw new ArgumentNullException(nameof(project));
            }
            if (contact == null)
            {
                throw new ArgumentNullException(nameof(contact));
            }
            var idAccessor = await contactStore.AddContactAsync(project, contact, cancellationToken);
            var validationResult = await UpdateProjectAsync(project, cancellationToken);
            contact.Id = idAccessor();
            return validationResult;
        }

        /// <summary>
        /// Add contacts to a project as an asynchronous operation.
        /// </summary>
        /// <param name="project">The project to add the contact to.</param>
        /// <param name="contacts">The collection of <see cref="ContactItem" />s to add.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken" /> used to propagate notifications that the operation should be canceled.</param>
        /// <returns>
        /// The task object representing the asynchronous operation.
        /// </returns>
        /// <exception cref="System.ArgumentNullException">
        /// project
        /// or
        /// contacts
        /// </exception>
        public virtual async Task<ValidationResult> AddContactsAsync(ProjectItem project, IEnumerable<ContactItem> contacts, CancellationToken cancellationToken)
        {
            ThrowIfDisposed();
            var contactStore = GetContactStore();
            if (project == null)
            {
                throw new ArgumentNullException(nameof(project));
            }
            if (contacts == null)
            {
                throw new ArgumentNullException(nameof(contacts));
            }
            await contactStore.AddContactsAsync(project, contacts, cancellationToken);
            return await UpdateProjectAsync(project, cancellationToken);
        }

        /// <summary>
        /// Replaces the given <paramref name="contact" /> on the specified <paramref name="project" /> with the <paramref name="newContact" />
        /// </summary>
        /// <param name="project">The project which owns the contact.</param>
        /// <param name="contact">The contact to replace.</param>
        /// <param name="newContact">The new contact to replace the existing <paramref name="contact" /> with.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken" /> used to propagate notifications that the operation should be canceled.</param>
        /// <returns>
        /// The task object representing the asynchronous operation.
        /// </returns>
        /// <exception cref="System.ArgumentNullException">
        /// project
        /// or
        /// or
        /// </exception>
        public virtual async Task<ValidationResult> ReplaceContactAsync(ProjectItem project, ContactItem contact, ContactItem newContact, CancellationToken cancellationToken)
        {
            ThrowIfDisposed();
            var contactStore = GetContactStore();
            if (project == null)
            {
                throw new ArgumentNullException(nameof(project));
            }
            if (contact == null)
            {
                throw new ArgumentNullException(nameof(contact));
            }
            if (newContact == null)
            {
                throw new ArgumentNullException(nameof(newContact));
            }
            await contactStore.ReplaceContactAsync(project, contact, newContact, cancellationToken);
            return await UpdateProjectAsync(project, cancellationToken);
        }

        /// <summary>
        /// Removes the specified <paramref name="contact" /> from the given <paramref name="project" />.
        /// </summary>
        /// <param name="project">The project to remove the specified <paramref name="contact" /> from.</param>
        /// <param name="contact">The contact of <see cref="ContactItem" /> to remove.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken" /> used to propagate notifications that the operation should be canceled.</param>
        /// <returns>
        /// The task object representing the asynchronous operation.
        /// </returns>
        /// <exception cref="System.ArgumentNullException">project
        /// or
        /// contacts</exception>
        public virtual Task<ValidationResult> RemoveContactAsync(ProjectItem project, ContactItem contact, CancellationToken cancellationToken)
        {
            ThrowIfDisposed();
            if (project == null)
            {
                throw new ArgumentNullException(nameof(project));
            }
            if (contact == null)
            {
                throw new ArgumentNullException(nameof(contact));
            }
            return RemoveContactsAsync(project, new[] { contact }, cancellationToken);
        }

        /// <summary>
        /// Removes the specified <paramref name="contacts" /> from the given <paramref name="project" />.
        /// </summary>
        /// <param name="project">The project to remove the specified <paramref name="contact" /> from.</param>
        /// <param name="contacts">The contacts.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken" /> used to propagate notifications that the operation should be canceled.</param>
        /// <returns>
        /// The task object representing the asynchronous operation.
        /// </returns>
        /// <exception cref="System.ArgumentNullException">project
        /// or
        /// contacts</exception>
        public virtual async Task<ValidationResult> RemoveContactsAsync(ProjectItem project, IEnumerable<ContactItem> contacts, CancellationToken cancellationToken)
        {
            ThrowIfDisposed();
            var contactStore = GetContactStore();
            if (project == null)
            {
                throw new ArgumentNullException(nameof(project));
            }
            if (contacts == null)
            {
                throw new ArgumentNullException(nameof(contacts));
            }
            await contactStore.RemoveContactsAsync(project, contacts, cancellationToken);
            return await UpdateProjectAsync(project, cancellationToken);
        }

        /// <summary>
        /// Sets the specified <paramref name="businessTags" /> for the specified <paramref name="contacts" />.
        /// </summary>
        /// <param name="contacts">The contact to remove the specified <paramref name="businessTags" /> from.</param>
        /// <param name="tagsToAdd">A list of <see cref="BusinessTagItem" />s to be specified on all the contacts.</param>
        /// <param name="tagsToRemove">A list of <see cref="BusinessTagItem" />s to be unspecified on all the contacts</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken" /> used to propagate notifications that the operation should be canceled.</param>
        /// <returns>
        /// The task object representing the asynchronous operation.
        /// </returns>
        /// <exception cref="System.ArgumentNullException">contacts
        /// or
        /// tagsToAdd
        /// or
        /// tagsToRemove</exception>
        public virtual async Task<ValidationResult> SetBusinessTagsAsync(IEnumerable<int> contacts, IEnumerable<int> tagsToAdd, IEnumerable<int> tagsToRemove, CancellationToken cancellationToken)
        {
            ThrowIfDisposed();
            var contactStore = GetContactTagStore();
            if (contacts == null)
            {
                throw new ArgumentNullException("contacts");
            }
            if (tagsToAdd == null)
            {
                throw new ArgumentNullException(nameof(tagsToAdd));
            }
            if (tagsToRemove == null)
            {
                throw new ArgumentNullException(nameof(tagsToRemove));
            }
            await contactStore.SetBusinessTagsAsync(contacts, tagsToAdd, tagsToRemove, cancellationToken);
            return ValidationResult.Success;
        }

        /// <summary>
        /// Returns a value whether the contact is associated with the given tags or not.
        /// </summary>
        /// <param name="contact">The contact that is associated with the given tags.</param>
        /// <param name="includeWithTags">A list of <see cref="BusinessTagItem" /> to be specified on all the contacts.</param>
        /// <param name="excludeWithTags">A list of <see cref="BusinessTagItem" /> to be unspecified on all the contacts</param>
        /// <param name="cancellationToken">A token to observe while waiting for the task to complete.</param>
        /// <returns>
        /// A task that represents the asynchronous operation.
        /// </returns>
        /// <exception cref="System.ArgumentNullException">
        /// contact
        /// or
        /// includeWithTags
        /// or
        /// excludeWithTags
        /// </exception>
        public virtual Task<bool> HasBusinessTagsAsync(ContactItem contact, IEnumerable<int> includeWithTags, IEnumerable<int> excludeWithTags, CancellationToken cancellationToken)
        {
            ThrowIfDisposed();
            var contactStore = GetContactTagStore();
            if (contact == null)
            {
                throw new ArgumentNullException("contact");
            }
            if (includeWithTags == null)
            {
                throw new ArgumentNullException(nameof(includeWithTags));
            }
            if (excludeWithTags == null)
            {
                throw new ArgumentNullException(nameof(excludeWithTags));
            }
            return contactStore.HasBusinessTagsAsync(contact, includeWithTags, excludeWithTags, cancellationToken);
        }

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
        public virtual Task<ListResult<MailMessageItem>> GetMailMessagesAsync(ProjectItem project, string subject, MailMessageSortOrder orderBy, int pageIndex, int pageSize, MailMessageField fields, CancellationToken cancellationToken)
        {
            ThrowIfDisposed();
            return GetMailMessageStore().GetMailMessagesAsync(project, subject, orderBy, pageIndex, pageSize, fields, cancellationToken);
        }

        /// <summary>
        /// Gets the information from the data source for the mail message.
        /// </summary>
        /// <param name="id">The mail message ID to be found.</param>
        /// <param name="fields">The related fields to include in the query results.</param>
        /// <param name="cancellationToken">A token to observe while waiting for the task to complete.</param>
        /// <returns>
        /// A task that represents the asynchronous operation.
        /// </returns>
        public virtual Task<MailMessageItem> GetMailMessageByIdAsync(int id, MailMessageField fields, CancellationToken cancellationToken)
        {
            ThrowIfDisposed();
            return GetMailMessageStore().GetMailMessageByIdAsync(id, fields, cancellationToken);
        }

        /// <summary>
        /// Adds a new mail message.
        /// </summary>
        /// <param name="project">The project which owns the mail message.</param>
        /// <param name="mailMessage">The mail message to insert.</param>
        /// <param name="cancellationToken">A token to observe while waiting for the task to complete.</param>
        /// <returns>
        /// A task that represents the asynchronous operation.
        /// </returns>
        /// <exception cref="System.ArgumentNullException">
        /// project
        /// or
        /// </exception>
        public virtual async Task<ValidationResult> AddMailMessageAsync(ProjectItem project, MailMessageItem mailMessage, CancellationToken cancellationToken)
        {
            ThrowIfDisposed();
            var mailMessageStore = GetMailMessageStore();
            if (project == null)
            {
                throw new ArgumentNullException(nameof(project));
            }
            if (mailMessage == null)
            {
                throw new ArgumentNullException(nameof(mailMessage));
            }
            var idAccessor = await mailMessageStore.AddMailMessageAsync(project, mailMessage, cancellationToken);
            var validationResult = await UpdateProjectAsync(project, cancellationToken);
            if (validationResult.Succeeded)
            {
                mailMessage.Id = idAccessor();
            }
            return validationResult;
        }

        /// <summary>
        /// Updates the given mail message information with the given new mail message information
        /// </summary>
        /// <param name="project">The project.</param>
        /// <param name="oldMailMessage">The old mail message.</param>
        /// <param name="newMailMessage">The new mail message.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns></returns>
        /// <exception cref="System.ArgumentNullException">
        /// project
        /// or
        /// or
        /// </exception>
        public async Task<ValidationResult> ReplaceMailMessageAsync(ProjectItem project, MailMessageItem oldMailMessage, MailMessageItem newMailMessage, CancellationToken cancellationToken)
        {
            ThrowIfDisposed();
            var mailMessageStore = GetMailMessageStore();
            if (project == null)
            {
                throw new ArgumentNullException(nameof(project));
            }
            if (oldMailMessage == null)
            {
                throw new ArgumentNullException(nameof(oldMailMessage));
            }
            if (newMailMessage == null)
            {
                throw new ArgumentNullException(nameof(newMailMessage));
            }
            await mailMessageStore.ReplaceMailMessageAsync(project, oldMailMessage, newMailMessage, cancellationToken);
            return await UpdateProjectAsync(project, cancellationToken);
        }

        /// <summary>
        /// Removes a mail message.
        /// </summary>
        /// <param name="project">The project which owns the mail message.</param>
        /// <param name="mailMessage">The mail message to remove.</param>
        /// <param name="cancellationToken">A token to observe while waiting for the task to complete.</param>
        /// <returns>
        /// A task that represents the asynchronous operation.
        /// </returns>
        /// <exception cref="System.ArgumentNullException">
        /// project
        /// or
        /// </exception>
        public async Task<ValidationResult> RemoveMailMessageAsync(ProjectItem project, MailMessageItem mailMessage, CancellationToken cancellationToken)
        {
            ThrowIfDisposed();
            var mailMessageStore = GetMailMessageStore();
            if (project == null)
            {
                throw new ArgumentNullException(nameof(project));
            }
            if (mailMessage == null)
            {
                throw new ArgumentNullException(nameof(mailMessage));
            }
            await mailMessageStore.RemoveMailMessageAsync(project, mailMessage, cancellationToken);
            return await UpdateProjectAsync(project, cancellationToken);
        }

        /// <summary>
        /// Invalidates the cache.
        /// </summary>
        /// <param name="project">The project to remove from the cache.</param>
        private void InvalidateProjectCache(ProjectItem project)
        {
            var cacheItem = Cache.GetById(project.Id);
            if (cacheItem != null)
            {
                Cache.Remove(cacheItem);
            }
        }

        /// <summary>
        /// Validates project. Called by other ProjectManager methods.
        /// </summary>
        private Task<ValidationResult> ValidateProjectAsync(ProjectItem project, CancellationToken cancellationToken)
        {
            return (Validator == null) ? Task.FromResult(ValidationResult.Success) : Validator.ValidateAsync(this, project, cancellationToken);
        }

        /// <summary>
        /// Validates project and update. Called by other ProjectManager methods.
        /// </summary>
        private async Task<ValidationResult> UpdateProjectAsync(ProjectItem project, CancellationToken cancellationToken)
        {
            var result = await ValidateProjectAsync(project, cancellationToken);
            if (!result.Succeeded)
            {
                return result;
            }
            return await Store.UpdateAsync(project, cancellationToken);
        }

        /// <summary>
        /// Ensures that the Store implements the <see cref="IProjectActionStore" /> interface.
        /// </summary>
        /// <returns>
        /// The <see cref="IProjectActionStore" /> object.
        /// </returns>
        private IProjectActionStore GetActionStore()
        {
            var store = Store as IProjectActionStore;
            if (store == null)
            {
                throw new NotSupportedException();
            }
            return store;
        }

        /// <summary>
        /// Ensures that the Store implements the <see cref="IProjectBusinessTagStore" /> interface.
        /// </summary>
        /// <returns>
        /// The <see cref="IProjectBusinessTagStore" /> object.
        /// </returns>
        private IProjectBusinessTagStore GetBusinessTagStore()
        {
            var store = Store as IProjectBusinessTagStore;
            if (store == null)
            {
                throw new NotSupportedException();
            }
            return store;
        }

        /// <summary>
        /// Ensures that the Store implements the <see cref="IProjectContactStore" /> interface.
        /// </summary>
        /// <returns>
        /// The <see cref="IProjectContactStore" /> object.
        /// </returns>
        private IProjectContactStore GetContactStore()
        {
            var store = Store as IProjectContactStore;
            if (store == null)
            {
                throw new NotSupportedException();
            }
            return store;
        }

        /// <summary>
        /// Ensures that the Store implements the <see cref="IProjectContactTagStore" /> interface.
        /// </summary>
        /// <returns>
        /// The <see cref="IProjectContactTagStore" /> object.
        /// </returns>
        private IProjectContactTagStore GetContactTagStore()
        {
            var store = Store as IProjectContactTagStore;
            if (store == null)
            {
                throw new NotSupportedException();
            }
            return store;
        }

        /// <summary>
        /// Ensures that the Store implements the <see cref="IProjectMailMessageStore" /> interface.
        /// </summary>
        /// <returns>
        /// The <see cref="IProjectMailMessageStore" /> object.
        /// </returns>
        private IProjectMailMessageStore GetMailMessageStore()
        {
            var store = Store as IProjectMailMessageStore;
            if (store == null)
            {
                throw new NotSupportedException();
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
                Store.Dispose();
            }
            _disposed = true;
            Store = null;
        }
    }
}
