// Copyright (c) János Janka. All rights reserved.

using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Partnerinfo
{
    internal static class PartnerDbContextExtensions
    {
        /// <summary>
        /// Gets a collection of items and materializes the result-set to a read-only collection.
        /// </summary>
        /// <typeparam name="T">The type of the query.</typeparam>
        /// <param name="context">The context.</param>
        /// <param name="query">The query.</param>
        /// <returns>
        /// A collection of the <see cref="T" /> objects.
        /// </returns>
        public static async Task<IList<T>> FindItemsAsync<T>(this PartnerDbContext context, IQueryable<T> query, CancellationToken cancellationToken)
        {
            return await query.ToListAsync(cancellationToken);
        }

        /// <summary>
        /// Gets a collection of items, in a page of data, and materializes the result-set to a read-only collection.
        /// </summary>
        /// <typeparam name="T">The type of the query.</typeparam>
        /// <param name="context">The context.</param>
        /// <param name="query">The query.</param>
        /// <param name="offset">The index of the page of results to return. Use 1 to indicate the first page.</param>
        /// <param name="pageSize">The size of the page of results to return. <paramref name="offset" /> is non-zero-based.</param>
        /// <param name="totalItemCount">The total number of matched objects.</param>
        /// <returns>
        /// A collection of the <see cref="T" /> objects.
        /// </returns>
        public static IList<T> FindItems<T>(this PartnerDbContext context, IQueryable<T> query, int pageIndex, int pageSize, out int totalItemCount)
        {
            totalItemCount = query.Count();

            if (totalItemCount > 0)
            {
                return query.Paging(pageIndex, pageSize).ToList();
            }

            return new List<T>();
        }

        /// <summary>
        /// Gets a collection of items, in a page of data, and materializes the result-set to a read-only collection.
        /// </summary>
        /// <typeparam name="T">The type of the query.</typeparam>
        /// <param name="context">The context.</param>
        /// <param name="query">The query.</param>
        /// <param name="pageIndex">Index of the page.</param>
        /// <param name="pageSize">The size of the page of results to return. <paramref name="offset" /> is non-zero-based.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>
        /// A collection of the <see cref="T" /> objects.
        /// </returns>
        public static async Task<ListResult<T>> FindItemsAsync<T>(this PartnerDbContext context, IQueryable<T> query, int pageIndex, int pageSize, CancellationToken cancellationToken)
        {
            int count = await query.CountAsync(cancellationToken);
            if (count == 0)
            {
                return ListResult<T>.Empty;
            }
            return ListResult.Create(await query.Paging(pageIndex, pageSize).ToListAsync(cancellationToken), count);
        }
    }
}
