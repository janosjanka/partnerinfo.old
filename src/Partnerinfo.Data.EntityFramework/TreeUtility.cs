// Copyright (c) János Janka. All rights reserved.

using System;
using System.Collections.Generic;
using System.Linq;

namespace Partnerinfo
{
    public static class TreeUtility
    {
        /// <summary>
        /// Builds a tree from a sequential collection.
        /// </summary>
        public static T BuildTreeNode<T, TKey>(IList<T> items, Func<T, TKey> idFunc, Func<T, TKey> parentIdFunc, Action<T, T> addToChildren)
        {
            if (items == null)
            {
                throw new ArgumentNullException("items");
            }

            if (items.Count > 0)
            {
                T root = items[0];
                if (items.Count > 1)
                {
                    var dictionary = items.ToDictionary(idFunc);
                    foreach (T item in dictionary.Values)
                    {
                        TKey parentId = parentIdFunc(item);
                        if (parentId != null)
                        {
                            T parent;
                            if (dictionary.TryGetValue(parentId, out parent))
                            {
                                addToChildren(parent, item);
                            }
                        }
                    }
                }
                return root;
            }
            return default(T);
        }
    }
}
