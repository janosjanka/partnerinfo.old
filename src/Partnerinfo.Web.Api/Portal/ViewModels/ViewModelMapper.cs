// Copyright (c) Partnerinfo Ltd. All rights reserved.

using System.Collections.Generic;
using System.Linq;
using Partnerinfo.Portal.ViewModels.Pages;
using Partnerinfo.Portal.ViewModels.Portals;

namespace Partnerinfo.Portal.ViewModels
{
    /// <summary>
    /// Provides facilities for mapping and working with view model objects.
    /// </summary>
    public static class ViewModelMapper
    {
        /// <summary>
        /// Maps an action to a result model object.
        /// </summary>
        /// <param name="portal">The action to map.</param>
        /// <returns>
        /// The mapped action.
        /// </returns>
        public static ListResult<PortalViewModel> ToPortalViewModel(ListResult<PortalItem> portal)
        {
            return new ListResult<PortalViewModel>(portal.Data.Select(p => ToPortalViewModel(p)), portal.Total);
        }

        /// <summary>
        /// Maps an action to a result model object.
        /// </summary>
        /// <param name="portal">The action to map.</param>
        /// <returns>
        /// The mapped action.
        /// </returns>
        public static PortalViewModel ToPortalViewModel(PortalItem portal, IList<PageItem> portalPages = null)
        {
            return portal == null ? null : new PortalViewModel
            {
                Id = portal.Id,
                Uri = portal.Uri,
                Name = portal.Name,
                Description = portal.Description,
                GATrackingId = portal.GATrackingId,
                CreatedDate = portal.CreatedDate,
                ModifiedDate = portal.ModifiedDate,
                Owners = portal.Owners,
                Project = ToUniqueItem(portal.Project),
                HomePage = ToResourceItem(portal.HomePage),
                MasterPage = ToResourceItem(portal.MasterPage),
                Pages = ToPageListViewModel(portalPages)
            };
        }

        /// <summary>
        /// Maps an action to a result model object.
        /// </summary>
        /// <param name="portal">The action to map.</param>
        /// <returns>
        /// The mapped action.
        /// </returns>
        public static PageViewModel ToPageViewModel(PageItem page)
        {
            return page == null ? null : new PageViewModel
            {
                Id = page.Id,
                Uri = page.Uri,
                Name = page.Name,
                Description = page.Description,
                HtmlContent = page.HtmlContent,
                StyleContent = page.StyleContent,
                ModifiedDate = page.ModifiedDate,
                Portal = ToResourceItem(page.Portal),
                Master = ToResourceItem(page.Master),
                Children = ToPageListViewModel(page.Children.OfType<PageItem>())
            };
        }

        /// <summary>
        /// Maps an action to a result model object.
        /// </summary>
        /// <param name="portal">The action to map.</param>
        /// <returns>
        /// The mapped action.
        /// </returns>
        public static ICollection<PageViewModel> ToPageListViewModel(IEnumerable<PageItem> list)
        {
            return list == null ? new List<PageViewModel>() : list.Select(m => new PageViewModel
            {
                Id = m.Id,
                Uri = m.Uri,
                Name = m.Name,
                Description = m.Description,
                HtmlContent = m.HtmlContent,
                StyleContent = m.StyleContent,
                ModifiedDate = m.ModifiedDate,
                Master = ToResourceItem(m.Master),
                Children = ToPageListViewModel(m.Children.OfType<PageItem>())
            })
            .ToList();
        }

        /// <summary>
        /// Maps a <paramref name="portal" /> to a <see cref="PageLayersViewModel" /> object.
        /// </summary>
        /// <param name="portal">The model to map.</param>
        /// <returns>
        /// The viewModel object.
        /// </returns>
        public static PageLayersViewModel ToPageLayersViewModel(PortalItem portal, PortalCompilerResult portalCompilerResult)
        {
            return portal == null || portalCompilerResult == null ? null : new PageLayersViewModel
            {
                Portal = ToPortalViewModel(portal),
                MasterPage = ToPageViewModel(portalCompilerResult.MasterPage),
                ContentPage = ToPageViewModel(portalCompilerResult.ContentPage)
            };
        }

        /// <summary>
        /// Maps a <paramref name="resource" /> to a <see cref="ResourceItem" /> object.
        /// </summary>
        /// <param name="resource">The resource to map.</param>
        /// <returns>
        /// The <see cref="ResourceItem" />.
        /// </returns>
        public static ResourceItem ToResourceItem(ResourceItem resource)
        {
            return resource == null ? null : new ResourceItem { Id = resource.Id, Uri = resource.Uri, Name = resource.Name };
        }

        /// <summary>
        /// Maps a <paramref name="uniqueItem" /> to a <see cref="UniqueItem" /> object.
        /// </summary>
        /// <param name="uniqueItem">The unique item to map.</param>
        /// <returns>
        /// The <see cref="UniqueItem" />.
        /// </returns>
        public static UniqueItem ToUniqueItem(UniqueItem uniqueItem)
        {
            return uniqueItem == null ? null : new UniqueItem { Id = uniqueItem.Id, Name = uniqueItem.Name };
        }
    }
}