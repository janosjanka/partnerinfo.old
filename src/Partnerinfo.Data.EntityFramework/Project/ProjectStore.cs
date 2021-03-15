// Copyright (c) János Janka. All rights reserved.

using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Globalization;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Partnerinfo.Identity.EntityFramework;
using Partnerinfo.Security.EntityFramework;

namespace Partnerinfo.Project.EntityFramework
{
    /// <summary>
    /// Provides facilities for querying and working with system data as objects.
    /// </summary>
    public class ProjectStore :
        IProjectStore,
        IProjectActionStore,
        IProjectBusinessTagStore,
        IProjectContactStore,
        IProjectContactTagStore,
        IProjectMailMessageStore
    {
        private bool _disposed;

        /// <summary>
        /// Initializes a new instance of the <see cref="ProjectStore" /> class.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <exception cref="System.ArgumentNullException"></exception>
        public ProjectStore(DbContext context)
        {
            if (context == null)
            {
                throw new ArgumentNullException(nameof(context));
            }
            Context = context;
        }

        /// <summary>
        /// If true will call SaveChanges after CreateAsync/UpdateAsync/DeleteAsync.
        /// </summary>
        /// <value>
        ///   <c>true</c> if save changes; otherwise, <c>false</c>.
        /// </value>
        public bool AutoSaveChanges { get; set; } = true;

        /// <summary>
        /// Gets the context for the store.
        /// </summary>
        /// <value>
        /// The context.
        /// </value>
        public DbContext Context { get; }

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
        public virtual Task<ListResult<ProjectItem>> FindAllAsync(int userId, string name, ProjectSortOrder orderBy, int pageIndex, int pageSize, ProjectField fields, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            ThrowIfDisposed();
            return Projects
                .AsNoTracking()
                .Where(ACL, userId, name)
                .OrderBy(orderBy)
                .Select(ACL, Users, fields)
                .ToListResultAsync(pageIndex, pageSize, cancellationToken);
        }

        /// <summary>
        /// Finds a project with the given primary key value.
        /// </summary>
        /// <param name="id">The primary key for the item to be found.</param>
        /// <param name="cancellationToken">A token to observe while waiting for the task to complete.</param>
        /// <returns>
        /// A task that represents the asynchronous operation.
        /// </returns>
        public virtual Task<ProjectItem> FindByIdAsync(int id, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            ThrowIfDisposed();
            return Projects
                .Where(p => p.Id == id)
                .Select(ACL, Users, ProjectField.All)
                .FirstOrDefaultAsync(cancellationToken);
        }

        /// <summary>
        /// Inserts a project.
        /// </summary>
        /// <param name="project">The project to insert.</param>
        /// <param name="cancellationToken">A token to observe while waiting for the task to complete.</param>
        /// <returns>
        /// A task that represents the asynchronous operation.
        /// </returns>
        public virtual async Task<ValidationResult> CreateAsync(ProjectItem project, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            ThrowIfDisposed();
            if (project == null)
            {
                throw new ArgumentNullException(nameof(project));
            }
            var projectEntity = Context.Add(new ProjectEntity { Name = project.Name, Sender = project.Sender });
            await SaveChangesAsync(cancellationToken);
            project.Id = projectEntity.Id;
            project.CreatedDate = projectEntity.CreatedDate;
            project.ModifiedDate = projectEntity.ModifiedDate;
            return ValidationResult.Success;
        }

        /// <summary>
        /// Updates a project.
        /// </summary>
        /// <param name="project">The project to update.</param>
        /// <param name="cancellationToken">A token to observe while waiting for the task to complete.</param>
        /// <returns>
        /// A task that represents the asynchronous operation.
        /// </returns>
        public virtual async Task<ValidationResult> UpdateAsync(ProjectItem project, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            ThrowIfDisposed();
            if (project == null)
            {
                throw new ArgumentNullException(nameof(project));
            }
            var projectEntity = await Projects.FindAsync(cancellationToken, project.Id);
            if (projectEntity == null)
            {
                throw new InvalidOperationException(string.Format(CultureInfo.CurrentCulture, ProjectResources.ProjectNotFound, project.Id));
            }
            Context.Patch(projectEntity, new { project.Name, project.Sender });
            try
            {
                await SaveChangesAsync(cancellationToken);
            }
            catch (DbUpdateConcurrencyException)
            {
                return ValidationResult.Failed(ProjectResources.ConcurrencyFailure);
            }
            project.ModifiedDate = projectEntity.ModifiedDate;
            return ValidationResult.Success;
        }

        /// <summary>
        /// Deletes a project.
        /// </summary>
        /// <param name="project">The project to delete.</param>
        /// <param name="cancellationToken">A token to observe while waiting for the task to complete.</param>
        /// <returns>
        /// A task that represents the asynchronous operation.
        /// </returns>
        public virtual async Task<ValidationResult> DeleteAsync(ProjectItem project, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            ThrowIfDisposed();
            if (project == null)
            {
                throw new ArgumentNullException(nameof(project));
            }
            var projectEntity = await Projects.FindAsync(cancellationToken, project.Id);
            if (projectEntity == null)
            {
                throw new InvalidOperationException(string.Format(CultureInfo.CurrentCulture, ProjectResources.ProjectNotFound, project.Id));
            }
            Context.Remove(projectEntity);
            try
            {
                await SaveChangesAsync(cancellationToken);
            }
            catch (DbUpdateConcurrencyException)
            {
                return ValidationResult.Failed(ProjectResources.ConcurrencyFailure);
            }
            return ValidationResult.Success;
        }

        /// <summary>
        /// Finds a collection of actions with the given values.
        /// </summary>
        /// <param name="project">The project which owns the actions.</param>
        /// <param name="name">The name to be found.</param>
        /// <param name="orderBy">The order in which items are returned in a result set.</param>
        /// <param name="pageIndex">The index of the page of results to return. Use 1 to indicate the first page.</param>
        /// <param name="pageSize">The size of the page of results to return. <paramref name="pageIndex" /> is non-zero-based.</param>
        /// <param name="fields">The related fields to include in the query results.</param>
        /// <param name="cancellationToken">A token to observe while waiting for the task to complete.</param>
        /// <returns>
        /// A task that represents the asynchronous operation.
        /// </returns>
        public virtual Task<ListResult<ActionItem>> GetActionsAsync(ProjectItem project, string name, ActionSortOrder orderBy, int pageIndex, int pageSize, ActionField fields, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            ThrowIfDisposed();
            if (project == null)
            {
                throw new ArgumentNullException(nameof(project));
            }
            return Actions
               .AsNoTracking()
               .Where(project.Id, name)
               .OrderBy(orderBy)
               .Paging(pageIndex, pageSize)
               .Select(fields)
               .ToListResultAsync(pageIndex, pageSize, cancellationToken);
        }

        /// <summary>
        /// Gets the information from the data source for the action.
        /// </summary>
        /// <param name="id">The action ID to be found.</param>
        /// <param name="cancellationToken">A token to observe while waiting for the task to complete.</param>
        /// <returns>
        /// A task that represents the asynchronous operation.
        /// </returns>
        public virtual async Task<ActionItem> GetActionByIdAsync(int id, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            ThrowIfDisposed();
            return MapToActionItem(await Actions.QueryById(id).ToArrayAsync(cancellationToken));
        }

        /// <summary>
        /// Gets the information from the data source for the action.
        /// </summary>
        /// <param name="actionLinkId">The unique action identifier from the data source for the action.</param>
        /// <param name="cancellationToken">A <see cref="System.Threading.CancellationToken" /> to observe while waiting for the task to complete.</param>
        /// <returns>
        /// Returns the action associated with the specified unique identifier.
        /// </returns>
        public virtual async Task<ActionItem> GetActionByLinkIdAsync(int actionLinkId, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            ThrowIfDisposed();
            return MapToActionItem(await Actions.SqlQuery("Project.GetActionByLinkId @ActionLinkId", DbParameters.Value("ActionLinkId", actionLinkId)).ToArrayAsync(cancellationToken));
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
        public virtual Task<Func<int>> AddActionAsync(ProjectItem project, ActionItem parentAction, ActionItem action, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            ThrowIfDisposed();
            if (project == null)
            {
                throw new ArgumentNullException(nameof(project));
            }
            if (action == null)
            {
                throw new ArgumentNullException(nameof(action));
            }

            var actionToAdd = Context.Add(new ProjectAction
            {
                ProjectId = project.Id,
                ParentId = parentAction?.Id,
                Type = action.Type,
                SortOrder = parentAction != null ? (byte)((Actions.Where(a => a.ParentId == parentAction.Id).Max(a => (byte?)a.SortOrder) ?? 0) + 1) : byte.MinValue,
                Name = action.Name,
                Enabled = action.Enabled,
                Options = action.Options != null ? JsonNetUtility.ObjectToXml(action.Options) : null
            });

            return Task.FromResult<Func<int>>(() => actionToAdd.Id);
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
        public virtual async Task ReplaceActionAsync(ProjectItem project, ActionItem action, ActionItem newAction, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            ThrowIfDisposed();
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

            var actionEntity = await Actions.FindAsync(cancellationToken, action.Id);
            if (actionEntity == null)
            {
                throw new InvalidOperationException("Action does not exist.");
            }
            Context.Patch(actionEntity, new
            {
                Type = newAction.Type,
                Enabled = newAction.Enabled,
                Name = newAction.Name,
                Options = newAction.Options != null ? JsonNetUtility.ObjectToXml(newAction.Options) : null
            });
        }

        /// <summary>
        /// Copies the specified action.
        /// </summary>
        /// <param name="project">The project which owns the action.</param>
        /// <param name="action">The action.</param>
        /// <param name="cancellationToken">A token to observe while waiting for the task to complete.</param>
        /// <returns>
        /// A task that represents the asynchronous operation.
        /// </returns>
        public virtual async Task<Func<int>> CopyActionAsync(ProjectItem project, ActionItem action, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            ThrowIfDisposed();
            if (project == null)
            {
                throw new ArgumentNullException(nameof(project));
            }
            if (action == null)
            {
                throw new ArgumentNullException(nameof(action));
            }

            var actions = await Actions.QueryById(action.Id).ToArrayAsync(cancellationToken);
            if (actions.Length == 0)
            {
                throw new InvalidOperationException(string.Format(CultureInfo.CurrentCulture, ProjectResources.ActionNotFound, action.Id));
            }
            foreach (var a in actions)
            {
                var entry = Context.Entry(a);
                entry.State = EntityState.Added;
                entry.Entity.ModifiedDate = DateTime.UtcNow;
            }
            var actionEntity = actions[0];
            return () => actionEntity.Id;
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
        public virtual async Task MoveActionBeforeAsync(ProjectItem project, ActionItem action, ActionItem referenceAction, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            ThrowIfDisposed();
            if (project == null)
            {
                throw new ArgumentNullException(nameof(project));
            }
            if (action == null)
            {
                throw new ArgumentNullException(nameof(action));
            }

            var actionEntity = await Actions.FindAsync(cancellationToken, action.Id);
            if (actionEntity == null)
            {
                throw new InvalidOperationException(string.Format(CultureInfo.CurrentCulture, ProjectResources.ActionNotFound, action.Id));
            }

            if (referenceAction == null)
            {
                // This is the most simple use-case. If referenceAction is null, move the specified action to
                // the end of the list of child nodes. It will generate duplicated SortOrder if you do not save the entity before moving an other one.
                actionEntity.SortOrder = (byte)((Actions.Where(a => a.ParentId == actionEntity.ParentId).Max(a => (byte?)a.SortOrder) ?? 0) + 1);
            }
            else
            {
                var referenceActionEntity = await Actions.FindAsync(cancellationToken, referenceAction.Id);
                if (referenceActionEntity?.ProjectId != actionEntity.ProjectId)
                {
                    throw new InvalidOperationException(string.Format(CultureInfo.CurrentCulture, ProjectResources.ReferenceActionNotFound, action.Id));
                }
                if (referenceActionEntity.ParentId == null)
                {
                    throw new InvalidOperationException(string.Format(CultureInfo.CurrentCulture, ProjectResources.ReferenceActionCannotBeRoot, action.Id));
                }

                // An action can also be moved to another level.
                actionEntity.ParentId = referenceActionEntity.ParentId;

                // Get the child actions at the specified level to reindex them.
                var actionEntities = await Actions
                    .Where(a => a.ParentId == referenceActionEntity.ParentId)
                    .OrderBy(a => a.SortOrder)
                    .ToListAsync(cancellationToken);

                var actionEntityIndex = actionEntities.FindIndex(a => a.Id == actionEntity.Id);
                if (actionEntityIndex >= 0)
                {
                    actionEntities.RemoveAt(actionEntityIndex);
                }
                var referenceActionIndex = actionEntities.FindIndex(a => a.Id == referenceAction.Id);
                actionEntities.Insert(referenceActionIndex, actionEntity);
                for (byte i = 0; i < actionEntities.Count; ++i)
                {
                    actionEntities[i].SortOrder = i;
                }
            }
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
        public virtual async Task RemoveActionAsync(ProjectItem project, ActionItem action, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            ThrowIfDisposed();
            if (project == null)
            {
                throw new ArgumentNullException(nameof(project));
            }
            if (action == null)
            {
                throw new ArgumentNullException(nameof(action));
            }

            var actionEntities = await Actions.QueryById(action.Id).ToArrayAsync(cancellationToken);
            foreach (var actionEntity in actionEntities)
            {
                Context.Remove(actionEntity);
            }
        }

        /// <summary>
        /// Gets a list of <see cref="BusinessTagItem" />s to be belonging to the specified <paramref name="project" /> as an asynchronous operation.
        /// </summary>
        /// <param name="project">The project whose associated business tags to retrieve.</param>
        /// <param name="cancellationToken">A token to observe while waiting for the task to complete.</param>
        /// <returns>
        /// The task object representing the asynchronous operation.
        /// </returns>
        public virtual Task<ListResult<BusinessTagItem>> GetBusinessTagsAsync(ProjectItem project, CancellationToken cancellationToken)
        {
            ThrowIfDisposed();
            if (project == null)
            {
                throw new ArgumentNullException(nameof(project));
            }

            return (
                from businessTag in BusinessTags.AsNoTracking()
                where businessTag.ProjectId == project.Id
                orderby businessTag.Name
                select new BusinessTagItem
                {
                    Id = businessTag.Id,
                    Name = businessTag.Name,
                    Color = businessTag.Color,
                    Project = new UniqueItem { Id = businessTag.Project.Id, Name = businessTag.Project.Name },
                    ItemCount = businessTag.Contacts.Count()
                }
            ).ToListResultAsync(cancellationToken);
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
            return BusinessTags
                .Where(businessTag => businessTag.Id == id)
                .Select(businessTag => new BusinessTagItem
                {
                    Id = businessTag.Id,
                    Name = businessTag.Name,
                    Color = businessTag.Color,
                    Project = new UniqueItem { Id = businessTag.Project.Id, Name = businessTag.Project.Name },
                    ItemCount = businessTag.Contacts.Count()
                })
                .FirstOrDefaultAsync(cancellationToken);
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
        public virtual Task<BusinessTagItem> GetBusinessTagByNameAsync(ProjectItem project, string name, CancellationToken cancellationToken)
        {
            ThrowIfDisposed();
            if (project == null)
            {
                throw new ArgumentNullException(nameof(project));
            }

            return BusinessTags
                .Where(businessTag => businessTag.ProjectId == project.Id && businessTag.Name == name)
                .Select(businessTag => new BusinessTagItem
                {
                    Id = businessTag.Id,
                    Name = businessTag.Name,
                    Color = businessTag.Color,
                    Project = new UniqueItem { Id = businessTag.Project.Id, Name = businessTag.Project.Name },
                    ItemCount = businessTag.Contacts.Count()
                })
                .FirstOrDefaultAsync(cancellationToken);
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
        public virtual Task AddBusinessTagsAsync(ProjectItem project, IEnumerable<BusinessTagItem> businessTags, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            ThrowIfDisposed();
            if (project == null)
            {
                throw new ArgumentNullException(nameof(project));
            }
            if (businessTags == null)
            {
                throw new ArgumentNullException(nameof(businessTags));
            }

            foreach (var businessTag in businessTags)
            {
                BusinessTags.Add(new ProjectBusinessTag { ProjectId = project.Id, Name = businessTag.Name, Color = businessTag.Color });
            }

            return Task.CompletedTask;
        }

        /// <summary>
        /// Add a business tag to a project as an asynchronous operation.
        /// </summary>
        /// <param name="project">The project to add the business tag to.</param>
        /// <param name="businessTag">The business tag to add.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken" /> used to propagate notifications that the operation should be canceled.</param>
        /// <returns>
        /// The task object representing the asynchronous operation.
        /// </returns>
        public virtual Task<Func<int>> AddBusinessTagAsync(ProjectItem project, BusinessTagItem businessTag, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            ThrowIfDisposed();
            if (project == null)
            {
                throw new ArgumentNullException(nameof(project));
            }
            if (businessTag == null)
            {
                throw new ArgumentNullException(nameof(businessTag));
            }

            var businessTagEntity = BusinessTags.Add(new ProjectBusinessTag
            {
                ProjectId = project.Id,
                Name = businessTag.Name,
                Color = businessTag.Color
            });
            return Task.FromResult<Func<int>>(() => businessTagEntity.Id);
        }

        /// <summary>
        /// Replaces the given <paramref name="businessTag" /> on the specified <paramref name="project" /> with the <paramref name="newBusinessTag" /></summary>
        /// <param name="project">The project which owns the business tag.</param>
        /// <param name="businessTag">The business tag to replace.</param>
        /// <param name="newBusinessTag">The new business tag to replace the existing <paramref name="businessTag" /> with.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken" /> used to propagate notifications that the operation should be canceled.</param>
        /// <returns>
        /// The task object representing the asynchronous operation.
        /// </returns>
        public virtual async Task ReplaceBusinessTagAsync(ProjectItem project, BusinessTagItem businessTag, BusinessTagItem newBusinessTag, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            ThrowIfDisposed();
            if (project == null)
            {
                throw new ArgumentNullException(nameof(project));
            }
            if (businessTag == null)
            {
                throw new ArgumentNullException(nameof(businessTag));
            }
            if (newBusinessTag == null)
            {
                throw new ArgumentNullException(nameof(newBusinessTag));
            }

            var oldBusinessTag = await BusinessTags.FirstOrDefaultAsync(t => t.Id == businessTag.Id, cancellationToken);
            if (oldBusinessTag != null)
            {
                newBusinessTag.Id = oldBusinessTag.Id;
                oldBusinessTag.Name = newBusinessTag.Name;
                oldBusinessTag.Color = newBusinessTag.Color;
            }
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
        public virtual async Task RemoveBusinessTagsAsync(ProjectItem project, IEnumerable<BusinessTagItem> businessTags, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            ThrowIfDisposed();
            if (project == null)
            {
                throw new ArgumentNullException(nameof(project));
            }
            if (businessTags == null)
            {
                throw new ArgumentNullException(nameof(businessTags));
            }

            var businessTagList = await BusinessTags
                .Where(project.Id, businessTags.Select(t => t.Id))
                .ToArrayAsync(cancellationToken);

            foreach (var businessTag in businessTagList)
            {
                Context.Remove(businessTag);
            }
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
        public virtual async Task<ListResult<ContactItem>> GetContactsAsync(ProjectItem project, string name, IEnumerable<int> includeWithTags, IEnumerable<int> excludeWithTags, ContactSortOrder orderBy, int pageIndex, int pageSize, ContactField fields, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            ThrowIfDisposed();
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

            return await Contacts
                .AsNoTracking()
                .Where(project.Id, name, includeWithTags, excludeWithTags)
                .OrderBy(orderBy)
                .Select(BusinessTags, fields)
                .ToListResultAsync(pageIndex, pageSize, cancellationToken);
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
        public virtual async Task<IList<ContactItem>> GetContactsAsync(ProjectItem project, IEnumerable<int> ids, ContactSortOrder orderBy, ContactField fields, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            ThrowIfDisposed();
            if (project == null)
            {
                throw new ArgumentNullException(nameof(project));
            }
            if (ids == null)
            {
                throw new ArgumentNullException(nameof(ids));
            }

            return await Contacts
                .AsNoTracking()
                .Where(project.Id, ids)
                .OrderBy(orderBy)
                .Select(BusinessTags, fields)
                .ToListAsync(cancellationToken);
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
            cancellationToken.ThrowIfCancellationRequested();
            ThrowIfDisposed();
            return Contacts.Where(c => c.Id == id).Select(BusinessTags, fields).FirstOrDefaultAsync(cancellationToken);
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
        public virtual Task<ContactItem> GetContactByFacebookIdAsync(ProjectItem project, long facebookId, ContactField fields, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            ThrowIfDisposed();
            if (project == null)
            {
                throw new ArgumentNullException(nameof(project));
            }
            return Contacts
                .Where(c => c.ProjectId == project.Id && c.FacebookId == facebookId)
                .Select(BusinessTags, fields)
                .FirstOrDefaultAsync(cancellationToken);
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
        public virtual Task<ContactItem> GetContactByMailAsync(ProjectItem project, string email, ContactField fields, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            ThrowIfDisposed();
            if (project == null)
            {
                throw new ArgumentNullException(nameof(project));
            }
            return Contacts
                .Where(c => c.ProjectId == project.Id && c.Email.Address == email)
                .Select(BusinessTags, fields)
                .FirstOrDefaultAsync(cancellationToken);
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
        public virtual Task AddContactsAsync(ProjectItem project, IEnumerable<ContactItem> contacts, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            ThrowIfDisposed();
            if (project == null)
            {
                throw new ArgumentNullException(nameof(project));
            }
            if (contacts == null)
            {
                throw new ArgumentNullException(nameof(contacts));
            }

            foreach (var contact in contacts)
            {
                var contactEntity = new ProjectContact
                {
                    ProjectId = project.Id,
                    FirstName = contact.FirstName,
                    LastName = contact.LastName,
                    NickName = contact.LastName,
                    Gender = contact.Gender,
                    Birthday = contact.Birthday,
                    Comment = contact.Comment
                };
                if (contact.Email != null)
                {
                    contactEntity.Email = contact.Email;
                }
                if (contact.Phones != null)
                {
                    contactEntity.Phones = contact.Phones;
                }
                Contacts.Add(contactEntity);
            }
            return Task.CompletedTask;
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
        public virtual Task<Func<int>> AddContactAsync(ProjectItem project, ContactItem contact, CancellationToken cancellationToken)
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

            var contactEntity = Contacts.Add(new ProjectContact
            {
                ProjectId = project.Id,
                SponsorId = contact.Sponsor?.Id,
                FacebookId = contact.FacebookId,
                Email = contact.Email ?? MailAddressItem.None,
                FirstName = contact.FirstName,
                LastName = contact.LastName,
                NickName = contact.LastName,
                Gender = contact.Gender,
                Birthday = contact.Birthday?.ToLocalTime(),
                Phones = contact.Phones ?? PhoneGroupItem.Empty,
                Comment = contact.Comment
            });
            return Task.FromResult<Func<int>>(() => contactEntity.Id);
        }

        /// <summary>
        /// Replaces the given <paramref name="contact" /> on the specified <paramref name="project" /> with the <paramref name="newContact" /></summary>
        /// <param name="project">The project which owns the contact.</param>
        /// <param name="contact">The contact to replace.</param>
        /// <param name="newContact">The new contact to replace the existing <paramref name="contact" /> with.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken" /> used to propagate notifications that the operation should be canceled.</param>
        /// <returns>
        /// The task object representing the asynchronous operation.
        /// </returns>
        public virtual async Task ReplaceContactAsync(ProjectItem project, ContactItem contact, ContactItem newContact, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            ThrowIfDisposed();
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

            var contactEntity = await Contacts.FirstOrDefaultAsync(c => c.ProjectId == project.Id && c.Id == contact.Id, cancellationToken);
            if (contactEntity != null)
            {
                newContact.Id = contactEntity.Id;
                contactEntity.Sponsor = null;
                contactEntity.SponsorId = newContact.Sponsor?.Id;
                contactEntity.FacebookId = newContact.FacebookId;
                contactEntity.Email = newContact.Email ?? MailAddressItem.None;
                contactEntity.FirstName = newContact.FirstName;
                contactEntity.LastName = newContact.LastName;
                contactEntity.NickName = newContact.NickName;
                contactEntity.Gender = newContact.Gender;
                contactEntity.Birthday = newContact.Birthday?.ToLocalTime();
                contactEntity.Phones = newContact.Phones ?? PhoneGroupItem.Empty;
                contactEntity.Comment = newContact.Comment;
                contactEntity.ModifiedDate = DateTime.UtcNow;
                // Context.Patch(oldContact, newContact);
            }
        }

        /// <summary>
        /// Removes the specified <paramref name="contacts" /> from the given <paramref name="project" />.
        /// </summary>
        /// <param name="project">The project to remove the specified <paramref name="contacts" /> from.</param>
        /// <param name="contacts">A collection of <see cref="ContactItem" />s to remove.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken" /> used to propagate notifications that the operation should be canceled.</param>
        /// <returns>
        /// The task object representing the asynchronous operation.
        /// </returns>
        public virtual async Task RemoveContactsAsync(ProjectItem project, IEnumerable<ContactItem> contacts, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            ThrowIfDisposed();
            if (project == null)
            {
                throw new ArgumentNullException(nameof(project));
            }
            if (contacts == null)
            {
                throw new ArgumentNullException(nameof(contacts));
            }

            var contactList = await Contacts
                .Where(project.Id, contacts.Select(c => c.Id))
                .ToArrayAsync(cancellationToken);

            foreach (var contact in contactList)
            {
                Context.Remove(contact);
            }
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
        public virtual Task SetBusinessTagsAsync(IEnumerable<int> contacts, IEnumerable<int> tagsToAdd, IEnumerable<int> tagsToRemove, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            ThrowIfDisposed();
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

            return Context.Database.ExecuteSqlCommandAsync(
                "Project.SetContactTags @ContactList, @IncludeList, @ExcludeList",
                cancellationToken,
                DbParameters.IdList("ContactList", contacts),
                DbParameters.IdList("IncludeList", tagsToAdd),
                DbParameters.IdList("ExcludeList", tagsToRemove));
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
        public virtual Task<bool> HasBusinessTagsAsync(ContactItem contact, IEnumerable<int> includeWithTags, IEnumerable<int> excludeWithTags, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            ThrowIfDisposed();
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

            return Contacts.Where(includeWithTags, excludeWithTags).AnyAsync(c => c.Id == contact.Id, cancellationToken);
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
            cancellationToken.ThrowIfCancellationRequested();
            ThrowIfDisposed();
            if (project == null)
            {
                throw new ArgumentNullException(nameof(project));
            }

            return MailMessages
                .AsNoTracking()
                .Where(project.Id, subject)
                .OrderBy(orderBy)
                .Select(fields)
                .ToListResultAsync(pageIndex, pageSize, cancellationToken);
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
            return MailMessages.Where(m => m.Id == id).Select(fields).FirstOrDefaultAsync(cancellationToken);
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
        public virtual Task<Func<int>> AddMailMessageAsync(ProjectItem project, MailMessageItem mailMessage, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            ThrowIfDisposed();
            if (project == null)
            {
                throw new ArgumentNullException(nameof(project));
            }
            if (mailMessage == null)
            {
                throw new ArgumentNullException(nameof(mailMessage));
            }

            var mailMessageEntity = Context.Add(new ProjectMailMessage
            {
                ProjectId = project.Id,
                Subject = mailMessage.Subject,
                Body = mailMessage.Body
            });
            return Task.FromResult<Func<int>>(() => mailMessageEntity.Id);
        }

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
        public virtual async Task ReplaceMailMessageAsync(ProjectItem project, MailMessageItem mailMessage, MailMessageItem newMailMessage, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            ThrowIfDisposed();
            if (project == null)
            {
                throw new ArgumentNullException(nameof(project));
            }
            if (mailMessage == null)
            {
                throw new ArgumentNullException(nameof(mailMessage));
            }
            if (newMailMessage == null)
            {
                throw new ArgumentNullException(nameof(newMailMessage));
            }

            var mailMessageEntity = await MailMessages.FindAsync(cancellationToken, mailMessage.Id);
            if (mailMessageEntity != null)
            {
                Context.Patch(mailMessageEntity, new
                {
                    Subject = newMailMessage.Subject,
                    Body = newMailMessage.Body
                });
            }
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
        public virtual async Task RemoveMailMessageAsync(ProjectItem project, MailMessageItem mailMessage, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            ThrowIfDisposed();
            if (project == null)
            {
                throw new ArgumentNullException(nameof(project));
            }
            if (mailMessage == null)
            {
                throw new ArgumentNullException(nameof(mailMessage));
            }

            var mailMessageToDelete = await MailMessages.FindAsync(cancellationToken, mailMessage.Id);
            if (mailMessageToDelete != null)
            {
                Context.Remove(mailMessageToDelete);
            }
        }

        private static ActionItem MapToActionItem(IEnumerable<ProjectAction> actions)
        {
            var arr = actions.Select(a => new ActionItem
            {
                Id = a.Id,
                Parent = a.ParentId == null ? null : new UniqueItem { Id = (int)a.ParentId },
                Project = new UniqueItem { Id = a.ProjectId },
                Type = a.Type,
                Enabled = a.Enabled,
                ModifiedDate = a.ModifiedDate,
                Name = a.Name,
                Options = a.Options != null ? JsonNetUtility.XmlToJObject(a.Options) : null
            });

            return TreeUtility.BuildTreeNode(
                arr.ToArray(),
                p => p.Id,
                p => p.Parent?.Id,
                (parent, child) => parent.Children.Add(child));
        }

        /// <summary>
        /// The <see cref="DbSet" /> for access to identity Access Control Entries  in the context.
        /// </summary>
        /// <value>
        /// The identity Access Control Entries.
        /// </value>
        private DbSet<SecurityAccessRule> ACL => Context.Set<SecurityAccessRule>();

        /// <summary>
        /// The <see cref="DbSet" /> for access to identity users in the context.
        /// </summary>
        /// <value>
        /// The identity users.
        /// </value>
        private DbSet<IdentityUser> Users => Context.Set<IdentityUser>();

        /// <summary>
        /// The <see cref="DbSet" /> for access to projects in the context.
        /// </summary>
        /// <value>
        /// The projects.
        /// </value>
        private DbSet<ProjectEntity> Projects => Context.Set<ProjectEntity>();

        /// <summary>
        /// The <see cref="DbSet" /> for access to business tags in the context.
        /// </summary>
        /// <value>
        /// The business tags.
        /// </value>
        private DbSet<ProjectBusinessTag> BusinessTags => Context.Set<ProjectBusinessTag>();

        /// <summary>
        /// The <see cref="DbSet" /> for access to contacts in the context.
        /// </summary>
        /// <value>
        /// The contacts.
        /// </value>
        private DbSet<ProjectContact> Contacts => Context.Set<ProjectContact>();

        /// <summary>
        /// The <see cref="DbSet" /> for access to business tags in the context.
        /// </summary>
        /// <value>
        /// The contact tags.
        /// </value>
        private DbSet<ProjectContactTag> ContactTags => Context.Set<ProjectContactTag>();

        /// <summary>
        /// The <see cref="DbSet" /> for access to actions in the context.
        /// </summary>
        /// <value>
        /// The actions.
        /// </value>
        private DbSet<ProjectAction> Actions => Context.Set<ProjectAction>();

        /// <summary>
        /// The <see cref="DbSet" /> for access to mail messages in the context.
        /// </summary>
        /// <value>
        /// The mail messages.
        /// </value>
        private DbSet<ProjectMailMessage> MailMessages => Context.Set<ProjectMailMessage>();

        /// <summary>
        /// Saves all changes made in this context to the underlying database.
        /// </summary>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>
        /// A task that represents the asynchronous operation.
        /// </returns>
        private Task SaveChangesAsync(CancellationToken cancellationToken)
        {
            return AutoSaveChanges ? Context.SaveChangesAsync(cancellationToken) : Task.FromResult(0);
        }

        /// <summary>
        /// Throws a <see cref="ObjectDisposedException" /> if the context has already been disposed.
        /// </summary>
        /// <exception cref="System.ObjectDisposedException"></exception>
        private void ThrowIfDisposed()
        {
            if (_disposed)
            {
                throw new ObjectDisposedException(GetType().Name);
            }
        }

        /// <summary>
        /// Releases unmanaged and - optionally - managed resources.
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// If disposing, calls dispose on the Context.  Always nulls out the Context
        /// </summary>
        /// <param name="disposing"><c>true</c> to release both managed and unmanaged resources; <c>false</c> to release only unmanaged resources.</param>
        protected virtual void Dispose(bool disposing)
        {
            // Do not dispose the context here because it must be performed by the caller
            // An ASP.NET application, which uses dependency injection with lifetime configuration, 
            // can be crashed if the context is disposed prematurely

            _disposed = true;
        }
    }
}
