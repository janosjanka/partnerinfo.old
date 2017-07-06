// Copyright (c) János Janka. All rights reserved.

using System;
using System.Collections.Generic;
using System.Linq;
using Partnerinfo.Security;

namespace Partnerinfo.Drive.EntityFramework
{
    internal static class DriveQueries
    {
        /// <summary>
        /// Gets a collection of all the projects in the database.
        /// </summary>
        public static readonly Func<PartnerDbContext, int, int?, string, IQueryable<FileResult>> QueryAll =
            (PartnerDbContext context, int userId, int? parentId, string name) =>
                from d in context.DriveDocuments
                join a in context.SecurityAccessRules on new
                {
                    ObjectType = AccessObjectType.File,
                    ObjectId = d.Id,
                    UserId = new int?(userId)
                }
                equals new
                {
                    ObjectType = a.ObjectType,
                    ObjectId = a.ObjectId,
                    UserId = a.UserId
                }
                where (d.ParentId == parentId)
                   && (name == null || d.Name.StartsWith(name))
                orderby d.Type ascending, d.ModifiedDate descending
                select new FileResult
                {
                    Id = d.Id,
                    ParentId = d.ParentId,
                    OwnerId = d.OwnerId,
                    Type = d.Type,
                    Name = d.Name,
                    Length = d.Length,
                    Slug = d.Slug,
                    PhysicalPath = d.PhysicalPath,
                    Owners = (from oa in context.SecurityAccessRules
                              where oa.ObjectType == AccessObjectType.File
                                 && oa.ObjectId == d.Id
                                 && oa.UserId != null
                                 && oa.Permission == AccessPermission.IsOwner
                              orderby oa.Id
                              select oa.User).Select(MappingExpressions.ExprUserAsAccountItem).ToList(),
                    CreatedDate = d.CreatedDate,
                    ModifiedDate = d.ModifiedDate
                };

        /// <summary>
        /// Gets a collection of all the documents in the database.
        /// </summary>
        public static readonly Func<PartnerDbContext, IEnumerable<int>, IQueryable<FileItem>> QueryAllByIds =
            (PartnerDbContext context, IEnumerable<int> documents) =>
                from d in context.DriveDocuments
                where documents.Contains(d.Id)
                select d;
    }
}
