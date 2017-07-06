// Copyright (c) János Janka. All rights reserved.

using System;

namespace Partnerinfo.Logging.Rules
{
    public class RuleProContext
    {
        public RuleProContext(IServiceResolver resolver, RuleItem ruleItem, EventResult eventItem)
        {
            if (resolver == null)
            {
                throw new ArgumentNullException(nameof(resolver));
            }
            if (ruleItem == null)
            {
                throw new ArgumentNullException(nameof(ruleItem));
            }
            if (eventItem == null)
            {
                throw new ArgumentNullException(nameof(eventItem));
            }
            Resolver = resolver;
            RuleItem = ruleItem;
            EventItem = eventItem;
        }

        /// <summary>
        /// Dependency Resolver
        /// </summary>
        public IServiceResolver Resolver { get; private set; }

        /// <summary>
        /// Gets the rule item.
        /// </summary>
        /// <value>
        /// The rule item.
        /// </value>
        public RuleItem RuleItem { get; private set; }

        /// <summary>
        /// Gets the event item.
        /// </summary>
        /// <value>
        /// The event item.
        /// </value>
        public EventResult EventItem { get; private set; }

        /// <summary>
        /// Resolves singly registered services that support arbitrary object creation.
        /// </summary>
        /// <returns>
        /// The requested service or object.
        /// </returns>
        public T Resolve<T>() where T : class
        {
            var svc = Resolver.Resolve(typeof(T)) as T;
            if (svc == null)
            {
                throw new InvalidOperationException($"Type is not registered: {typeof(T).Name}");
            }
            return svc;
        }
    }
}
