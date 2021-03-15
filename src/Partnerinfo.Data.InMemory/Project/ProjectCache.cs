// Copyright (c) János Janka. All rights reserved.

using System;
using System.Runtime.Caching;

namespace Partnerinfo.Project.InMemory
{
    public class ProjectCache : IProjectCache
    {
        private bool _disposed;

        /// <summary>
        /// Initializes a new instance of the <see cref="ProjectCache" /> class.
        /// </summary>
        public ProjectCache()
        {
            Cache = new MemoryCache("Project.Projects");
        }

        /// <summary>
        /// Gets the object cache for projects.
        /// </summary>
        /// <value>
        /// The object cache.
        /// </value>
        protected MemoryCache Cache { get; set; }

        /// <summary>
        /// Gets a project from the cache with the given primary key.
        /// </summary>
        /// <param name="id">The primary key for the item to be found.</param>
        /// <returns>
        /// A task that represents the asynchronous operation.
        /// </returns>
        public virtual ProjectItem GetById(int id)
        {
            ThrowIfDisposed();
            return Cache.Get(id.ToString()) as ProjectItem;
        }

        /// <summary>
        /// Adds or replaces a project in the cache.
        /// </summary>
        /// <param name="project">The project to add or replace.</param>
        public virtual void Put(ProjectItem project)
        {
            ThrowIfDisposed();
            if (project == null)
            {
                throw new ArgumentNullException(nameof(project));
            }
            Cache.Set(project.Id.ToString(), project, new CacheItemPolicy { SlidingExpiration = TimeSpan.FromSeconds(30) });
        }

        /// <summary>
        /// Removes a project from the cache.
        /// </summary>
        /// <param name="project">The project to remove.</param>
        public virtual void Remove(ProjectItem project)
        {
            ThrowIfDisposed();
            if (project == null)
            {
                throw new ArgumentNullException(nameof(project));
            }
            Cache.Remove(project.Id.ToString());
        }

        /// <summary>
        /// Throws a <see cref="ObjectDisposedException" /> if the context has already been disposed.
        /// </summary>
        /// <exception cref="System.ObjectDisposedException" />
        private void ThrowIfDisposed()
        {
            if (_disposed)
            {
                throw new ObjectDisposedException(GetType().Name);
            }
        }

        /// <summary>
        /// Releases unmanaged and - optionally - managed resources.
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// If disposing, calls dispose on the Context.  Always nulls out the Context
        /// </summary>
        /// <param name="disposing"><c>true</c> to release both managed and unmanaged resources; <c>false</c> to release only unmanaged resources.</param>
        protected virtual void Dispose(bool disposing)
        {
            if (disposing && !_disposed)
            {
                Cache.Dispose();
            }
            _disposed = true;
        }
    }
}
