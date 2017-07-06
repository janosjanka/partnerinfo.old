// Copyright (c) János Janka. All rights reserved.

using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Linq.Expressions;
using Partnerinfo.Identity.EntityFramework;
using Partnerinfo.Security;
using Partnerinfo.Security.EntityFramework;

namespace Partnerinfo.Project.EntityFramework
{
    internal static class ProjectQueries
    {
        /// <summary>
        /// Filters a sequences of items based on predicates.
        /// </summary>
        public static IQueryable<ProjectEntity> Where(this IQueryable<ProjectEntity> query, IQueryable<SecurityAccessRule> acl, int userId, string name)
        {
            return from project in query
                   join ace in acl on new
                   {
                       ObjectType = AccessObjectType.Project,
                       ObjectId = project.Id,
                       UserId = new int?(userId)
                   }
                   equals new
                   {
                       ObjectType = ace.ObjectType,
                       ObjectId = ace.ObjectId,
                       UserId = ace.UserId
                   }
                   where name == null || project.Name.Contains(name)
                   select project;
        }

        /// <summary>
        /// Filters a sequences of items based on predicates.
        /// </summary>
        public static DbSqlQuery<ProjectAction> QueryById(this DbSet<ProjectAction> query, int id)
        {
            return query.SqlQuery("Project.GetActionById @Id", DbParameters.Value("Id", id));
        }

        /// <summary>
        /// Filters a sequences of items based on predicates.
        /// </summary>
        public static IQueryable<ProjectAction> Where(this IQueryable<ProjectAction> query, int projectId, string name)
        {
            query = query.Where(a => a.ProjectId == projectId && a.ParentId == null);

            if (name != null)
            {
                query = query.Where(p => p.Name.StartsWith(name));
            }

            return query;
        }

        /// <summary>
        /// Filters a sequences of items based on predicates.
        /// </summary>
        public static IQueryable<ProjectBusinessTag> Where(this IQueryable<ProjectBusinessTag> query, int projectId, IEnumerable<int> ids)
        {
            return query.Where(c => c.ProjectId == projectId && ids.Contains(c.Id));
        }

        /// <summary>
        /// Filters a sequences of items based on predicates.
        /// </summary>
        public static IQueryable<ProjectContact> Where(this IQueryable<ProjectContact> query, IEnumerable<int> includeWithTags, IEnumerable<int> excludeWithTags)
        {
            if (includeWithTags.Any())
            {
                query = query.Where(c => c.BusinessTags.Any(tag => includeWithTags.Contains(tag.BusinessTagId)));
            }

            if (excludeWithTags.Any())
            {
                query = query.Where(c => !c.BusinessTags.Any(tag => excludeWithTags.Contains(tag.BusinessTagId)));
            }

            return query;
        }

        /// <summary>
        /// Filters a sequences of items based on predicates.
        /// </summary>
        public static IQueryable<ProjectContact> Where(this IQueryable<ProjectContact> query, int projectId, string name, IEnumerable<int> includeWithTags, IEnumerable<int> excludeWithTags)
        {
            query = query.Where(c => c.ProjectId == projectId);

            if (name != null)
            {
                query = query.Where(c => c.Email.Address.StartsWith(name) || c.Email.Name.StartsWith(name));
            }

            return query = query.Where(includeWithTags, excludeWithTags);
        }

        /// <summary>
        /// Filters a sequences of items based on predicates.
        /// </summary>
        public static IQueryable<ProjectContact> Where(this IQueryable<ProjectContact> query, int projectId, IEnumerable<int> ids)
        {
            return query.Where(c => c.ProjectId == projectId && ids.Contains(c.Id));
        }

        /// <summary>
        /// Filters a sequences of items based on predicates.
        /// </summary>
        public static IQueryable<ProjectMailMessage> Where(this IQueryable<ProjectMailMessage> query, int projectId, string subject)
        {
            query = query.Where(m => m.ProjectId == projectId);

            if (subject != null)
            {
                query = query.Where(m => m.Subject.StartsWith(subject));
            }

            return query;
        }

        /// <summary>
        /// Sorts the items of a sequence according to a key.
        /// </summary>
        public static IQueryable<ProjectEntity> OrderBy(this IQueryable<ProjectEntity> query, ProjectSortOrder orderBy)
        {
            if (orderBy == ProjectSortOrder.Recent)
            {
                return query.OrderByDescending(p => p.ModifiedDate);
            }

            if (orderBy == ProjectSortOrder.Name)
            {
                return query.OrderBy(p => p.Name);
            }

            return query.OrderBy(p => p.Id);
        }

        /// <summary>
        /// Sorts the items of a sequence according to a key.
        /// </summary>
        public static IQueryable<ProjectAction> OrderBy(this IQueryable<ProjectAction> query, ActionSortOrder orderBy)
        {
            if (orderBy == ActionSortOrder.Recent)
            {
                return query.OrderByDescending(p => p.ModifiedDate);
            }

            if (orderBy == ActionSortOrder.Name)
            {
                return query.OrderBy(p => p.Name);
            }

            return query.OrderBy(p => p.Id);
        }

        /// <summary>
        /// Sorts the items of a sequence according to a key.
        /// </summary>
        public static IQueryable<ProjectContact> OrderBy(this IQueryable<ProjectContact> query, ContactSortOrder orderBy)
        {
            if (orderBy == ContactSortOrder.Recent)
            {
                return query.OrderByDescending(p => p.ModifiedDate);
            }

            if (orderBy == ContactSortOrder.Name)
            {
                return query.OrderBy(p => p.Email.Name);
            }

            return query.OrderBy(p => p.Id);
        }

        /// <summary>
        /// Sorts the items of a sequence according to a key.
        /// </summary>
        public static IQueryable<ProjectMailMessage> OrderBy(this IQueryable<ProjectMailMessage> query, MailMessageSortOrder orderBy)
        {
            if (orderBy == MailMessageSortOrder.Recent)
            {
                return query.OrderByDescending(p => p.ModifiedDate);
            }

            if (orderBy == MailMessageSortOrder.Subject)
            {
                return query.OrderBy(p => p.Subject);
            }

            return query.OrderBy(p => p.Id);
        }

        /// <summary>
        /// Projects each product of a sequence into a new form.
        /// </summary>
        public static IQueryable<ProjectItem> Select(this IQueryable<ProjectEntity> query, IQueryable<SecurityAccessRule> identityAcl, IQueryable<IdentityUser> identityUsers, ProjectField fields)
        {
            Expression<Func<ProjectEntity, ProjectItem>> selector = project => new ProjectItem
            {
                Id = project.Id,
                Name = project.Name,
                Sender = project.Sender,
                CreatedDate = project.CreatedDate,
                ModifiedDate = project.ModifiedDate
            };

            if (fields.HasFlag(ProjectField.Owners))
            {
                selector = selector.Merge(project => new ProjectItem
                {
                    Owners = identityAcl
                        .Where(ace => ace.ObjectType == AccessObjectType.Project
                                   && ace.ObjectId == project.Id
                                   && ace.UserId != null
                                   && ace.Permission == AccessPermission.IsOwner)
                        .OrderBy(ace => ace.Id)
                        .Select(ace => ace.User)
                        .Select(MappingExpressions.ExprUserAsAccountItem)
                        .ToList()
                });
            }

            if (fields.HasFlag(ProjectField.Statistics))
            {
                selector = selector.Merge(project => new ProjectItem
                {
                    ContactCount = project.Contacts.Count()
                });
            }

            return query.Select(selector);
        }

        /// <summary>
        /// Projects each product of a sequence into a new form.
        /// </summary>
        public static IQueryable<ActionItem> Select(this IQueryable<ProjectAction> query, ActionField fields)
        {
            Expression<Func<ProjectAction, ActionItem>> selector = action => new ActionItem
            {
                Id = action.Id,
                Parent = action.ParentId == null ? null : new UniqueItem { Id = (int)action.ParentId },
                Enabled = action.Enabled,
                Type = action.Type,
                Name = action.Name,
                ModifiedDate = action.ModifiedDate
            };

            if (fields.HasFlag(ActionField.Project))
            {
                selector = selector.Merge(action => new ActionItem
                {
                    Project = new UniqueItem { Id = action.Project.Id, Name = action.Project.Name }
                });
            }

            return query.Select(selector);
        }

        /// <summary>
        /// Projects each product of a sequence into a new form.
        /// </summary>
        public static IQueryable<ContactItem> Select(this IQueryable<ProjectContact> query, IQueryable<ProjectBusinessTag> businessTagQuery, ContactField fields)
        {
            Expression<Func<ProjectContact, ContactItem>> selector = contact => new ContactItem
            {
                Id = contact.Id,
                FacebookId = contact.FacebookId,
                Email = contact.Email,
                FirstName = contact.FirstName,
                LastName = contact.LastName,
                NickName = contact.NickName,
                Gender = contact.Gender,
                Birthday = contact.Birthday,
                ModifiedDate = contact.ModifiedDate,
                Phones = contact.Phones,
                Comment = contact.Comment
            };

            if (fields.HasFlag(ContactField.Project))
            {
                selector = selector.Merge(contact => new ContactItem
                {
                    Project = new UniqueItem { Id = contact.Project.Id, Name = contact.Project.Name }
                });
            }

            if (fields.HasFlag(ContactField.Sponsor))
            {
                selector = selector.Merge(contact => new ContactItem
                {
                    Sponsor = contact.Sponsor == null ? null : new AccountItem
                    {
                        Id = contact.Sponsor.Id,
                        Email = contact.Sponsor.Email,
                        FirstName = contact.Sponsor.FirstName,
                        LastName = contact.Sponsor.LastName,
                        NickName = contact.Sponsor.NickName,
                        Gender = contact.Sponsor.Gender,
                        Birthday = contact.Sponsor.Birthday
                    }
                });
            }

            if (fields.HasFlag(ContactField.BusinessTags))
            {
                selector = selector.Merge(contact => new ContactItem
                {
                    BusinessTags = (from contactTag in contact.BusinessTags
                                    join businessTag in businessTagQuery on contactTag.BusinessTagId equals businessTag.Id
                                    orderby businessTag.Name
                                    select new BusinessTagItem
                                    {
                                        Id = businessTag.Id,
                                        Name = businessTag.Name,
                                        Color = businessTag.Color,
                                        CreatedDate = contactTag.CreatedDate
                                    }).ToList<UniqueItem>()
                });
            }

            return query.Select(selector);
        }

        /// <summary>
        /// Projects each product of a sequence into a new form.
        /// </summary>
        public static IQueryable<MailMessageItem> Select(this IQueryable<ProjectMailMessage> query, MailMessageField fields)
        {
            Expression<Func<ProjectMailMessage, MailMessageItem>> selector = mailMessage => new MailMessageItem
            {
                Id = mailMessage.Id,
                Subject = mailMessage.Subject,
                ModifiedDate = mailMessage.ModifiedDate
            };

            if (fields.HasFlag(MailMessageField.Project))
            {
                selector = selector.Merge(mailMessage => new MailMessageItem
                {
                    Project = new UniqueItem { Id = mailMessage.Project.Id, Name = mailMessage.Project.Name }
                });
            }

            if (fields.HasFlag(MailMessageField.Body))
            {
                selector = selector.Merge(mailMessage => new MailMessageItem
                {
                    Body = mailMessage.Body
                });
            }

            return query.Select(selector);
        }
    }
}