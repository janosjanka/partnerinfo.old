// Copyright (c) János Janka. All rights reserved.

using System;
using System.Collections.Immutable;

namespace Partnerinfo.Project.Templating
{
    internal sealed class ObjectDescriptor
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ObjectDescriptor" /> class.
        /// </summary>
        /// <param name="instance">The instance.</param>
        /// <param name="accessors">The accessors.</param>
        public ObjectDescriptor(object instance, ImmutableDictionary<string, Func<object, object>> accessors)
        {
            Instance = instance;
            Accessors = accessors;
        }

        /// <summary>
        /// Gets the instance.
        /// </summary>
        /// <value>
        /// The instance.
        /// </value>
        public object Instance { get; }

        /// <summary>
        /// Gets a list of property accessors for the current <see cref="Instance" />.
        /// </summary>
        /// <value>
        /// A list of property accessor.
        /// </value>
        public ImmutableDictionary<string, Func<object, object>> Accessors { get; }
    }
}
