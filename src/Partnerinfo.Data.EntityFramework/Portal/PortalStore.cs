// Copyright (c) János Janka. All rights reserved.

using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Globalization;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Partnerinfo.Identity.EntityFramework;
using Partnerinfo.Project.EntityFramework;
using Partnerinfo.Security.EntityFramework;

namespace Partnerinfo.Portal.EntityFramework
{
    public class PortalStore :
        IPortalStore,
        IPortalDomainStore,
        IPortalPageStore,
        IPortalPageReferenceStore,
        IPortalMediaStore
    {
        private bool _disposed;

        /// <summary>
        /// Initializes a new instance of the <see cref="PortalStore" /> class.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <exception cref="System.ArgumentNullException"></exception>
        public PortalStore(DbContext context)
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
        /// Gets the URI for the specified <paramref name="portal" />.
        /// </summary>
        /// <param name="portal">The portal whose URI should be retrieved.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken" /> used to propagate notifications that the operation should be canceled.</param>
        /// <returns>
        /// The task object representing the asynchronous operation.
        /// </returns>
        public virtual Task<string> GetUriAsync(PortalItem portal, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            ThrowIfDisposed();
            if (portal == null)
            {
                throw new ArgumentNullException(nameof(portal));
            }
            return Task.FromResult(portal.Uri);
        }

        /// <summary>
        /// Sets the <paramref name="normalizedUri" /> for a <paramref name="portal" />.
        /// </summary>
        /// <param name="portal">The portal whose name should be set.</param>
        /// <param name="normalizedUri">The normalized URI to set.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken" /> used to propagate notifications that the operation should be canceled.</param>
        /// <returns>
        /// The task object representing the asynchronous operation.
        /// </returns>
        public virtual Task SetUriAsync(PortalItem portal, string normalizedUri, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            ThrowIfDisposed();
            if (portal == null)
            {
                throw new ArgumentNullException(nameof(portal));
            }
            portal.Uri = normalizedUri;
            return Task.CompletedTask;
        }

        /// <summary>
        /// Creates the specified <paramref name="portal" /> in the portal store, as an asynchronous operation.
        /// </summary>
        /// <param name="portal">The portal to create.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken" /> used to propagate notifications that the operation should be canceled.</param>
        /// <returns>
        /// The <see cref="Task" /> that represents the asynchronous operation, containing the <see cref="ValidationResult" /> of the creation operation.
        /// </returns>
        public virtual async Task<ValidationResult> CreateAsync(PortalItem portal, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            ThrowIfDisposed();
            if (portal == null)
            {
                throw new ArgumentNullException(nameof(portal));
            }
            var portalEntity = Context.Add(Mapper.Map(portal, new PortalEntity()));
            await SaveChangesAsync(cancellationToken);
            Mapper.Map(portalEntity, portal);
            return ValidationResult.Success;
        }

        /// <summary>
        /// Updates the specified <paramref name="portal" /> in the portal store, as an asynchronous operation
        /// </summary>
        /// <param name="portal">The portal to update.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken" /> used to propagate notifications that the operation should be canceled.</param>
        /// <returns>
        /// The <see cref="Task" /> that represents the asynchronous operation, containing the <see cref="ValidationResult" /> of the update operation.
        /// </returns>
        public virtual async Task<ValidationResult> UpdateAsync(PortalItem portal, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            ThrowIfDisposed();
            if (portal == null)
            {
                throw new ArgumentNullException(nameof(portal));
            }
            var portalEntity = await Portals.FindAsync(cancellationToken, portal.Id);
            if (portalEntity == null)
            {
                throw new InvalidOperationException(string.Format(CultureInfo.CurrentCulture, PortalResources.PortalNotFound, portal.Uri));
            }
            Mapper.Map(portal, portalEntity);
            try
            {
                await SaveChangesAsync(cancellationToken);
            }
            catch (DbUpdateConcurrencyException)
            {
                return ValidationResult.Failed(PortalResources.ConcurrencyFailure);
            }
            Mapper.Map(portalEntity, portal);
            return ValidationResult.Success;
        }

        /// <summary>
        /// Copies an existing portal to a new portal.
        /// </summary>
        /// <param name="portal">The portal to copy.</param>
        /// <param name="normalizedUri">The normalized uri for the new portal.</param>
        /// <param name="cancellationToken">A token to observe while waiting for the task to complete.</param>
        /// <returns>
        /// A task that represents the asynchronous operation.
        /// </returns>
        public virtual async Task<ValidationResult> CopyAsync(PortalItem portal, string normalizedUri, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            ThrowIfDisposed();
            if (portal == null)
            {
                throw new ArgumentNullException(nameof(portal));
            }

            await Context.Database.ExecuteSqlCommandAsync(
                $"{DbSchema.Portal}.CopyPortal @Uri, @NewUri",
                cancellationToken,
                DbParameters.Value("Uri", portal.Uri),
                DbParameters.Value("NewUri", normalizedUri));

            return ValidationResult.Success;
        }

        /// <summary>
        /// Deletes the specified <paramref name="portal" /> from the portal store, as an asynchronous operation.
        /// </summary>
        /// <param name="portal">The portal to delete.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken" /> used to propagate notifications that the operation should be canceled.</param>
        /// <returns>
        /// The <see cref="Task" /> that represents the asynchronous operation, containing the <see cref="ValidationResult" /> of the update operation.
        /// </returns>
        public virtual async Task<ValidationResult> DeleteAsync(PortalItem portal, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            ThrowIfDisposed();
            if (portal == null)
            {
                throw new ArgumentNullException(nameof(portal));
            }
            var portalEntity = await Portals.FindAsync(cancellationToken, portal.Id);
            if (portalEntity == null)
            {
                throw new InvalidOperationException(string.Format(CultureInfo.CurrentCulture, PortalResources.PortalNotFound, portal.Uri));
            }
            Context.Remove(portalEntity);
            try
            {
                await SaveChangesAsync(cancellationToken);
            }
            catch (DbUpdateConcurrencyException)
            {
                return ValidationResult.Failed(PortalResources.ConcurrencyFailure);
            }
            return ValidationResult.Success;
        }

        /// <summary>
        /// Finds and returns a portal, if any, which has the specified normalized uri.
        /// </summary>
        /// <param name="normalizedUri">The normalized uri to search for.</param>
        /// <param name="fields">The related fields to include in the query results.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken" /> used to propagate notifications that the operation should be canceled.</param>
        /// <returns>
        /// The <see cref="Task" /> that represents the asynchronous operation, containing the portal matching the specified <paramref name="normalizedUri" /> if it exists.
        /// </returns>
        public virtual Task<PortalItem> FindByUriAsync(string normalizedUri, PortalField fields, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            ThrowIfDisposed();
            return Portals
                .Where(p => p.Uri == normalizedUri)
                .Select(AccessRules, Users, fields)
                .SingleOrDefaultAsync(cancellationToken);
        }

        /// <summary>
        /// Finds and returns a portal, if any, which has the specified normalized domain.
        /// </summary>
        /// <param name="normalizedDomain">The normalized domain to search for.</param>
        /// <param name="fields">The related fields to include in the query results.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken" /> used to propagate notifications that the operation should be canceled.</param>
        /// <returns>
        /// The <see cref="Task" /> that represents the asynchronous operation, containing the portal matching the specified <paramref name="normalizedDomain" /> if it exists.
        /// </returns>
        public virtual Task<PortalItem> FindByDomainAsync(string normalizedDomain, PortalField fields, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            ThrowIfDisposed();
            return Portals
                .Where(p => p.Domain == normalizedDomain)
                .Select(AccessRules, Users, fields)
                .SingleOrDefaultAsync(cancellationToken);
        }

        /// <summary>
        /// Finds a collection of portals with the given values.
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
        public Task<ListResult<PortalItem>> FindAllAsync(int userId, string name, PortalSortOrder orderBy, int pageIndex, int pageSize, PortalField fields, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            ThrowIfDisposed();
            return Portals
                .AsNoTracking()
                .Where(AccessRules, userId, name)
                .OrderBy(orderBy)
                .Select(AccessRules, Users, fields)
                .ToListResultAsync(pageIndex, pageSize, cancellationToken);
        }

        /// <summary>
        /// Sets the <paramref name="homePage" /> for a <paramref name="portal" />.
        /// </summary>
        /// <param name="portal">The portal whose home page should be set.</param>
        /// <param name="homePage">The home page to set.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken" /> used to propagate notifications that the operation should be canceled.</param>
        /// <returns>
        /// The task object representing the asynchronous operation.
        /// </returns>
        public virtual async Task SetHomePageAsync(PortalItem portal, ResourceItem homePage, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            ThrowIfDisposed();
            if (portal == null)
            {
                throw new ArgumentNullException(nameof(portal));
            }
            var portalEntity = await Portals.FindAsync(cancellationToken, portal.Id);
            if (portalEntity == null)
            {
                throw new InvalidOperationException(string.Format(CultureInfo.CurrentCulture, PortalResources.PortalNotFound, portal.Uri));
            }
            if (homePage == null)
            {
                portalEntity.HomePageId = null;
                portalEntity.HomePage = null;
            }
            else
            {
                var pageEntity = await Pages.FindAsync(cancellationToken, homePage.Id);
                if (pageEntity == null)
                {
                    throw new InvalidOperationException(string.Format(CultureInfo.CurrentCulture, PortalResources.PageNotFound, pageEntity.Id));
                }
                portalEntity.HomePageId = pageEntity.Id;
                portalEntity.HomePage = pageEntity;
            }
        }

        /// <summary>
        /// Gets the home page for the specified <paramref name="portal" />.
        /// </summary>
        /// <param name="portal">The portal whose home page should be returned.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken" /> used to propagate notifications that the operation should be canceled.</param>
        /// <returns>
        /// The task object containing the results of the asynchronous operation, the home page for the specified <paramref name="portal" />.
        /// </returns>
        public virtual Task<ResourceItem> GetHomePageAsync(PortalItem portal, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            ThrowIfDisposed();
            if (portal == null)
            {
                throw new ArgumentNullException(nameof(portal));
            }
            return Task.FromResult(portal.HomePage);
        }

        /// <summary>
        /// Sets the <paramref name="masterPage" /> for a <paramref name="portal" />.
        /// </summary>
        /// <param name="portal">The portal whose master page should be set.</param>
        /// <param name="masterPage">The master page to set.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken" /> used to propagate notifications that the operation should be canceled.</param>
        /// <returns>
        /// The task object representing the asynchronous operation.
        /// </returns>
        public virtual async Task SetMasterPageAsync(PortalItem portal, ResourceItem masterPage, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            ThrowIfDisposed();
            if (portal == null)
            {
                throw new ArgumentNullException(nameof(portal));
            }
            var portalEntity = await Portals.FindAsync(cancellationToken, portal.Id);
            if (portalEntity == null)
            {
                throw new InvalidOperationException(string.Format(CultureInfo.CurrentCulture, PortalResources.PortalNotFound, portal.Uri));
            }
            if (masterPage == null)
            {
                portalEntity.MasterPageId = null;
                portalEntity.MasterPage = null;
            }
            else
            {
                var pageEntity = await Pages.FindAsync(cancellationToken, masterPage.Id);
                if (pageEntity == null)
                {
                    throw new InvalidOperationException(string.Format(CultureInfo.CurrentCulture, PortalResources.PageNotFound, pageEntity.Id));
                }
                portalEntity.MasterPageId = pageEntity.Id;
                portalEntity.MasterPage = pageEntity;
            }
        }

        /// <summary>
        /// Gets the master page for the specified <paramref name="portal" />.
        /// </summary>
        /// <param name="portal">The portal whose master page should be returned.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken" /> used to propagate notifications that the operation should be canceled.</param>
        /// <returns>
        /// The task object containing the results of the asynchronous operation, the master page for the specified <paramref name="portal" />.
        /// </returns>
        public virtual Task<ResourceItem> GetMasterPageAsync(PortalItem portal, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            ThrowIfDisposed();
            if (portal == null)
            {
                throw new ArgumentNullException(nameof(portal));
            }
            return Task.FromResult(portal.MasterPage);
        }

        /// <summary>
        /// Gets the project for the specified <paramref name="portal" />.
        /// </summary>
        /// <param name="portal">The portal whose project should be returned.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken" /> used to propagate notifications that the operation should be canceled.</param>
        /// <returns>
        /// The task object containing the results of the asynchronous operation, the project for the specified <paramref name="portal" />.
        /// </returns>
        public virtual Task<UniqueItem> GetProjectAsync(PortalItem portal, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            ThrowIfDisposed();
            if (portal == null)
            {
                throw new ArgumentNullException(nameof(portal));
            }
            return Task.FromResult(portal.Project);
        }

        /// <summary>
        /// Sets the <paramref name="project" /> for a <paramref name="portal" />.
        /// </summary>
        /// <param name="portal">The portal whose project should be set.</param>
        /// <param name="project">The project to set.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken" /> used to propagate notifications that the operation should be canceled.</param>
        /// <returns>
        /// The task object representing the asynchronous operation.
        /// </returns>
        public virtual async Task SetProjectAsync(PortalItem portal, UniqueItem project, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            ThrowIfDisposed();
            if (portal == null)
            {
                throw new ArgumentNullException(nameof(portal));
            }
            var portalEntity = await Portals.FindAsync(cancellationToken, portal.Id);
            if (portalEntity == null)
            {
                throw new InvalidOperationException(string.Format(CultureInfo.CurrentCulture, PortalResources.PortalNotFound, portal.Uri));
            }
            if (project == null)
            {
                portalEntity.ProjectId = null;
                portalEntity.Project = null;
            }
            else
            {
                var projectEntity = await Projects.FindAsync(cancellationToken, project.Id);
                if (projectEntity == null)
                {
                    throw new InvalidOperationException(string.Format(CultureInfo.CurrentCulture, PortalResources.ProjectNotFound, project.Id));
                }
                portalEntity.ProjectId = projectEntity.Id;
                portalEntity.Project = projectEntity;
            }
        }

        /// <summary>
        /// Gets the URI for the specified <paramref name="page" />.
        /// </summary>
        /// <param name="page">The page whose URI should be returned.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken" /> used to propagate notifications that the operation should be canceled.</param>
        /// <returns>
        /// The task object representing the asynchronous operation.
        /// </returns>
        public virtual Task<string> GetPageUriAsync(PortalItem portal, PageItem page, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            ThrowIfDisposed();
            if (portal == null)
            {
                throw new ArgumentNullException(nameof(portal));
            }
            if (page == null)
            {
                throw new ArgumentNullException(nameof(page));
            }
            return Context.Database.SqlQuery<string>(
                $"{DbSchema.Portal}.GetPageUriById @Id", DbParameters.Value("Id", page.Id))
                .FirstOrDefaultAsync(cancellationToken);
        }

        /// <summary>
        /// Sets the <paramref name="normalizedUri" /> for a <paramref name="page" />.
        /// </summary>
        /// <param name="page">The page whose URI should be set.</param>
        /// <param name="normalizedUri">The normalized URI to set.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken" /> used to propagate notifications that the operation should be canceled.</param>
        /// <returns>
        /// The task object representing the asynchronous operation.
        /// </returns>
        public virtual async Task SetPageUriAsync(PortalItem portal, PageItem page, string normalizedUri, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            ThrowIfDisposed();
            if (portal == null)
            {
                throw new ArgumentNullException(nameof(portal));
            }
            if (page == null)
            {
                throw new ArgumentNullException(nameof(page));
            }
            var pageEntity = await Pages.FindAsync(cancellationToken, page.Id);
            if (pageEntity == null)
            {
                throw new InvalidOperationException(string.Format(CultureInfo.CurrentCulture, PortalResources.PageNotFound, page.Id));
            }
            pageEntity.Uri = normalizedUri;
            page.Uri = normalizedUri;
        }

        /// <summary>
        /// Gets the master page for the specified <paramref name="page" />.
        /// </summary>
        /// <param name="portal">The portal whose master page should be returned.</param>
        /// <param name="page">The page whose master page should be returned.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken" /> used to propagate notifications that the operation should be canceled.</param>
        /// <returns>
        /// The task object containing the results of the asynchronous operation, the master page for the specified <paramref name="page" />.
        /// </returns>
        public virtual Task<PageItem> GetPageMasterAsync(PortalItem portal, PageItem page, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            ThrowIfDisposed();
            if (portal == null)
            {
                throw new ArgumentNullException(nameof(portal));
            }
            if (page == null)
            {
                throw new ArgumentNullException(nameof(page));
            }
            if (page.Master?.Id == null)
            {
                return Task.FromResult(default(PageItem));
            }
            return GetPageByIdAsync((int)page.Master?.Id, PageField.None, cancellationToken);
        }

        /// <summary>
        /// Sets the <paramref name="masterPage" /> for a <paramref name="page" />.
        /// </summary>
        /// <param name="portal">The portal whose master page should be set.</param>
        /// <param name="page">The page whose master page should be set.</param>
        /// <param name="masterPage">The master page to set.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken" /> used to propagate notifications that the operation should be canceled.</param>
        /// <returns>
        /// The task object representing the asynchronous operation.
        /// </returns>
        public virtual async Task SetPageMasterAsync(PortalItem portal, PageItem page, PageItem masterPage, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            ThrowIfDisposed();
            if (portal == null)
            {
                throw new ArgumentNullException(nameof(portal));
            }
            if (page == null)
            {
                throw new ArgumentNullException(nameof(page));
            }
            var pageEntity = await Pages.FindAsync(cancellationToken, page.Id);
            if (pageEntity == null)
            {
                throw new InvalidOperationException(string.Format(CultureInfo.CurrentCulture, PortalResources.PageNotFound, page.Id));
            }
            if (masterPage == null)
            {
                pageEntity.Master = null;
                pageEntity.MasterId = null;
            }
            else
            {
                var pageMasterEntity = await Pages.FindAsync(cancellationToken, masterPage.Id);
                if (pageMasterEntity == null)
                {
                    throw new InvalidOperationException(string.Format(CultureInfo.CurrentCulture, PortalResources.PageNotFound, masterPage.Id));
                }
                pageEntity.Master = pageMasterEntity;
                pageEntity.MasterId = pageMasterEntity.Id;
                page.Master = masterPage;
            }
        }

        /// <summary>
        /// Adds a new page.
        /// </summary>
        /// <param name="portal">The portal which owns the page.</param>
        /// <param name="parentPage">The parent page. If this parameter is null, the page will be a root page.</param>
        /// <param name="page">The page to insert.</param>
        /// <param name="cancellationToken">A token to observe while waiting for the task to complete.</param>
        /// <returns>
        /// A task that represents the asynchronous operation.
        /// </returns>
        public virtual async Task AddPageAsync(PortalItem portal, PageItem parentPage, PageItem page, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            ThrowIfDisposed();
            if (portal == null)
            {
                throw new ArgumentNullException(nameof(portal));
            }
            if (page == null)
            {
                throw new ArgumentNullException(nameof(page));
            }
            var portalEntity = await Portals.FindAsync(cancellationToken, portal.Id);
            if (portalEntity == null)
            {
                throw new InvalidOperationException(string.Format(CultureInfo.CurrentCulture, PortalResources.PortalNotFound, portal.Uri));
            }
            var pageEntity = Mapper.Map(page, new PortalPage());
            pageEntity.PortalId = portalEntity.Id;
            pageEntity.Portal = portalEntity;
            Context.Add(pageEntity);
            await SaveChangesAsync(cancellationToken);
            Mapper.Map(pageEntity, page);
        }

        /// <summary>
        /// Updates the given page information with the given new page information.
        /// </summary>
        /// <param name="portal">The portal which owns the page.</param>
        /// <param name="page">The page.</param>
        /// <param name="newPage">The new page.</param>
        /// <param name="cancellationToken">A token to observe while waiting for the task to complete.</param>
        /// <returns>
        /// A task that represents the asynchronous operation.
        /// </returns>
        public virtual async Task ReplacePageAsync(PortalItem portal, PageItem page, PageItem newPage, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            ThrowIfDisposed();
            if (portal == null)
            {
                throw new ArgumentNullException(nameof(portal));
            }
            if (page == null)
            {
                throw new ArgumentNullException(nameof(page));
            }
            if (newPage == null)
            {
                throw new ArgumentNullException(nameof(newPage));
            }

            var pageEntity = await Pages.FindAsync(cancellationToken, page.Id);
            if (pageEntity == null)
            {
                throw new InvalidOperationException(string.Format(CultureInfo.CurrentCulture, PortalResources.PageNotFound, page.Id));
            }
            Mapper.Map(newPage, pageEntity);
        }

        /// <summary>
        /// Copies an existing page to a new page.
        /// </summary>
        /// <param name="portal">The portal which owns the page.</param>
        /// <param name="page">The page to copy.</param>
        /// <param name="normalizedUri">The normalized URI to set.</param>
        /// <param name="cancellationToken">A token to observe while waiting for the task to complete.</param>
        /// <returns>
        /// A task that represents the asynchronous operation.
        /// </returns>
        public virtual Task CopyPageAsync(PortalItem portal, PageItem page, string normalizedUri, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            ThrowIfDisposed();
            if (portal == null)
            {
                throw new ArgumentNullException(nameof(portal));
            }
            if (page == null)
            {
                throw new ArgumentNullException(nameof(page));
            }
            if (normalizedUri == null)
            {
                throw new ArgumentNullException(nameof(normalizedUri));
            }

            return Context.Database.ExecuteSqlCommandAsync(
                $"{DbSchema.Portal}.CopyPage @PortalUri, @PageUris, @NewPageUri, @MaxLevel",
                cancellationToken,
                DbParameters.UriValue("PortalUri", portal.Uri),
                DbParameters.UriTable("PageUris", page.Uri?.Split('/')),
                DbParameters.UriValue("NewPageUri", normalizedUri),
                DbParameters.Value("MaxLevel", 16));
        }

        /// <summary>
        /// Moves the specified <paramref name="page" /> to a new location as a child of the current page.
        /// </summary>
        /// <param name="portal">The portal which owns the page.</param>
        /// <param name="page">The page to move.</param>
        /// <param name="referencePage">The reference page. If null, <paramref name="page" /> is inserted at the end of the list of child nodes.</param>
        /// <param name="cancellationToken">A token to observe while waiting for the task to complete.</param>
        /// <returns>
        /// A task that represents the asynchronous operation.
        /// </returns>
        public virtual async Task MovePageAsync(PortalItem portal, PageItem page, PageItem referencePage, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            ThrowIfDisposed();
            if (portal == null)
            {
                throw new ArgumentNullException(nameof(portal));
            }
            if (page == null)
            {
                throw new ArgumentNullException(nameof(page));
            }

            var pageEntity = await Pages.FindAsync(cancellationToken, page.Id);
            if (pageEntity == null)
            {
                throw new InvalidOperationException(string.Format(CultureInfo.CurrentCulture, PortalResources.PageNotFound, page.Id));
            }
            if (referencePage == null)
            {
                pageEntity.ParentId = null;
            }
            else
            {
                var referencePageEntity = await Pages.FindAsync(cancellationToken, referencePage.Id);
                if (referencePageEntity == null)
                {
                    throw new InvalidOperationException(string.Format(CultureInfo.CurrentCulture, PortalResources.PageNotFound, referencePage.Id));
                }
                pageEntity.ParentId = referencePageEntity.Id;
            }
        }

        /// <summary>
        /// Removes a page.
        /// </summary>
        /// <param name="portal">The portal which owns the page.</param>
        /// <param name="page">The page to remove.</param>
        /// <param name="cancellationToken">A token to observe while waiting for the task to complete.</param>
        /// <returns>
        /// A task that represents the asynchronous operation.
        /// </returns>
        public virtual async Task RemovePageAsync(PortalItem portal, PageItem page, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            ThrowIfDisposed();
            if (portal == null)
            {
                throw new ArgumentNullException(nameof(portal));
            }
            if (page == null)
            {
                throw new ArgumentNullException(nameof(page));
            }
            var pageEntity = await Pages.FindAsync(cancellationToken, page.Id);
            if (pageEntity != null)
            {
                Context.Remove(pageEntity);
            }
        }

        /// <summary>
        /// Finds and returns a page, if any, which has the specified primary key.
        /// </summary>
        /// <param name="id">The primary key for the item to be found.</param>
        /// <param name="fields">The related fields to include in the query results.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken" /> used to propagate notifications that the operation should be canceled.</param>
        /// <returns>
        /// The <see cref="Task" /> that represents the asynchronous operation, containing the page matching the specified <paramref name="id" /> if it exists.
        /// </returns>
        public virtual async Task<PageItem> GetPageByIdAsync(int id, PageField fields, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            ThrowIfDisposed();
            var page = await Pages.Where(p => p.Id == id).Select(fields).SingleOrDefaultAsync(cancellationToken);
            if (page == null)
            {
                return null;
            }
            if (page.Master != null)
            {
                page.Master.Uri = await GetPageUriByIdAsync(page.Master.Id, cancellationToken);
            }
            return page;
        }

        /// <summary>
        /// Finds and returns a page, if any, which has the specified normalized uri.
        /// </summary>
        /// <param name="portal">The portal.</param>
        /// <param name="normalizedUri">The normalized uri to search for.</param>
        /// <param name="fields">The related fields to include in the query results.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken" /> used to propagate notifications that the operation should be canceled.</param>
        /// <returns>
        /// The <see cref="Task" /> that represents the asynchronous operation, containing the page matching the specified <paramref name="normalizedUri" /> if it exists.
        /// </returns>
        public virtual async Task<PageItem> GetPageByUriAsync(PortalItem portal, string normalizedUri, PageField fields, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            ThrowIfDisposed();
            if (portal == null)
            {
                throw new ArgumentNullException(nameof(portal));
            }
            var pageId = await GetPageIdByUriAsync(portal.Uri, normalizedUri, cancellationToken);
            if (pageId == null)
            {
                return null;
            }
            var page = await Pages.Where(p => p.Id == pageId).Select(fields).SingleOrDefaultAsync(cancellationToken);
            if (page == null)
            {
                return null;
            }
            if (page.Master != null)
            {
                page.Master.Uri = await GetPageUriByIdAsync(page.Master.Id, cancellationToken);
            }
            return page;
        }

        /// <summary>
        /// Gets a list of <see cref="PageItem" />s to be belonging to the specified <paramref name="portal" /> as an asynchronous operation.
        /// </summary>
        /// <param name="portal">The portal which owns the pages.</param>
        /// <param name="cancellationToken">A token to observe while waiting for the task to complete.</param>
        /// <returns>
        /// A task that represents the asynchronous operation.
        /// </returns>
        public virtual async Task<IList<PageItem>> GetPagesAsync(PortalItem portal, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            ThrowIfDisposed();
            if (portal == null)
            {
                throw new ArgumentNullException(nameof(portal));
            }

            var pageList = new List<PageItem>();
            var pageDictionary = await Context.Database.SqlQuery<PortalPage>(
                $"{DbSchema.Portal}.GetPages @PortalUri", DbParameters.Value("PortalUri", portal.Uri))
                .ToDictionaryAsync(
                    p => p.Id,
                    p => new Tuple<PortalPage, PageItem>(p, new PageItem
                    {
                        Id = p.Id,
                        Uri = p.Uri,
                        ModifiedDate = p.ModifiedDate,
                        Name = p.Name,
                        Description = p.Description
                    }),
                    cancellationToken);

            Tuple<PortalPage, PageItem> parent;
            Tuple<PortalPage, PageItem> master;

            foreach (var page in pageDictionary.Values)
            {
                if (page.Item1.MasterId != null && pageDictionary.TryGetValue((int)page.Item1.MasterId, out master))
                {
                    // Map the master page to a new object to avoid circular references
                    // when its child collection will be serialized to XML or JSON.
                    page.Item2.Master = new ResourceItem
                    {
                        Id = master.Item2.Id,
                        Uri = master.Item2.Uri,
                        Name = master.Item2.Name
                    };
                }
                if (page.Item1.ParentId != null && pageDictionary.TryGetValue((int)page.Item1.ParentId, out parent))
                {
                    parent.Item2.Children.Add(page.Item2);
                }
                else
                {
                    pageList.Add(page.Item2);
                }
            }

            return pageList;
        }

        /// <summary>
        /// Gets a list of <see cref="PageItem" />s to be belonging to the specified <paramref name="portal" /> as an asynchronous operation.
        /// The last page in the list represents the specified page, and all other pages represent an ordered list of master pages.
        /// </summary>
        /// <param name="portal">The portal which owns the pages.</param>
        /// <param name="normalizedUri">The normalized uri to search for.</param>
        /// <param name="cancellationToken">A token to observe while waiting for the task to complete.</param>
        /// <returns>
        /// A task that represents the asynchronous operation.
        /// </returns>
        public virtual async Task<PageLayers> GetPageLayersByUriAsync(PortalItem portal, string normalizedUri, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            ThrowIfDisposed();
            if (portal == null)
            {
                throw new ArgumentNullException(nameof(portal));
            }

            var pageLayers = new List<PageItem>();
            await Context.Database.SqlQuery<PortalPage>(
                $"{DbSchema.Portal}.GetPageLayersByUri @PortalUri, @PageUris",
                DbParameters.UriValue("PortalUri", portal.Uri),
                DbParameters.UriTable("PageUris", normalizedUri?.Split('/')))
                .ForEachAsync(p => pageLayers.Add(Mapper.Map(p, new PageItem())), cancellationToken);

            if (pageLayers.Count == 0)
            {
                return PageLayers.Empty;
            }
            if (pageLayers.Count == 1)
            {
                return PageLayers.Create(pageLayers[0]);
            }
            var contentPageIx = pageLayers.Count - 1;
            return PageLayers.Create(pageLayers[contentPageIx], ImmutableArray.Create(pageLayers.ToArray(), 0, contentPageIx));
        }

        /// <summary>
        /// Gets the page identifier for the specified <paramref name="pageUri" />.
        /// </summary>
        /// <param name="portalUri">The portal URI which identifies the portal.</param>
        /// <param name="pageUri">The page URI which identifies the page.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken" /> used to propagate notifications that the operation should be canceled.</param>
        /// <returns>
        /// The task object representing the asynchronous operation.
        /// </returns>
        protected virtual Task<int?> GetPageIdByUriAsync(string portalUri, string pageUri, CancellationToken cancellationToken)
        {
            return Context.Database.SqlQuery<int?>(
                $"SELECT {DbSchema.Portal}.GetPageIdByUri(@PortalUri, @PageUris)",
                DbParameters.UriValue("PortalUri", portalUri),
                DbParameters.UriTable("PageUris", pageUri?.Split('/')))
                .FirstOrDefaultAsync(cancellationToken);
        }

        /// <summary>
        /// Gets the page URI for the specified <paramref name="id" />.
        /// </summary>
        /// <param name="id">The identifier which identifies the page.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken" /> used to propagate notifications that the operation should be canceled.</param>
        /// <returns>
        /// The task object representing the asynchronous operation.
        /// </returns>
        protected virtual Task<string> GetPageUriByIdAsync(int id, CancellationToken cancellationToken)
        {
            return Context.Database.SqlQuery<string>(
                $"SELECT {DbSchema.Portal}.GetPageUriById(@Id)",
                DbParameters.Value("Id", id))
                .FirstOrDefaultAsync(cancellationToken);
        }

        /// <summary>
        /// Gets a list of references for the specified <paramref name="page" />.
        /// </summary>
        /// <param name="portal">The portal which owns the page.</param>
        /// <param name="page">The page to be found.</param>
        /// <param name="cancellationToken">A token to observe while waiting for the task to complete.</param>
        /// <returns>
        /// A task that represents the asynchronous operation.
        /// </returns>
        public virtual async Task<ListResult<ReferenceItem>> GetPageReferencesAsync(PortalItem portal, PageItem page, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            ThrowIfDisposed();
            if (portal == null)
            {
                throw new ArgumentNullException(nameof(portal));
            }
            if (page == null)
            {
                throw new ArgumentNullException(nameof(page));
            }

            var pageEntity = await Pages.FindAsync(cancellationToken, page.Id);
            if (pageEntity == null)
            {
                throw new InvalidOperationException(string.Format(CultureInfo.CurrentCulture, PortalResources.PageNotFound, page.Id));
            }
            if (pageEntity.ReferenceList == null)
            {
                return ListResult<ReferenceItem>.Empty;
            }
            return ListResult.Create(ReferenceSerializer.Deserialize(pageEntity.ReferenceList));
        }

        /// <summary>
        /// Sets a list of references for the specified <paramref name="page" />.
        /// </summary>
        /// <param name="portal">The portal which owns the page.</param>
        /// <param name="page">The page to update.</param>
        /// <param name="references">The references to add.</param>
        /// <param name="cancellationToken">A token to observe while waiting for the task to complete.</param>
        /// <returns>
        /// A task that represents the asynchronous operation.
        /// </returns>
        public virtual async Task SetPageReferencesAsync(PortalItem portal, PageItem page, IEnumerable<ReferenceItem> references, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            ThrowIfDisposed();
            if (portal == null)
            {
                throw new ArgumentNullException(nameof(portal));
            }
            if (page == null)
            {
                throw new ArgumentNullException(nameof(page));
            }
            if (references == null)
            {
                throw new ArgumentNullException(nameof(references));
            }

            var pageEntity = await Pages.FindAsync(cancellationToken, page.Id);
            if (pageEntity == null)
            {
                throw new InvalidOperationException(string.Format(CultureInfo.CurrentCulture, PortalResources.PageNotFound, page.Id));
            }
            pageEntity.ReferenceList = (references.Any() ? ReferenceSerializer.Serialize(references) : null);
        }

        /// <summary>
        /// Adds a new media to the portal.
        /// </summary>
        /// <param name="portal">The portal which owns the page.</param>
        /// <param name="parentMedia">The parent media which owns the new media.</param>
        /// <param name="media">The media to add.</param>
        /// <param name="cancellationToken">A token to observe while waiting for the task to complete.</param>
        /// <returns>
        /// A task that represents the asynchronous operation.
        /// </returns>
        public virtual async Task AddMediaAsync(PortalItem portal, MediaItem parentMedia, MediaItem media, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            ThrowIfDisposed();
            if (portal == null)
            {
                throw new ArgumentNullException(nameof(portal));
            }
            if (media == null)
            {
                throw new ArgumentNullException(nameof(media));
            }

            var mediaEntity = Context.Add(new PortalMedia
            {
                PortalId = portal.Id,
                Uri = media.Uri,
                Type = media.Type,
                Name = media.Name
            });

            // It is required to get the unique ID generated by SQL Server.
            await Context.SaveChangesAsync(cancellationToken);

            media.Id = mediaEntity.Id;
            media.ModifiedDate = mediaEntity.ModifiedDate;
        }

        /// <summary>
        /// Removes a <see cref="MediaItem" /> from the specified <paramref name="portal" /> as an asynchronous operation.
        /// </summary>
        /// <param name="portal">The portal which owns the media.</param>
        /// <param name="media">The media to remove.</param>
        /// <param name="cancellationToken">A token to observe while waiting for the task to complete.</param>
        /// <returns>
        /// A task that represents the asynchronous operation.
        /// </returns>
        public virtual async Task RemoveMediaAsync(PortalItem portal, MediaItem media, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            ThrowIfDisposed();
            if (portal == null)
            {
                throw new ArgumentNullException(nameof(portal));
            }
            if (media == null)
            {
                throw new ArgumentNullException(nameof(media));
            }

            var mediaEntity = await Media.FindAsync(cancellationToken, media.Id);
            if (mediaEntity != null)
            {
                Context.Remove(mediaEntity);
            }
        }

        /// <summary>
        /// Finds and returns a <see cref="MediaItem" />, if any, which has the specified normalized uri.
        /// </summary>
        /// <param name="portal">The portal which owns the media.</param>
        /// <param name="normalizedUri">The normalized uri to search for.</param>
        /// <param name="fields">The related fields to include in the query results.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken" /> used to propagate notifications that the operation should be canceled.</param>
        /// <returns>
        /// The <see cref="Task" /> that represents the asynchronous operation, containing the page matching the specified <paramref name="normalizedUri" /> if it exists.
        /// </returns>
        public virtual async Task<MediaItem> GetMediaByUriAsync(PortalItem portal, string normalizedUri, MediaField fields, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            ThrowIfDisposed();
            if (portal == null)
            {
                throw new ArgumentNullException(nameof(portal));
            }
            if (normalizedUri == null)
            {
                throw new ArgumentNullException(nameof(normalizedUri));
            }
            var mediaId = await GetMediaIdByUriAsync(portal.Uri, normalizedUri, cancellationToken);
            if (mediaId == null)
            {
                return null;
            }
            return await Media
                .Where(m => m.Id == mediaId)
                .Select(fields)
                .SingleOrDefaultAsync(cancellationToken);
        }

        /// <summary>
        /// Gets a list of <see cref="MediaItem" />s to be belonging to the specified <paramref name="portal" /> as an asynchronous operation.
        /// </summary>
        /// <param name="portal">The portal which owns the pages.</param>
        /// <param name="parentMedia">The parent media which owns the media.</param>
        /// <param name="name">The name for the item to be found.</param>
        /// <param name="orderBy">The order in which items are returned in a result set.</param>
        /// <param name="fields">The related fields to include in the query results.</param>
        /// <param name="cancellationToken">A token to observe while waiting for the task to complete.</param>
        /// <returns>
        /// A task that represents the asynchronous operation.
        /// </returns>
        public virtual Task<ListResult<MediaItem>> GetMediaAsync(PortalItem portal, MediaItem parentMedia, string name, MediaSortOrder orderBy, MediaField fields, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            ThrowIfDisposed();
            if (portal == null)
            {
                throw new ArgumentNullException(nameof(portal));
            }
            return Media
                .Where(portal.Id, parentMedia?.Id, name)
                .OrderBy(orderBy)
                .Select(fields)
                .ToListResultAsync(cancellationToken);
        }

        /// <summary>
        /// Gets the media identifier for the specified <paramref name="mediaUri" />.
        /// </summary>
        /// <param name="portalUri">The portal URI which identifies the portal.</param>
        /// <param name="mediaUri">The media URI which identifies the media.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken" /> used to propagate notifications that the operation should be canceled.</param>
        /// <returns>
        /// The task object representing the asynchronous operation.
        /// </returns>
        protected virtual Task<int?> GetMediaIdByUriAsync(string portalUri, string mediaUri, CancellationToken cancellationToken)
        {
            return Context.Database.SqlQuery<int?>(
                $"SELECT {DbSchema.Portal}.GetMediaIdByUri(@PortalUri, @PageUris)",
                DbParameters.UriValue("PortalUri", portalUri),
                DbParameters.UriTable("MediaUris", mediaUri?.Split('/')))
                .FirstOrDefaultAsync(cancellationToken);
        }

        /// <summary>
        /// Gets the media URI for the specified <paramref name="id" />.
        /// </summary>
        /// <param name="id">The identifier which identifies the media.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken" /> used to propagate notifications that the operation should be canceled.</param>
        /// <returns>
        /// The task object representing the asynchronous operation.
        /// </returns>
        protected virtual Task<string> GetMediaUriByIdAsync(int id, CancellationToken cancellationToken)
        {
            return Context.Database.SqlQuery<string>(
                $"SELECT {DbSchema.Portal}.GetMediaUriById(@Id)",
                DbParameters.Value("Id", id))
                .FirstOrDefaultAsync(cancellationToken);
        }

        /// <summary>
        /// The <see cref="DbSet" /> for access to identity Access Control Entries  in the context.
        /// </summary>
        /// <value>
        /// The identity Access Control Entries.
        /// </value>
        private DbSet<SecurityAccessRule> AccessRules => Context.Set<SecurityAccessRule>();

        /// <summary>
        /// The <see cref="DbSet" /> for access to identity users in the context.
        /// </summary>
        /// <value>
        /// The identity users.
        /// </value>
        private DbSet<IdentityUser> Users => Context.Set<IdentityUser>();

        /// <summary>
        /// The <see cref="DbSet" /> for access to <see cref="ProjectEntity" />s in the context.
        /// </summary>
        /// <value>
        /// The <see cref="ProjectEntity" />s.
        /// </value>
        private DbSet<ProjectEntity> Projects => Context.Set<ProjectEntity>();

        /// <summary>
        /// The <see cref="DbSet" /> for access to <see cref="PortalEntity" />s in the context.
        /// </summary>
        /// <value>
        /// The <see cref="PortalEntity" />s.
        /// </value>
        private DbSet<PortalEntity> Portals => Context.Set<PortalEntity>();

        /// <summary>
        /// The <see cref="DbSet" /> for access to <see cref="PortalPage" />s in the context.
        /// </summary>
        /// <value>
        /// The <see cref="PortalPage" />s.
        /// </value>
        private DbSet<PortalPage> Pages => Context.Set<PortalPage>();

        /// <summary>
        /// The <see cref="DbSet" /> for access to <see cref="PortalMedia" />s in the context.
        /// </summary>
        /// <value>
        /// The <see cref="PortalMedia" />s.
        /// </value>
        private DbSet<PortalMedia> Media => Context.Set<PortalMedia>();

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
