// Copyright (c) János Janka. All rights reserved.

using System;
using System.Linq;
using System.Linq.Expressions;
using Partnerinfo.Identity.EntityFramework;
using Partnerinfo.Security;
using Partnerinfo.Security.EntityFramework;

namespace Partnerinfo.Portal.EntityFramework
{
    internal static class PortalQueries
    {
        /// <summary>
        /// Filters a sequences of items based on predicates.
        /// </summary>
        public static IQueryable<PortalEntity> Where(this IQueryable<PortalEntity> query, IQueryable<SecurityAccessRule> acl, int userId)
        {
            return from portal in query
                   join ace in acl on new
                   {
                       ObjectType = AccessObjectType.Portal,
                       ObjectId = portal.Id,
                       UserId = new int?(userId)
                   }
                   equals new
                   {
                       ObjectType = ace.ObjectType,
                       ObjectId = ace.ObjectId,
                       UserId = ace.UserId
                   }
                   select portal;
        }

        /// <summary>
        /// Filters a sequences of items based on predicates.
        /// </summary>
        public static IQueryable<PortalEntity> Where(this IQueryable<PortalEntity> query, IQueryable<SecurityAccessRule> acl, int userId, string name)
        {
            var q = Where(query, acl, userId);

            if (name != null)
            {
                q = q.Where(p => p.Name.Contains(name));
            }

            return q;
        }

        /// <summary>
        /// Filters a sequences of items based on predicates.
        /// </summary>
        public static IQueryable<PortalMedia> Where(this IQueryable<PortalMedia> query, int id)
        {
            return query.Where(m => m.Id == id);
        }

        /// <summary>
        /// Filters a sequences of items based on predicates.
        /// </summary>
        public static IQueryable<PortalMedia> Where(this IQueryable<PortalMedia> query, int portalId, int? parentId, string name)
        {
            query = query.Where(m => m.PortalId == portalId && m.ParentId == parentId);

            if (name != null)
            {
                query = query.Where(m => m.Name.Contains(name));
            }

            return query;
        }

        /// <summary>
        /// Sorts the items of a sequence according to a key.
        /// </summary>
        public static IQueryable<PortalEntity> OrderBy(this IQueryable<PortalEntity> query, PortalSortOrder orderBy)
        {
            if (orderBy == PortalSortOrder.Recent)
            {
                return query.OrderByDescending(p => p.ModifiedDate);
            }

            if (orderBy == PortalSortOrder.Name)
            {
                return query.OrderBy(p => p.Name);
            }

            return query.OrderBy(p => p.Id);
        }

        /// <summary>
        /// Sorts the items of a sequence according to a key.
        /// </summary>
        public static IQueryable<PortalMedia> OrderBy(this IQueryable<PortalMedia> query, MediaSortOrder orderBy)
        {
            if (orderBy == MediaSortOrder.Recent)
            {
                return query.OrderByDescending(p => p.ModifiedDate);
            }

            if (orderBy == MediaSortOrder.Name)
            {
                return query.OrderBy(p => p.Name);
            }

            return query.OrderBy(p => p.Id);
        }

        /// <summary>
        /// Projects each product of a sequence into a new form.
        /// </summary>
        public static IQueryable<PortalItem> Select(this IQueryable<PortalEntity> query, IQueryable<SecurityAccessRule> acl, IQueryable<IdentityUser> users, PortalField fields)
        {
            Expression<Func<PortalEntity, PortalItem>> selector = portal => new PortalItem
            {
                Id = portal.Id,
                Uri = portal.Uri,
                Domain = portal.Domain,
                CreatedDate = portal.CreatedDate,
                ModifiedDate = portal.ModifiedDate,
                Name = portal.Name,
                Description = portal.Description,
                GATrackingId = portal.GATrackingId
            };

            if (fields.HasFlag(PortalField.Owners))
            {
                selector = selector.Merge(portal => new PortalItem
                {
                    Owners = (from ace in acl
                              where ace.ObjectType == AccessObjectType.Portal
                                 && ace.ObjectId == portal.Id
                                 && ace.UserId != null
                                 && ace.Permission == AccessPermission.IsOwner
                              orderby ace.Id
                              select ace.User)
                              .Select(MappingExpressions.ExprUserAsAccountItem)
                              .ToList()
                });
            }

            if (fields.HasFlag(PortalField.Project))
            {
                selector = selector.Merge(portal => new PortalItem
                {
                    Project = portal.Project == null ? null : new UniqueItem
                    {
                        Id = portal.Project.Id,
                        Name = portal.Project.Name
                    }
                });
            }

            if (fields.HasFlag(PortalField.HomePage))
            {
                selector = selector.Merge(portal => new PortalItem
                {
                    HomePage = portal.HomePage == null ? null : new ResourceItem
                    {
                        Id = portal.HomePage.Id,
                        Uri = PortalFunctions.GetPageUriById(portal.HomePage.Id),
                        Name = portal.HomePage.Name
                    }
                });
            }

            if (fields.HasFlag(PortalField.MasterPage))
            {
                selector = selector.Merge(portal => new PortalItem
                {
                    MasterPage = portal.MasterPage == null ? null : new ResourceItem
                    {
                        Id = portal.MasterPage.Id,
                        Uri = PortalFunctions.GetPageUriById(portal.MasterPage.Id),
                        Name = portal.MasterPage.Name
                    }
                });
            }

            return query.Select(selector);
        }

        /// <summary>
        /// Projects each product of a sequence into a new form.
        /// </summary>
        public static IQueryable<PageItem> Select(this IQueryable<PortalPage> query, PageField fields)
        {
            Expression<Func<PortalPage, PageItem>> selector = page => new PageItem
            {
                Id = page.Id,
                Uri = page.Uri,
                ModifiedDate = page.ModifiedDate,
                Name = page.Name,
                Description = page.Description
            };

            if (fields.HasFlag(PageField.Portal))
            {
                selector = selector.Merge(page => new PageItem
                {
                    Portal = new ResourceItem
                    {
                        Id = page.Portal.Id,
                        Uri = page.Portal.Uri,
                        Name = page.Portal.Name
                    }
                });
            }

            if (fields.HasFlag(PageField.Master))
            {
                selector = selector.Merge(page => new PageItem
                {
                    Master = page.Master == null ? null : new ResourceItem
                    {
                        Id = page.Master.Id,
                        Uri = PortalFunctions.GetPageUriById(page.Master.Id),
                        Name = page.Master.Name
                    }
                });
            }

            if (fields.HasFlag(PageField.Content))
            {
                selector = selector.Merge(page => new PageItem
                {
                    HtmlContent = page.HtmlContent,
                    StyleContent = page.StyleContent
                });
            }

            return query.Select(selector);
        }

        /// <summary>
        /// Projects each product of a sequence into a new form.
        /// </summary>
        public static IQueryable<MediaItem> Select(this IQueryable<PortalMedia> query, MediaField fields)
        {
            Expression<Func<PortalMedia, MediaItem>> selector = media => new MediaItem
            {
                Id = media.Id,
                Uri = media.Uri,
                Type = media.Type,
                Name = media.Name,
                ModifiedDate = media.ModifiedDate
            };

            //if (fields.HasFlag(MediaField.Portal))
            //{
            //    selector = selector.Merge(media => new MediaItem
            //    {
            //        Portal = new ResourceItem
            //        {
            //            Id = media.Portal.Id,
            //            Uri = media.Portal.Uri,
            //            Name = media.Portal.Name
            //        }
            //    });
            //}

            return query.Select(selector);
        }
    }
}
