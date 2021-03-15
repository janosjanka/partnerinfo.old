﻿// Copyright (c) János Janka. All rights reserved.

using System.Collections.Immutable;

namespace Partnerinfo
{
    /// <summary>
    /// Represents an immutable, thread-safe list of items.
    /// Use the static <see cref="ListResult" /> class to create a new instance of the <see cref="ListResult{T}" /> class.
    /// </summary>
    /// <typeparam name="T">The type of items.</typeparam>
    public sealed class ListResult<T>
    {
        /// <summary>
        /// Represents an empty <see cref="ListResult{T}" /> instance.
        /// </summary>
        public static readonly ListResult<T> Empty = new ListResult<T>();

        /// <summary>
        /// Gets the strongly typed list of results to return.
        /// </summary>
        /// <value>
        /// The strongly typed list of results to return.
        /// </value>
        public ImmutableArray<T> Data { get; }

        /// <summary>
        /// Gets the total number of results before paging is applied.
        /// </summary>
        /// <value>
        /// The total number of results before paging is applied.
        /// </value>
        public int Total { get; }

        /// <summary>
        /// Prevents a default instance of the <see cref="ListResult{T}" /> class from being created.
        /// </summary>
        internal ListResult()
        {
            Data = ImmutableArray<T>.Empty;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ListResult{T}" /> class.
        /// </summary>
        /// <param name="data">The strongly typed list of results to return.</param>
        /// <param name="total">The total number of results before paging is applied.</param>
        internal ListResult(ImmutableArray<T> data, int total)
        {
            Data = data;
            Total = total;
        }
    }
}