// Copyright (c) János Janka. All rights reserved.

using System;
using System.Linq;
using System.Linq.Expressions;

namespace Partnerinfo.Identity.EntityFramework
{
    internal static class UserQueries
    {
        /// <summary>
        /// Filters a sequences of items based on predicates.
        /// </summary>
        public static IQueryable<IdentityUser> Where(this IQueryable<IdentityUser> query, string name)
        {
            if (name != null)
            {
                query = query.Where(user => user.Email.Name.StartsWith(name));
            }
            return query;
        }

        /// <summary>
        /// Sorts the items of a sequence according to a key.
        /// </summary>
        public static IQueryable<IdentityUser> OrderBy(this IQueryable<IdentityUser> query, UserSortOrder orderBy)
        {
            if (orderBy == UserSortOrder.Recent)
            {
                return query.OrderByDescending(user => user.LastLoginDate);
            }

            if (orderBy == UserSortOrder.Name)
            {
                return query.OrderBy(user => user.Email.Name);
            }

            return query.OrderBy(user => user.Id);
        }

        /// <summary>
        /// Projects each user of a sequence into a new form.
        /// </summary>
        public static IQueryable<UserItem> Select(this IQueryable<IdentityUser> query, UserField fields)
        {
            Expression<Func<IdentityUser, UserItem>> selector = user => new UserItem
            {
                Id = user.Id,
                Email = user.Email,
                FirstName = user.FirstName,
                LastName = user.LastName,
                NickName = user.NickName,
                Gender = user.Gender,
                Birthday = user.Birthday,

                CreatedDate = user.CreatedDate,
                ModifiedDate = user.ModifiedDate,
                LastLoginDate = user.LastLoginDate,
                LastIPAddress = user.LastIPAddress,
                LastEventDate = user.LastEventDate
            };

            return query.Select(selector);
        }
    }
}
