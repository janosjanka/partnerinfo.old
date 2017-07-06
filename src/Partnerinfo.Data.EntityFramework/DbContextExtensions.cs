// Copyright (c) János Janka. All rights reserved.

using System;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Diagnostics;
using System.Linq;

namespace Partnerinfo
{
    /// <summary>
    /// Extensions for Entity Framework 7.0
    /// </summary>
    internal static class DbContextExtensions
    {
        /// <summary>
        /// Adds an entity to the DbContext
        /// </summary>
        public static TEntity Add<TEntity>(this DbContext context, TEntity entity) where TEntity : class
        {
            Debug.Assert(context != null);
            Debug.Assert(entity != null);

            var entry = context.Entry(entity);
            if (entry == null)
            {
                context.Set<TEntity>().Add(entity);
            }
            else
            {
                entry.State = EntityState.Added;
            }
            return entity;
        }

        /// <summary>
        /// Updates an entity and sets the ModifiedDate to the current date
        /// </summary>
        public static TEntity Update<TEntity>(this DbContext context, TEntity entity) where TEntity : class
        {
            Debug.Assert(context != null);
            Debug.Assert(entity != null);

            var entry = context.Entry(entity);
            entry.State = EntityState.Modified;
            SetModifiedDate(entry);
            return entity;
        }

        /// <summary>
        /// Patches an entity and sets the ModifiedDate to the current date
        /// </summary>
        public static TEntity Patch<TEntity>(this DbContext context, TEntity entity, object changes) where TEntity : class
        {
            Debug.Assert(context != null);
            Debug.Assert(changes != null);

            var entry = context.Entry(entity);
            entry.CurrentValues.SetValues(changes);
            SetModifiedDate(entry);
            return entity;
        }

        /// <summary>
        /// Deletes an entity
        /// </summary>
        public static TEntity Remove<TEntity>(this DbContext context, TEntity entity) where TEntity : class
        {
            Debug.Assert(context != null);
            Debug.Assert(entity != null);

            // An Added entity does not yet exist in the database. If it is then marked as deleted there is
            // nothing to delete because it was not yet inserted, so just make sure it doesn't get inserted.
            var entry = context.Entry(entity);
            entry.State = entry.State == EntityState.Added ? EntityState.Unchanged : EntityState.Deleted;
            return entity;
        }

        /// <summary>
        /// Updates the property 'ModifiedDate' when the entity was last written.
        /// </summary>
        private static void SetModifiedDate(DbEntityEntry entry)
        {
            if (entry.CurrentValues.PropertyNames.Contains("ModifiedDate", StringComparer.Ordinal))
            {
                var modifiedDate = entry.Property("ModifiedDate");
                if (modifiedDate != null)
                {
                    modifiedDate.CurrentValue = DateTime.UtcNow;
                }
            }
        }
    }
}
