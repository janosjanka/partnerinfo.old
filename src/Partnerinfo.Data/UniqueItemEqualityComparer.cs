// Copyright (c) János Janka. All rights reserved.

using System;
using System.Collections.Generic;

namespace Partnerinfo
{
    /// <summary>
    /// Defines methods to support the comparison of objects for equality.
    /// </summary>
    public sealed class UniqueItemEqualityComparer : IEqualityComparer<UniqueItem>
    {
        /// <summary>
        /// An equality comparer that can compare two unique resources by ids.
        /// </summary>
        public static readonly IEqualityComparer<UniqueItem> Default = new UniqueItemEqualityComparer();

        /// <summary>
        /// When overridden in a derived class, determines whether two objects of type <paramref name="T" /> are equal.
        /// </summary>
        /// <param name="x">The first object to compare.</param>
        /// <param name="y">The second object to compare.</param>
        /// <returns>
        /// true if the specified objects are equal; otherwise, false.
        /// </returns>
        public bool Equals(UniqueItem x, UniqueItem y)
        {
            if (x == y)
            {
                return true;
            }
            if (x == null || y == null)
            {
                return false;
            }
            return x.Id == y.Id;
        }

        /// <summary>
        /// Returns a hash code for this instance.
        /// </summary>
        /// <param name="obj">The obj.</param>
        /// <returns>
        /// A hash code for this instance, suitable for use in hashing algorithms and data structures like a hash table.
        /// </returns>
        /// <exception cref="System.ArgumentNullException">The type of obj is a reference type and obj is null.</exception>
        public int GetHashCode(UniqueItem obj)
        {
            if (obj == null)
            {
                throw new ArgumentNullException(nameof(obj));
            }
            return obj.Id.GetHashCode();
        }
    }
}
