// Copyright (c) János Janka. All rights reserved.

using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Partnerinfo.Security;

namespace Partnerinfo.Portal
{
    public class PortalManager : IDisposable
    {
        private bool _disposed;

        /// <summary>
        /// Initializes a new instance of the <see cref="PortalManager" /> class
        /// </summary>
        /// <param name="securityManager">The security manager that that <see cref="PortalManager" /> operates against.</param>
        /// <param name="store">The store that the <see cref="PortalManager" /> operates against.</param>
        /// <param name="validator">The validator that that <see cref="PortalManager" /> operates against.</param>
        /// <exception cref="System.ArgumentNullException">
        /// </exception>
        public PortalManager(
            SecurityManager securityManager,
            IPortalStore store,
            IPortalValidator validator)
        {
            if (securityManager == null)
            {
                throw new ArgumentNullException(nameof(securityManager));
            }
            if (store == null)
            {
                throw new ArgumentNullException(nameof(store));
            }
            SecurityManager = securityManager;
            Store = store;
            Validator = validator ?? PortalValidator.Default;
        }

        /// <summary>
        /// Gets or sets the URI normalizer that the <see cref="PortalManager" /> operates against.
        /// </summary>
        /// <value>
        /// The URI normalizer.
        /// </value>
        public IUriNormalizer UriNormalizer { get; set; } = DefaultUriNormalizer.Default;

        /// <summary>
        /// Gets or sets the <see cref="IMediaStreamStore" /> that the <see cref="PortalManager" /> operates against.
        /// </summary>
        /// <value>
        /// The <see cref="IMediaStreamStore" />.
        /// </value>
        public IMediaStreamStore MediaStreamStore { get; set; }

        /// <summary>
        /// Gets or sets the security manager that the <see cref="PortalManager" /> operates against.
        /// </summary>
        /// <value>
        /// The security manager.
        /// </value>
        protected SecurityManager SecurityManager { get; set; }

        /// <summary>
        /// Gets or sets the store that the <see cref="PortalManager" /> operates against.
        /// </summary>
        /// <value>
        /// The store.
        /// </value>
        protected IPortalStore Store { get; set; }

        /// <summary>
        /// Gets a list of <see cref="IPortalValidator" />s.
        /// </summary>
        /// <value>
        /// A list of <see cref="IPortalValidator" />s.
        /// </value>
        protected IPortalValidator Validator { get; set; }

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
            return Store.GetUriAsync(portal, cancellationToken);
        }

        /// <summary>
        /// Sets the <paramref name="uri" /> for a <paramref name="portal" />.
        /// </summary>
        /// <param name="portal">The portal whose name should be set.</param>
        /// <param name="uri">The URI to set.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken" /> used to propagate notifications that the operation should be canceled.</param>
        /// <returns>
        /// The task object representing the asynchronous operation.
        /// </returns>
        public virtual async Task<ValidationResult> SetUriAsync(PortalItem portal, string uri, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            ThrowIfDisposed();
            if (portal == null)
            {
                throw new ArgumentNullException(nameof(portal));
            }
            await Store.SetUriAsync(portal, NormalizeUri(uri), cancellationToken);
            return await UpdatePortalAsync(portal, cancellationToken);
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
            ThrowIfDisposed();
            if (portal == null)
            {
                throw new ArgumentNullException(nameof(portal));
            }
            return Store.GetHomePageAsync(portal, cancellationToken);
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
        public virtual async Task<ValidationResult> SetHomePageAsync(PortalItem portal, ResourceItem homePage, CancellationToken cancellationToken)
        {
            ThrowIfDisposed();
            if (portal == null)
            {
                throw new ArgumentNullException(nameof(portal));
            }
            if (homePage == null)
            {
                throw new ArgumentNullException(nameof(homePage));
            }
            await Store.SetHomePageAsync(portal, homePage, cancellationToken);
            return await UpdatePortalAsync(portal, cancellationToken);
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
            ThrowIfDisposed();
            if (portal == null)
            {
                throw new ArgumentNullException(nameof(portal));
            }
            return Store.GetMasterPageAsync(portal, cancellationToken);
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
        public virtual async Task<ValidationResult> SetMasterPageAsync(PortalItem portal, ResourceItem masterPage, CancellationToken cancellationToken)
        {
            ThrowIfDisposed();
            if (portal == null)
            {
                throw new ArgumentNullException(nameof(portal));
            }
            await Store.SetMasterPageAsync(portal, masterPage, cancellationToken);
            return await UpdatePortalAsync(portal, cancellationToken);
        }

        /// <summary>
        /// Creates the specified <paramref name="portal" /> in the portal store, as an asynchronous operation.
        /// </summary>
        /// <param name="portal">The portal to create.</param>
        /// <param name="acl">A list of <see cref="AccessRuleItem" />s to be specified on the portal.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken" /> used to propagate notifications that the operation should be canceled.</param>
        /// <returns>
        /// The <see cref="Task" /> that represents the asynchronous operation, containing the <see cref="ValidationResult" /> of the creation operation.
        /// </returns>
        public virtual async Task<ValidationResult> CreateAsync(PortalItem portal, IEnumerable<AccessRuleItem> acl, CancellationToken cancellationToken)
        {
            ThrowIfDisposed();
            if (portal == null)
            {
                throw new ArgumentNullException(nameof(portal));
            }
            if (acl == null)
            {
                throw new ArgumentNullException(nameof(acl));
            }
            portal.Uri = NormalizeUri(portal.Uri);
            var validationResult = await ValidatePortalAsync(portal, cancellationToken);
            if (!validationResult.Succeeded)
            {
                return validationResult;
            }
            validationResult = await Store.CreateAsync(portal, cancellationToken);
            if (!validationResult.Succeeded)
            {
                return validationResult;
            }
            foreach (var ace in acl)
            {
                await SecurityManager.SetAccessRuleAsync(portal, ace, cancellationToken);
            }
            return ValidationResult.Success;
        }

        /// <summary>
        /// Creates the specified <paramref name="portal" /> in the portal store, as an asynchronous operation.
        /// </summary>
        /// <param name="portal">The portal to create.</param>
        /// <param name="acl">A list of <see cref="AccessRuleItem" />s to be specified on the portal.</param>
        /// <param name="template">The name of the template.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken" /> used to propagate notifications that the operation should be canceled.</param>
        /// <returns>
        /// The <see cref="Task" /> that represents the asynchronous operation, containing the <see cref="ValidationResult" /> of the creation operation.
        /// </returns>
        public virtual async Task<ValidationResult> CreateAsync(PortalItem portal, IEnumerable<AccessRuleItem> acl, string template, CancellationToken cancellationToken)
        {
            var store = GetPageStore();
            var validationResult = await CreateAsync(portal, acl, cancellationToken);
            if (!validationResult.Succeeded)
            {
                return validationResult;
            }
            if (template != null)
            {
                var templatePath = ServerPaths.Map(ServerPaths.PortalTemplates, template);

                var layoutPage = new PageItem { Uri = "layout", Name = "_Layout", HtmlContent = File.ReadAllText(Path.Combine(templatePath, "layout.html")).Replace("{{title}}", portal.Name) };
                var homePage = new PageItem { Uri = "home", Name = "Home", HtmlContent = File.ReadAllText(Path.Combine(templatePath, "home.html")).Replace("{{title}}", portal.Name) };
                var confirmPage = new PageItem { Uri = "confirm", Name = "Confirm", HtmlContent = File.ReadAllText(Path.Combine(templatePath, "confirm.html")).Replace("{{title}}", portal.Name) };

                await store.AddPageAsync(portal, null, layoutPage, cancellationToken);
                await store.AddPageAsync(portal, null, homePage, cancellationToken);
                await store.AddPageAsync(portal, null, confirmPage, cancellationToken);

                await store.SetMasterPageAsync(portal, layoutPage, cancellationToken);
                await store.SetHomePageAsync(portal, homePage, cancellationToken);

                return await UpdatePortalAsync(portal, cancellationToken);
            }
            return validationResult;
        }

        /// <summary>
        /// Updates the specified <paramref name="portal" /> in the portal store, as an asynchronous operation.
        /// </summary>
        /// <param name="portal">The portal to update.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken" /> used to propagate notifications that the operation should be canceled.</param>
        /// <returns>
        /// The <see cref="Task" /> that represents the asynchronous operation, containing the <see cref="ValidationResult" /> of the update operation.
        /// </returns>
        public virtual Task<ValidationResult> UpdateAsync(PortalItem portal, CancellationToken cancellationToken)
        {
            ThrowIfDisposed();
            if (portal == null)
            {
                throw new ArgumentNullException(nameof(portal));
            }
            return UpdatePortalAsync(portal, cancellationToken);
        }

        /// <summary>
        /// Copies an existing portal to a new portal.
        /// </summary>
        /// <param name="portal">The portal to copy.</param>
        /// <param name="uri">The uri for the new portal.</param>
        /// <param name="cancellationToken">A token to observe while waiting for the task to complete.</param>
        /// <returns>
        /// A task that represents the asynchronous operation.
        /// </returns>
        public virtual Task<ValidationResult> CopyAsync(PortalItem portal, string uri, CancellationToken cancellationToken)
        {
            ThrowIfDisposed();
            if (portal == null)
            {
                throw new ArgumentNullException(nameof(portal));
            }
            return Store.CopyAsync(portal, NormalizeUri(uri), cancellationToken);
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
            ThrowIfDisposed();
            if (portal == null)
            {
                throw new ArgumentNullException(nameof(portal));
            }
            var validationResult = await Store.DeleteAsync(portal, cancellationToken);
            if (validationResult.Succeeded)
            {
                await SecurityManager.RemoveAccessRulesAsync(portal, cancellationToken);
            }
            return validationResult;
        }

        /// <summary>
        /// Finds and returns a portal, if any, which has the specified normalized uri.
        /// </summary>
        /// <param name="normalizedUri">The normalized uri to search for.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken" /> used to propagate notifications that the operation should be canceled.</param>
        /// <returns>
        /// The <see cref="Task" /> that represents the asynchronous operation, containing the portal matching the specified <paramref name="normalizedUri" /> if it exists.
        /// </returns>
        public virtual Task<PortalItem> FindByUriAsync(string normalizedUri, CancellationToken cancellationToken)
        {
            ThrowIfDisposed();
            return Store.FindByUriAsync(normalizedUri, PortalField.None, cancellationToken);
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
            ThrowIfDisposed();
            return Store.FindByUriAsync(normalizedUri, fields, cancellationToken);
        }

        /// <summary>
        /// Finds and returns a portal, if any, which has the specified normalized domain.
        /// </summary>
        /// <param name="normalizedDomain">The normalized domain to search for.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken" /> used to propagate notifications that the operation should be canceled.</param>
        /// <returns>
        /// The <see cref="Task" /> that represents the asynchronous operation, containing the portal matching the specified <paramref name="normalizedDomain" /> if it exists.
        /// </returns>
        public virtual Task<PortalItem> FindByDomainAsync(string normalizedDomain, CancellationToken cancellationToken)
        {
            ThrowIfDisposed();
            var store = GetDomainStore();
            return store.FindByDomainAsync(normalizedDomain, PortalField.None, cancellationToken);
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
            ThrowIfDisposed();
            var store = GetDomainStore();
            return store.FindByDomainAsync(normalizedDomain, fields, cancellationToken);
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
        public virtual Task<ListResult<PortalItem>> FindAllAsync(int userId, string name, PortalSortOrder orderBy, int pageIndex, int pageSize, PortalField fields, CancellationToken cancellationToken)
        {
            ThrowIfDisposed();
            return Store.FindAllAsync(userId, name, orderBy, pageIndex, pageSize, fields, cancellationToken);
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
        public virtual async Task<ValidationResult> SetProjectAsync(PortalItem portal, UniqueItem project, CancellationToken cancellationToken)
        {
            ThrowIfDisposed();
            if (portal == null)
            {
                throw new ArgumentNullException(nameof(portal));
            }
            await Store.SetProjectAsync(portal, project, cancellationToken);
            return await UpdatePortalAsync(portal, cancellationToken);
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
            ThrowIfDisposed();
            if (portal == null)
            {
                throw new ArgumentNullException(nameof(portal));
            }
            return Store.GetProjectAsync(portal, cancellationToken);
        }

        /// <summary>
        /// Gets the URI for the specified <paramref name="page" />.
        /// </summary>
        /// <param name="portal">The portal which owns the page.</param>
        /// <param name="page">The page whose URI should be returned.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken" /> used to propagate notifications that the operation should be canceled.</param>
        /// <returns>
        /// The task object representing the asynchronous operation.
        /// </returns>
        public virtual Task<string> GetPageUriAsync(PortalItem portal, PageItem page, CancellationToken cancellationToken)
        {
            ThrowIfDisposed();
            var store = GetPageStore();
            if (portal == null)
            {
                throw new ArgumentNullException(nameof(portal));
            }
            if (page == null)
            {
                throw new ArgumentNullException(nameof(page));
            }
            return store.GetPageUriAsync(portal, page, cancellationToken);
        }

        /// <summary>
        /// Sets the <paramref name="uri" /> for a <paramref name="page" />.
        /// </summary>
        /// <param name="portal">The portal which owns the page.</param>
        /// <param name="page">The page whose URI should be set.</param>
        /// <param name="uri">The URI to set.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken" /> used to propagate notifications that the operation should be canceled.</param>
        /// <returns>
        /// The task object representing the asynchronous operation.
        /// </returns>
        public virtual async Task<ValidationResult> SetPageUriAsync(PortalItem portal, PageItem page, string uri, CancellationToken cancellationToken)
        {
            ThrowIfDisposed();
            var store = GetPageStore();
            if (portal == null)
            {
                throw new ArgumentNullException(nameof(portal));
            }
            if (page == null)
            {
                throw new ArgumentNullException(nameof(page));
            }
            await store.SetPageUriAsync(portal, page, NormalizeUri(uri), cancellationToken);
            return await UpdatePortalAsync(portal, cancellationToken);
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
            ThrowIfDisposed();
            var store = GetPageStore();
            if (portal == null)
            {
                throw new ArgumentNullException(nameof(portal));
            }
            if (page == null)
            {
                throw new ArgumentNullException(nameof(page));
            }
            return store.GetPageMasterAsync(portal, page, cancellationToken);
        }

        /// <summary>
        /// Sets the <paramref name="masterPage" /> for a <paramref name="page" />.
        /// </summary>
        /// <param name="portal">The portal whose master page should be set.</param>
        /// <param name="page">The page whose master page should be set.</param>
        /// <param name="masterPageUri">The master page to set.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken" /> used to propagate notifications that the operation should be canceled.</param>
        /// <returns>
        /// The task object representing the asynchronous operation.
        /// </returns>
        public virtual async Task<ValidationResult> SetPageMasterAsync(PortalItem portal, PageItem page, string masterPageUri, CancellationToken cancellationToken)
        {
            ThrowIfDisposed();
            var store = GetPageStore();
            if (portal == null)
            {
                throw new ArgumentNullException(nameof(portal));
            }
            if (page == null)
            {
                throw new ArgumentNullException(nameof(page));
            }
            if (masterPageUri == null)
            {
                await store.SetPageMasterAsync(portal, page, null, cancellationToken);
            }
            else
            {
                var masterPage = await GetPageByUriAsync(portal, masterPageUri, cancellationToken);
                if (masterPage == null)
                {
                    return ValidationResult.Failed(
                        string.Format(CultureInfo.CurrentCulture, PortalResources.PageNotFound, string.Join("/", portal.Uri, masterPageUri)));
                }
                await store.SetPageMasterAsync(portal, page, masterPage, cancellationToken);
            }
            return await UpdatePortalAsync(portal, cancellationToken);
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
        public virtual async Task<ValidationResult> SetPageMasterAsync(PortalItem portal, PageItem page, PageItem masterPage, CancellationToken cancellationToken)
        {
            ThrowIfDisposed();
            var store = GetPageStore();
            if (portal == null)
            {
                throw new ArgumentNullException(nameof(portal));
            }
            if (page == null)
            {
                throw new ArgumentNullException(nameof(page));
            }
            await store.SetPageMasterAsync(portal, page, masterPage, cancellationToken);
            return await UpdatePortalAsync(portal, cancellationToken);
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
        public virtual async Task<ValidationResult> AddPageAsync(PortalItem portal, PageItem parentPage, PageItem page, CancellationToken cancellationToken)
        {
            ThrowIfDisposed();
            var store = GetPageStore();
            if (portal == null)
            {
                throw new ArgumentNullException(nameof(portal));
            }
            if (page == null)
            {
                throw new ArgumentNullException(nameof(page));
            }
            page.Uri = NormalizeUri(page.Uri);
            var result = await ValidatePageAsync(portal, page, cancellationToken);
            if (!result.Succeeded)
            {
                return result;
            }
            await store.AddPageAsync(portal, parentPage, page, cancellationToken);
            return await UpdatePortalAsync(portal, cancellationToken);
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
        public virtual async Task<ValidationResult> ReplacePageAsync(PortalItem portal, PageItem page, PageItem newPage, CancellationToken cancellationToken)
        {
            ThrowIfDisposed();
            var store = GetPageStore();
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
            newPage.Uri = NormalizeUri(newPage.Uri);
            var result = await ValidatePageAsync(portal, page, cancellationToken);
            if (!result.Succeeded)
            {
                return result;
            }
            await store.ReplacePageAsync(portal, page, newPage, cancellationToken);
            return await UpdatePortalAsync(portal, cancellationToken);
        }

        /// <summary>
        /// Copies an existing page to a new page.
        /// </summary>
        /// <param name="portal">The portal which owns the page.</param>
        /// <param name="page">The page to copy.</param>
        /// <param name="uri">The URI to set.</param>
        /// <param name="cancellationToken">A token to observe while waiting for the task to complete.</param>
        /// <returns>
        /// A task that represents the asynchronous operation.
        /// </returns>
        public virtual async Task<ValidationResult> CopyPageAsync(PortalItem portal, PageItem page, string uri, CancellationToken cancellationToken)
        {
            ThrowIfDisposed();
            var store = GetPageStore();
            if (portal == null)
            {
                throw new ArgumentNullException(nameof(portal));
            }
            if (page == null)
            {
                throw new ArgumentNullException(nameof(page));
            }
            if (uri == null)
            {
                throw new ArgumentNullException(nameof(uri));
            }
            await store.CopyPageAsync(portal, page, NormalizeUri(uri), cancellationToken);
            return await UpdatePortalAsync(portal, cancellationToken);
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
        public virtual async Task<ValidationResult> MovePageAsync(PortalItem portal, PageItem page, PageItem referencePage, CancellationToken cancellationToken)
        {
            ThrowIfDisposed();
            var store = GetPageStore();
            if (portal == null)
            {
                throw new ArgumentNullException(nameof(portal));
            }
            if (page == null)
            {
                throw new ArgumentNullException(nameof(page));
            }
            await store.MovePageAsync(portal, page, referencePage, cancellationToken);
            return await UpdatePortalAsync(portal, cancellationToken);
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
        public virtual async Task<ValidationResult> RemovePageAsync(PortalItem portal, PageItem page, CancellationToken cancellationToken)
        {
            ThrowIfDisposed();
            var store = GetPageStore();
            if (portal == null)
            {
                throw new ArgumentNullException(nameof(portal));
            }
            if (page == null)
            {
                throw new ArgumentNullException(nameof(page));
            }
            await store.RemovePageAsync(portal, page, cancellationToken);
            return await UpdatePortalAsync(portal, cancellationToken);
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
        public virtual Task<PageItem> GetPageByIdAsync(int id, CancellationToken cancellationToken)
        {
            return GetPageByIdAsync(id, PageField.None, cancellationToken);
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
        public virtual Task<PageItem> GetPageByIdAsync(int id, PageField fields, CancellationToken cancellationToken)
        {
            ThrowIfDisposed();
            var store = GetPageStore();
            return store.GetPageByIdAsync(id, fields, cancellationToken);
        }

        /// <summary>
        /// Finds and returns a page, if any, which has the specified normalized uri.
        /// </summary>
        /// <param name="portal">The portal which owns the page.</param>
        /// <param name="normalizedUri">The normalized uri to search for.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken" /> used to propagate notifications that the operation should be canceled.</param>
        /// <returns>
        /// The <see cref="Task" /> that represents the asynchronous operation, containing the page matching the specified <paramref name="normalizedUri" /> if it exists.
        /// </returns>
        public virtual Task<PageItem> GetPageByUriAsync(PortalItem portal, string normalizedUri, CancellationToken cancellationToken)
        {
            return GetPageByUriAsync(portal, normalizedUri, PageField.None, cancellationToken);
        }

        /// <summary>
        /// Finds and returns a page, if any, which has the specified normalized uri.
        /// </summary>
        /// <param name="portal">The portal which owns the page.</param>
        /// <param name="normalizedUri">The normalized uri to search for.</param>
        /// <param name="fields">The related fields to include in the query results.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken" /> used to propagate notifications that the operation should be canceled.</param>
        /// <returns>
        /// The <see cref="Task" /> that represents the asynchronous operation, containing the page matching the specified <paramref name="normalizedUri" /> if it exists.
        /// </returns>
        public virtual Task<PageItem> GetPageByUriAsync(PortalItem portal, string normalizedUri, PageField fields, CancellationToken cancellationToken)
        {
            ThrowIfDisposed();
            var store = GetPageStore();
            if (portal == null)
            {
                throw new ArgumentNullException(nameof(portal));
            }
            return store.GetPageByUriAsync(portal, normalizedUri, fields, cancellationToken);
        }

        /// <summary>
        /// Gets a list of <see cref="PageItem" />s to be belonging to the specified <paramref name="portal" /> as an asynchronous operation.
        /// </summary>
        /// <param name="portal">The portal which owns the pages.</param>
        /// <param name="cancellationToken">A token to observe while waiting for the task to complete.</param>
        /// <returns>
        /// A task that represents the asynchronous operation.
        /// </returns>
        public virtual Task<IList<PageItem>> GetPagesAsync(PortalItem portal, CancellationToken cancellationToken)
        {
            ThrowIfDisposed();
            var store = GetPageStore();
            if (portal == null)
            {
                throw new ArgumentNullException(nameof(portal));
            }
            return store.GetPagesAsync(portal, cancellationToken);
        }

        /// <summary>
        /// Gets a list of <see cref="PageItem" />s to be belonging to the specified <paramref name="portal" /> as an asynchronous operation.
        /// The first page in the list represents the specified page, and all other pages represent an ordered list of master pages.
        /// </summary>
        /// <param name="portal">The portal which owns the pages.</param>
        /// <param name="normalizedUri">The normalized uri to search for.</param>
        /// <param name="cancellationToken">A token to observe while waiting for the task to complete.</param>
        /// <returns>
        /// A task that represents the asynchronous operation.
        /// </returns>
        public virtual Task<PageLayers> GetPageLayersByUriAsync(PortalItem portal, string normalizedUri, CancellationToken cancellationToken)
        {
            ThrowIfDisposed();
            var store = GetPageStore();
            if (portal == null)
            {
                throw new ArgumentNullException(nameof(portal));
            }
            return store.GetPageLayersByUriAsync(portal, normalizedUri, cancellationToken);
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
        public virtual Task<ListResult<ReferenceItem>> GetPageReferencesAsync(PortalItem portal, PageItem page, CancellationToken cancellationToken)
        {
            ThrowIfDisposed();
            var store = GetPageReferenceStore();
            if (portal == null)
            {
                throw new ArgumentNullException(nameof(portal));
            }
            if (page == null)
            {
                throw new ArgumentNullException(nameof(page));
            }
            return store.GetPageReferencesAsync(portal, page, cancellationToken);
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
        public virtual async Task<ValidationResult> SetPageReferencesAsync(PortalItem portal, PageItem page, IEnumerable<ReferenceItem> references, CancellationToken cancellationToken)
        {
            ThrowIfDisposed();
            var store = GetPageReferenceStore();
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

            await store.SetPageReferencesAsync(portal, page, references, cancellationToken);
            return await UpdatePortalAsync(portal, cancellationToken);
        }

        /// <summary>
        /// Adds a new media to the portal.
        /// </summary>
        /// <param name="portal">The portal which owns the page.</param>
        /// <param name="parentMedia">The parent media which owns the new media.</param>
        /// <param name="media">The media to add.</param>
        /// <param name="mediaStream">The media stream to save.</param>
        /// <param name="cancellationToken">A token to observe while waiting for the task to complete.</param>
        /// <returns>
        /// A task that represents the asynchronous operation.
        /// </returns>
        /// <exception cref="System.ArgumentNullException">
        /// </exception>
        public virtual async Task<ValidationResult> AddMediaAsync(PortalItem portal, MediaItem parentMedia, MediaItem media, Stream mediaStream, CancellationToken cancellationToken)
        {
            ThrowIfDisposed();
            var mediaStore = GetMediaStore();
            if (portal == null)
            {
                throw new ArgumentNullException(nameof(portal));
            }
            if (media == null)
            {
                throw new ArgumentNullException(nameof(media));
            }

            media.Uri = NormalizeUri(media.Uri);

            if (mediaStream != null)
            {
                await MediaStreamStore.SaveMediaStreamAsync(portal, media, mediaStream, cancellationToken);
            }

            await mediaStore.AddMediaAsync(portal, parentMedia, media, cancellationToken);
            return await UpdatePortalAsync(portal, cancellationToken);
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
        public virtual async Task<ValidationResult> RemoveMediaAsync(PortalItem portal, MediaItem media, CancellationToken cancellationToken)
        {
            ThrowIfDisposed();
            var mediaStore = GetMediaStore();
            if (portal == null)
            {
                throw new ArgumentNullException(nameof(portal));
            }
            if (media == null)
            {
                throw new ArgumentNullException(nameof(media));
            }

            await mediaStore.RemoveMediaAsync(portal, media, cancellationToken);
            return await UpdatePortalAsync(portal, cancellationToken);
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
        public virtual Task<MediaItem> GetMediaByUriAsync(PortalItem portal, string normalizedUri, CancellationToken cancellationToken)
        {
            return GetMediaByUriAsync(portal, normalizedUri, MediaField.None, cancellationToken);
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
        public virtual Task<MediaItem> GetMediaByUriAsync(PortalItem portal, string normalizedUri, MediaField fields, CancellationToken cancellationToken)
        {
            ThrowIfDisposed();
            var mediaStore = GetMediaStore();
            if (portal == null)
            {
                throw new ArgumentNullException(nameof(portal));
            }
            return mediaStore.GetMediaByUriAsync(portal, normalizedUri, fields, cancellationToken);
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
            ThrowIfDisposed();
            var mediaStore = GetMediaStore();
            if (portal == null)
            {
                throw new ArgumentNullException(nameof(portal));
            }
            return mediaStore.GetMediaAsync(portal, parentMedia, name, orderBy, fields, cancellationToken);
        }

        /// <summary>
        /// Gets the absolute URL for a media by its <paramref name="uri" />.
        /// </summary>
        /// <param name="portalUri">The portal URI.</param>
        /// <param name="mediaUri">The media URI.</param>
        /// <returns>
        /// The absolute URL for a media.
        /// </returns>
        public virtual string GetMediaLinkByUri(string portalUri, string mediaUri)
        {
            ThrowIfDisposed();
            return MediaStreamStore.GetMediaLinkByUri(portalUri, mediaUri);
        }

        /// <summary>
        /// Normalizes a uri for consistent comparisons.
        /// </summary>
        /// <param name="uri">The uri to normalize.</param>
        /// <returns>
        /// A normalized value representing the specified <paramref name="uri" />.
        /// </returns>
        public virtual string NormalizeUri(string uri)
        {
            uri = uri?.Split('/')?.LastOrDefault();

            return (UriNormalizer == null) ? uri : UriNormalizer.Normalize(uri);
        }

        /// <summary>
        /// Validates portal. Called by other <see cref="PortalManager" /> methods.
        /// </summary>
        /// <param name="portal">The portal to validate.</param>
        /// <param name="cancellationToken">A token to observe while waiting for the task to complete.</param>
        /// <returns>
        /// A task that represents the asynchronous operation.
        /// </returns>
        private Task<ValidationResult> ValidatePortalAsync(PortalItem portal, CancellationToken cancellationToken)
        {
            return (Validator == null) ? Task.FromResult(ValidationResult.Success) : Validator.ValidateAsync(this, portal, cancellationToken);
        }

        /// <summary>
        /// Validates project. Called by other PortalManager methods.
        /// </summary>
        private Task<ValidationResult> ValidatePageAsync(PortalItem portal, PageItem page, CancellationToken cancellationToken)
        {
            var validator = Validator as IPageValidator;
            return (validator == null) ? Task.FromResult(ValidationResult.Success) : validator.ValidatePageAsync(this, portal, page, cancellationToken);
        }

        /// <summary>
        /// Validates project and update. Called by other PortalManager methods.
        /// </summary>
        private async Task<ValidationResult> UpdatePortalAsync(PortalItem portal, CancellationToken cancellationToken)
        {
            portal.Uri = NormalizeUri(portal.Uri);
            var result = await ValidatePortalAsync(portal, cancellationToken);
            if (!result.Succeeded)
            {
                return result;
            }
            return await Store.UpdateAsync(portal, cancellationToken);
        }

        /// <summary>
        /// Ensures that the Store implements the <see cref="IPortalDomainStore" /> interface.
        /// </summary>
        /// <returns>
        /// The <see cref="IPortalDomainStore" /> object.
        /// </returns>
        /// <exception cref="System.NotSupportedException"></exception>
        internal IPortalDomainStore GetDomainStore()
        {
            var store = Store as IPortalDomainStore;
            if (store == null)
            {
                throw new NotSupportedException();
            }
            return store;
        }

        /// <summary>
        /// Ensures that the Store implements the <see cref="IPortalPageStore" /> interface.
        /// </summary>
        /// <returns>
        /// The <see cref="IPortalPageStore" /> object.
        /// </returns>
        /// <exception cref="System.NotSupportedException"></exception>
        internal IPortalPageStore GetPageStore()
        {
            var store = Store as IPortalPageStore;
            if (store == null)
            {
                throw new NotSupportedException();
            }
            return store;
        }

        /// <summary>
        /// Ensures that the Store implements the <see cref="IPortalPageReferenceStore" /> interface.
        /// </summary>
        /// <returns>
        /// The <see cref="IPortalPageReferenceStore" /> object.
        /// </returns>
        /// <exception cref="System.NotSupportedException"></exception>
        internal IPortalPageReferenceStore GetPageReferenceStore()
        {
            var store = Store as IPortalPageReferenceStore;
            if (store == null)
            {
                throw new NotSupportedException();
            }
            return store;
        }

        /// <summary>
        /// Ensures that the Store implements the <see cref="IPortalMediaStore" /> interface.
        /// </summary>
        /// <returns>
        /// The <see cref="IPortalMediaStore" /> object.
        /// </returns>
        /// <exception cref="System.NotSupportedException"></exception>
        internal IPortalMediaStore GetMediaStore()
        {
            var store = Store as IPortalMediaStore;
            if (store == null)
            {
                throw new NotSupportedException();
            }
            return store;
        }

        /// <summary>
        /// Throws a <see cref="ObjectDisposedException" /> if the context has already been disposed
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
