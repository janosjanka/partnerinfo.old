// Copyright (c) János Janka. All rights reserved.

using System.Diagnostics;
using System.Linq;

namespace Partnerinfo.Logging.EntityFramework
{
    internal static class LoggingQueries
    {
        /// <summary>
        /// Filters a sequences of categories based on predicates.
        /// </summary>
        public static IQueryable<LoggingCategory> Where(this IQueryable<LoggingCategory> query, int ownerId, int? projectId)
        {
            if (projectId == null)
            {
                return query.Where(c => (c.OwnerId == ownerId) && (c.ProjectId == null));
            }
            else
            {
                return query.Where(c => (c.OwnerId == ownerId) && (c.ProjectId == projectId));
            }
        }

        /// <summary>
        /// Filters a sequences of categories based on predicates.
        /// </summary>
        public static IQueryable<LoggingRule> Where(this IQueryable<LoggingRule> query, int userId)
        {
            return query.Where(r => r.UserId == userId);
        }

        /// <summary>
        /// Sorts the categories of a sequence according to a key.
        /// </summary>
        public static IQueryable<LoggingCategory> OrderBy(this IQueryable<LoggingCategory> query, CategorySortOrder orderBy)
        {
            if (orderBy == CategorySortOrder.Name)
            {
                return query.OrderBy(p => p.Name);
            }
            if (orderBy == CategorySortOrder.Recent)
            {
                return query.OrderByDescending(p => p.ModifiedDate);
            }
            return query.OrderBy(p => p.Id);
        }

        /// <summary>
        /// Projects each category of a sequence into a new form.
        /// </summary>
        public static IQueryable<CategoryItem> Select(this IQueryable<LoggingCategory> query, CategoryField fields)
        {
            return query.Select(p => new CategoryItem
            {
                Id = p.Id,
                Name = p.Name,
                Color = new ColorInfo { RGB = p.Color }
            });
        }

        /// <summary>
        /// Maps a <see cref="LoggingRule" /> entity to a <see cref="RuleResult" /> domain object.
        /// </summary>
        /// <param name="rule">The rule entity to map to a domain object.</param>
        /// <returns>
        /// The <see cref="RuleResult" /> domain object.
        /// </returns>
        public static RuleItem ToRuleItem(LoggingRule rule)
        {
            Debug.Assert(rule != null, "The rule cannot be null.");

            var result = new RuleItem { Id = rule.Id };
            var options = LoggingRuleOptionsHelpers.Deserialize(rule.Options);
            if (options.Conditions != null)
            {
                foreach (var condition in options.Conditions)
                {
                    result.Conditions.Add(condition);
                }
            }
            if (options.Actions != null)
            {
                foreach (var action in options.Actions)
                {
                    result.Actions.Add(action);
                }
            }
            return result;
        }
    }
}
