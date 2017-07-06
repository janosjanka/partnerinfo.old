// Copyright (c) János Janka. All rights reserved.

using System;

namespace Partnerinfo.Portal
{
    /// <summary>
    /// Provides facilities for querying and working with cached project data.
    /// </summary>
    public interface IPortalCache : IDisposable
    {
        /// <summary>
        /// Adds or replaces a project in the cache.
        /// </summary>
        /// <param name="portal">The project to add or replace.</param>
        void Put(PortalItem portal);

        /// <summary>
        /// Removes a project from the cache.
        /// </summary>
        /// <param name="portal">The project to remove.</param>
        void Remove(PortalItem portal);

        /// <summary>
        /// Gets a project from the cache with the given primary key.
        /// </summary>
        /// <param name="normalizedUri">The primary key for the item to be found.</param>
        /// <returns>
        /// A task that represents the asynchronous operation.
        /// </returns>
        PortalItem Get(string normalizedUri);
    }
}
