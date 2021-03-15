// Copyright (c) János Janka. All rights reserved.

using System;
using System.Collections.Generic;
using System.Collections.Immutable;

namespace Partnerinfo
{
    /// <summary>
    /// A static factory class to create a new instance of the generic <see cref="ListResult{T}" /> class
    /// using type inference/deduction on methods. Also, factory methods enable us to cache immutable instances
    /// decreasing GC pressure.
    /// </summary>
    public static class ListResult
    {
        /// <summary>
        /// Creates a new instance of the <see cref="ListResult{T}" /> class or returns a cached version of the immutable object.
        /// </summary>
        /// <typeparam name="T">The type of the items.</typeparam>
        /// <returns>
        /// The <see cref="ListResult{T}" />.
        /// </returns>
        public static ListResult<T> Create<T>()
        {
            return ListResult<T>.Empty;
        }

        /// <summary>
        /// Creates a new instance of the <see cref="ListResult{T}" /> class or returns a cached version of the immutable object.
        /// </summary>
        /// <typeparam name="T">The type of the items.</typeparam>
        /// <param name="data">The strongly typed list of results to return.</param>
        /// <returns>
        /// The <see cref="ListResult{T}" />.
        /// </returns>
        public static ListResult<T> Create<T>(ImmutableArray<T> data)
        {
            if (data.Length == 0)
            {
                return Create<T>();
            }

            return new ListResult<T>(data, data.Length);
        }

        /// <summary>
        /// Creates a new instance of the <see cref="ListResult{T}" /> class or returns a cached version of the immutable object.
        /// </summary>
        /// <param name="data">The strongly typed list of results to return.</param>
        /// <returns>
        /// The <see cref="ListResult{T}" />.
        /// </returns>
        public static ListResult<T> Create<T>(ImmutableArray<T> data, int total)
        {
            if (data.Length == 0 && total == 0)
            {
                return Create<T>();
            }

            return new ListResult<T>(data, total);
        }

        /// <summary>
        /// Creates a new instance of the <see cref="ListResult{T}" /> class or returns a cached version of the immutable object.
        /// </summary>
        /// <param name="data">The strongly typed list of results to return.</param>
        /// <returns>
        /// The <see cref="ListResult{T}" />.
        /// </returns>
        public static ListResult<T> Create<T>(IEnumerable<T> data)
        {
            if (data == null)
            {
                throw new ArgumentNullException(nameof(data));
            }

            return Create(data.ToImmutableArray());
        }

        /// <summary>
        /// Creates a new instance of the <see cref="ListResult{T}" /> class or returns a cached version of the immutable object.
        /// </summary>
        /// <param name="data">The strongly typed list of results to return.</param>
        /// <returns>
        /// The <see cref="ListResult{T}" />.
        /// </returns>
        public static ListResult<T> Create<T>(IEnumerable<T> data, int total)
        {
            if (data == null)
            {
                throw new ArgumentNullException(nameof(data));
            }

            return Create(data.ToImmutableArray(), total);
        }
    }
}
