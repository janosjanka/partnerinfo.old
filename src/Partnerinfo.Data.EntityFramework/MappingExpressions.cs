// Copyright (c) János Janka. All rights reserved.

using System;
using System.Linq.Expressions;
using Partnerinfo.Identity;
using Partnerinfo.Identity.EntityFramework;

namespace Partnerinfo
{
    internal static class MappingExpressions
    {
        /// <summary>
        /// Represents a LINQ expression that creates a new user profile object with base data.
        /// </summary>
        internal static readonly Expression<Func<IdentityUser, AccountItem>> ExprUserAsAccountItem = u => new AccountItem
        {
            Id = u.Id,
            Email = u.Email,
            FirstName = u.FirstName,
            LastName = u.LastName,
            Gender = u.Gender,
            Birthday = u.Birthday
        };

        /// <summary>
        /// Represents a LINQ expression that creates a new user profile object with system data.
        /// </summary>
        internal static readonly Expression<Func<IdentityUser, AccountItem>> ExprUserAsUserItem = u => new UserItem
        {
            Id = u.Id,
            Email = u.Email,
            FirstName = u.FirstName,
            LastName = u.LastName,
            Gender = u.Gender,
            Birthday = u.Birthday,
            LastIPAddress = u.LastIPAddress,
            LastLoginDate = u.LastLoginDate,
            CreatedDate = u.CreatedDate,
            ModifiedDate = u.ModifiedDate
        };
    }
}
