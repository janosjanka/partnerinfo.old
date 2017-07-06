// Copyright (c) János Janka. All rights reserved.

using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq.Expressions;
using System.Threading;

namespace Partnerinfo.Project.Templating
{
    internal sealed class StringTemplateCache : IDisposable
    {
        private readonly ReaderWriterLockSlim _cacheLock;
        private readonly Dictionary<Type, ImmutableDictionary<string, Func<object, object>>> _accessorCache;

        /// <summary>
        /// Initializes a new instance of the <see cref="StringTemplateCache"/> class.
        /// </summary>
        public StringTemplateCache()
        {
            _cacheLock = new ReaderWriterLockSlim();
            _accessorCache = new Dictionary<Type, ImmutableDictionary<string, Func<object, object>>>();
        }

        /// <summary>
        /// Makes a collection of accessor functions using reflection.
        /// </summary>
        /// <param name="type">The type of the source.</param>
        /// <param name="source">The source object.</param>
        /// <returns>A collection of accessor functions.</returns>
        private static ImmutableDictionary<string, Func<object, object>> MakeAccessors(Type type, object source)
        {
            var properties = TypeDescriptor.GetProperties(type);
            var accessors = new List<KeyValuePair<string, Func<object, object>>>();

            for (int i = 0; i < properties.Count; ++i)
            {
                var property = properties[i];
                if (property.Attributes != null)
                {
                    for (int j = 0; j < property.Attributes.Count; ++j)
                    {
                        // Use a simple typeof() because the template field attribute is a sealed class.
                        var attribute = property.Attributes[j];
                        if (attribute.GetType() == typeof(TemplateFieldAttribute))
                        {
                            var fieldAttribute = (TemplateFieldAttribute)attribute;
                            accessors.Add(new KeyValuePair<string, Func<object, object>>(
                                fieldAttribute.GetName(), MakeAccessor(type, property, fieldAttribute)));
                        }
                    }
                }
            }

            return ImmutableDictionary.CreateRange(StringComparer.OrdinalIgnoreCase, accessors);
        }

        /// <summary>
        /// Makes an accessor function for the specified property.
        /// </summary>
        private static Func<object, object> MakeAccessor(Type sourceType, PropertyDescriptor property, TemplateFieldAttribute fieldAttribute = null)
        {
            var returnWith = default(Expression);
            var sourceParam = Expression.Parameter(typeof(object), "sourceObj");
            var sourceVar = Expression.Variable(sourceType, "sourceInst");
            var propertyGetter = Expression.Property(sourceVar, property.Name);

            // A label expression of the object type that is the target for Expression.Return().
            var returnTarget = Expression.Label(typeof(object));

            if (fieldAttribute == null || fieldAttribute.Property == null)
            {
                returnWith = Expression.Return(returnTarget, Expression.Convert(propertyGetter, typeof(object)));
            }
            else
            {
                // return sourceInst.Email != null ? sourceInst.Email.Address : null;
                returnWith =
                    Expression.IfThenElse(
                        Expression.NotEqual(propertyGetter, Expression.Constant(null)),
                            Expression.Return(returnTarget, Expression.Convert(Expression.Property(propertyGetter, fieldAttribute.Property), typeof(object))),
                            Expression.Return(returnTarget, Expression.Constant(null)));
            }

            return Expression.Lambda<Func<object, object>>(
                Expression.Block(
                    Expression.IfThen(
                        Expression.NotEqual(sourceParam, Expression.Constant(null)),
                        Expression.Block(
                            new ParameterExpression[] { sourceVar },
                            Expression.Assign(sourceVar, Expression.Convert(sourceParam, sourceType)),
                            returnWith)),
                    Expression.Label(returnTarget, Expression.Constant(null))),
                sourceParam).Compile();
        }

        /// <summary>
        /// Returns a collection of property accessors for given type.
        /// </summary>
        /// <param name="type">The type object.</param>
        public ImmutableDictionary<string, Func<object, object>> Get(Type type)
        {
            _cacheLock.EnterReadLock();
            try
            {
                return _accessorCache[type];
            }
            finally
            {
                _cacheLock.ExitReadLock();
            }
        }

        /// <summary>
        /// Prepares the source object.
        /// </summary>
        /// <param name="sources">The sources.</param>
        public void Put(ImmutableDictionary<string, object> sources)
        {
            foreach (var pair in sources)
            {
                if (pair.Value != null)
                {
                    CacheAccessors(pair.Value);
                }
            }
        }

        /// <summary>
        /// Caches template field accessors for the specified source type.
        /// </summary>
        /// <param name="source">The source object.</param>
        private void CacheAccessors(object source)
        {
            var type = source.GetType();

            _cacheLock.EnterUpgradeableReadLock();
            try
            {
                if (!_accessorCache.ContainsKey(type))
                {
                    _cacheLock.EnterWriteLock();
                    try
                    {
                        _accessorCache[type] = MakeAccessors(type, source);
                    }
                    finally
                    {
                        _cacheLock.ExitWriteLock();
                    }
                }
            }
            finally
            {
                _cacheLock.ExitUpgradeableReadLock();
            }
        }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public void Dispose()
        {
            _cacheLock.Dispose();
        }
    }
}
