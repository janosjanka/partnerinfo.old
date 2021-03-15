// Copyright (c) János Janka. All rights reserved.

using System;
using System.Collections.Generic;
using System.Linq;

namespace Partnerinfo.Portal.Models
{
    /// <summary>
    /// Provides facilities for mapping and working with view model objects.
    /// </summary>
    public static class ModelMapper
    {
        /// <summary>
        /// Maps an action to a result model object.
        /// </summary>
        /// <param name="portal">The action to map.</param>
        /// <returns>
        /// The mapped action.
        /// </returns>
        public static ListResult<PortalItemDto> ToPortalListDto(ListResult<PortalItem> portal)
        {
            return ListResult.Create(portal.Data.Select(p => ToPortalDto(p)), portal.Total);
        }

        /// <summary>
        /// Maps an action to a result model object.
        /// </summary>
        /// <param name="portal">The action to map.</param>
        /// <returns>
        /// The mapped action.
        /// </returns>
        public static PortalItemDto ToPortalDto(PortalItem portal, IList<PageItem> portalPages = null)
        {
            return portal == null ? null : new PortalItemDto
            {
                Id = portal.Id,
                Uri = portal.Uri,
                Domain = portal.Domain,
                Name = portal.Name,
                Description = portal.Description,
                GATrackingId = portal.GATrackingId,
                CreatedDate = portal.CreatedDate,
                ModifiedDate = portal.ModifiedDate,
                Owners = portal.Owners,
                Project = ToUniqueItem(portal.Project),
                HomePage = ToResourceItem(portal.HomePage),
                MasterPage = ToResourceItem(portal.MasterPage),
                Pages = ToPageListDto(portalPages)
            };
        }

        /// <summary>
        /// Maps an action to a result model object.
        /// </summary>
        /// <param name="portal">The action to map.</param>
        /// <returns>
        /// The mapped action.
        /// </returns>
        public static PageItemDto ToPageDto(PageItem page)
        {
            return page == null ? null : new PageItemDto
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
                References = ToReferenceListDto(page.References),
                Children = ToPageListDto(page.Children.OfType<PageItem>())
            };
        }

        /// <summary>
        /// Maps an action to a result model object.
        /// </summary>
        /// <param name="portal">The action to map.</param>
        /// <returns>
        /// The mapped action.
        /// </returns>
        public static ICollection<PageItemDto> ToPageListDto(IEnumerable<PageItem> list)
        {
            return list == null ? new List<PageItemDto>() : list.Select(m => new PageItemDto
            {
                Id = m.Id,
                Uri = m.Uri,
                Name = m.Name,
                Description = m.Description,
                HtmlContent = m.HtmlContent,
                StyleContent = m.StyleContent,
                ModifiedDate = m.ModifiedDate,
                Master = ToResourceItem(m.Master),
                Children = ToPageListDto(m.Children.OfType<PageItem>())
            })
            .ToList();
        }

        /// <summary>
        /// Maps a <paramref name="portal" /> to a <see cref="PageLayersDto" /> object.
        /// </summary>
        /// <param name="portal">The model to map.</param>
        /// <returns>
        /// The viewModel object.
        /// </returns>
        public static PageLayersDto ToPageLayersDto(PortalItem portal, PortalCompilerResult portalCompilerResult)
        {
            return portal == null || portalCompilerResult == null ? null : new PageLayersDto
            {
                Portal = ToPortalDto(portal),
                MasterPage = ToPageDto(portalCompilerResult.MasterPage),
                ContentPage = ToPageDto(portalCompilerResult.ContentPage)
            };
        }

        /// <summary>
        /// Maps an <see cref="ReferenceItem" /> to a DTO object.
        /// </summary>
        /// <param name="reference">The model to map.</param>
        /// <returns>
        /// The mapped model.
        /// </returns>
        public static ReferenceItemDto ToReferenceDto(ReferenceItem reference)
        {
            return reference == null ? null : new ReferenceItemDto
            {
                Type = reference.Type,
                Uri = reference.Uri
            };
        }

        /// <summary>
        /// Maps an <see cref="ReferenceItem" /> to a DTO object.
        /// </summary>
        /// <param name="referenceList">The model to map.</param>
        /// <returns>
        /// The mapped model.
        /// </returns>
        public static ICollection<ReferenceItemDto> ToReferenceListDto(IEnumerable<ReferenceItem> referenceList)
        {
            return referenceList.Select(r => ToReferenceDto(r)).ToArray();
        }

        /// <summary>
        /// Maps an <see cref="ReferenceItem" /> to a DTO object.
        /// </summary>
        /// <param name="referenceList">The model to map.</param>
        /// <returns>
        /// The mapped model.
        /// </returns>
        public static ListResult<ReferenceItemDto> ToReferenceListDto(ListResult<ReferenceItem> referenceList)
        {
            return ListResult.Create(referenceList.Data.Select(r => ToReferenceDto(r)), referenceList.Total);
        }

        /// <summary>
        /// Maps a <paramref name="media" /> to a <see cref="MediaItemDto" /> object.
        /// </summary>
        /// <param name="media">The model to map.</param>
        /// <returns>
        /// The viewModel object.
        /// </returns>
        public static ListResult<MediaItemDto> ToMediaListDto(ListResult<MediaItem> media, Func<string, string> linkGenerator)
        {
            return ListResult.Create(media.Data.Select(m => ToMediaDto(m, linkGenerator)), media.Total);
        }

        /// <summary>
        /// Maps a <paramref name="media" /> to a <see cref="MediaItemDto" /> object.
        /// </summary>
        /// <param name="media">The model to map.</param>
        /// <returns>
        /// The viewModel object.
        /// </returns>
        public static MediaItemDto ToMediaDto(MediaItem media, Func<string, string> linkGenerator)
        {
            return media == null ? null : new MediaItemDto
            {
                Id = media.Id,
                Uri = media.Uri,
                Name = media.Name,
                Type = media.Type,
                ModifiedDate = media.ModifiedDate,
                Link = linkGenerator(media.Uri)
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