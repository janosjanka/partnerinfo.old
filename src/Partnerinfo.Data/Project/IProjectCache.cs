// Copyright (c) János Janka. All rights reserved.

using System;

namespace Partnerinfo.Project
{
    /// <summary>
    /// Provides facilities for querying and working with cached project data.
    /// </summary>
    public interface IProjectCache : IDisposable
    {
        /// <summary>
        /// Gets a project from the cache with the given primary key.
        /// </summary>
        /// <param name="id">The primary key for the item to be found.</param>
        /// <returns>
        /// A task that represents the asynchronous operation.
        /// </returns>
        ProjectItem GetById(int id);

        /// <summary>
        /// Adds or replaces a project in the cache.
        /// </summary>
        /// <param name="project">The project to add or replace.</param>
        void Put(ProjectItem project);

        /// <summary>
        /// Removes a project from the cache.
        /// </summary>
        /// <param name="project">The project to remove.</param>
        void Remove(ProjectItem project);
    }
}
