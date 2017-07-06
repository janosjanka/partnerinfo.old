// Copyright (c) János Janka. All rights reserved.

using System.Linq;

namespace Partnerinfo.Security.EntityFramework
{
    internal static class SecurityQueries
    {
        /// <summary>
        /// Filters a sequences of items based on predicates.
        /// </summary>
        public static IQueryable<SecurityAccessRule> Where(this IQueryable<SecurityAccessRule> query, AccessObjectType objectType, int objectId)
        {
            return query.Where(ace => ace.ObjectType == objectType && ace.ObjectId == objectId);
        }

        /// <summary>
        /// Filters a sequences of items based on predicates.
        /// </summary>
        public static IQueryable<SecurityAccessRule> Where(this IQueryable<SecurityAccessRule> query, AccessObjectType objectType, int objectId, int? userId)
        {
            return query.Where(ace => ace.ObjectType == objectType && ace.ObjectId == objectId && ace.UserId == userId);
        }
    }
}
