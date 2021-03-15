// Copyright (c) János Janka. All rights reserved.

using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Partnerinfo.Logging.Rules
{
    public class RuleInvoker
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="RuleInvoker" /> class.
        /// </summary>
        /// <param name="resolver">The dependency resolver.</param>
        /// <param name="evaluator">The evaluator.</param>
        /// <param name="processor">The processor.</param>
        /// <exception cref="System.ArgumentNullException">
        /// </exception>
        public RuleInvoker(IServiceResolver resolver, IRuleEvaluator evaluator, IRuleProcessor processor)
        {
            if (resolver == null)
            {
                throw new ArgumentNullException(nameof(resolver));
            }
            if (evaluator == null)
            {
                throw new ArgumentNullException(nameof(evaluator));
            }
            if (processor == null)
            {
                throw new ArgumentNullException(nameof(processor));
            }
            Resolver = resolver;
            Evaluator = evaluator;
            Processor = processor;
        }

        /// <summary>
        /// Dependency Resolver
        /// </summary>
        protected IServiceResolver Resolver { get; private set; }

        /// <summary>
        /// Gets the rule evaluator for the invoker.
        /// </summary>
        protected IRuleEvaluator Evaluator { get; private set; }

        /// <summary>
        /// Gets the rule processor for the invoker.
        /// </summary>
        protected IRuleProcessor Processor { get; private set; }

        /// <summary>
        /// Called by the runtime to execute a command.
        /// </summary>
        public virtual async Task InvokeAsync(IEnumerable<RuleItem> rules, EventResult eventItem, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            if (rules == null)
            {
                throw new ArgumentNullException(nameof(rules));
            }
            if (eventItem == null)
            {
                throw new ArgumentNullException(nameof(eventItem));
            }
            foreach (var rule in rules)
            {
                var context = new RuleProContext(Resolver, rule, eventItem);
                if (Evaluator.Evaluate(context))
                {
                    await Processor.ExecuteAsync(context, cancellationToken);
                }
            }
        }

        /// <summary>
        /// Called by the runtime to execute a command.
        /// </summary>
        public virtual Task InvokeAsync(RuleItem rule, EventResult eventItem, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            if (rule == null)
            {
                throw new ArgumentNullException(nameof(rule));
            }
            if (eventItem == null)
            {
                throw new ArgumentNullException(nameof(eventItem));
            }
            var context = new RuleProContext(Resolver, rule, eventItem);
            if (Evaluator.Evaluate(context))
            {
                return Processor.ExecuteAsync(context, cancellationToken);
            }
            return Task.FromResult(false);
        }
    }
}
