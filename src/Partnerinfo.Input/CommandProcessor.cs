// Copyright (c) János Janka. All rights reserved.

using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Threading;
using System.Threading.Tasks;

namespace Partnerinfo.Input
{
    public class CommandProcessor : ICommandProcessor
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="CommandProcessor" /> class.
        /// </summary>
        /// <param name="processors">A collection of action activities.</param>
        /// <exception cref="System.ArgumentNullException">processors</exception>
        public CommandProcessor(IEnumerable<Lazy<ICommandProcessor, IDictionary<string, object>>> processors)
        {
            if (processors == null)
            {
                throw new ArgumentNullException("processors");
            }
            var dictionary = new Dictionary<CommandObjectKey, Lazy<ICommandProcessor, IDictionary<string, object>>>();
            foreach (var processor in processors)
            {
                object objectType;
                if (processor.Metadata.TryGetValue("Processor", out objectType))
                {
                    var metadata = objectType as CommandMetadata;
                    if (metadata != null)
                    {
                        dictionary[new CommandObjectKey(metadata.Type1, metadata.Type2)] = processor;
                    }
                }
            }
            Processors = ImmutableDictionary.ToImmutableDictionary(dictionary);
        }

        /// <summary>
        /// Gets a map of data command processors.
        /// </summary>
        /// <value>
        /// A map of data command processors.
        /// </value>
        protected ImmutableDictionary<CommandObjectKey, Lazy<ICommandProcessor, IDictionary<string, object>>> Processors { get; private set; }

        /// <summary>
        /// Called by the runtime to execute a command.
        /// </summary>
        /// <param name="context">The <see cref="CommandContext" /> to associate with this command and execution.</param>
        /// <param name="cancellationToken">A token to observe while waiting for the task to complete.</param>
        /// <returns>
        /// The <see cref="CommandResult" /> of the run task, which determines whether the command remains in the executing state, or transitions to the closed state.
        /// </returns>
        public virtual Task<CommandResult> ExecuteAsync(CommandContext context, CancellationToken cancellationToken)
        {
            if (context == null)
            {
                throw new ArgumentNullException("context");
            }
            if (context.Command?.Object == null)
            {
                return Task.FromResult(context.CreateResult(CommandStatusCode.NoAction));
            }
            Lazy<ICommandProcessor, IDictionary<string, object>> initializer;
            if (Processors.TryGetValue(new CommandObjectKey(context.Command.Object.Type, context.Command.Object.Object?.Type), out initializer))
            {
                return initializer.Value.ExecuteAsync(context, cancellationToken);
            }
            return Task.FromResult(context.CreateResult(CommandStatusCode.NoAction));
        }
    }
}
