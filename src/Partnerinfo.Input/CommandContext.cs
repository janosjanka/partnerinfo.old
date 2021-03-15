// Copyright (c) János Janka. All rights reserved.

using System;
using System.Collections.Generic;

namespace Partnerinfo.Input
{
    public class CommandContext
    {
        public CommandContext(IServiceResolver resolver, Command command)
        {
            if (resolver == null)
            {
                throw new ArgumentNullException("resolver");
            }
            if (command == null)
            {
                throw new ArgumentNullException("command");
            }
            Resolver = resolver;
            Command = command;
        }

        /// <summary>
        /// Dependency Resolver
        /// </summary>
        public IServiceResolver Resolver { get; private set; }

        /// <summary>
        /// The command object
        /// </summary>
        public Command Command { get; private set; }

        /// <summary>
        /// Error messages
        /// </summary>
        public IList<string> Errors { get; } = new List<string>();

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

        /// <summary>
        /// Creates an activity result passing the current contact state to the result object.
        /// </summary>
        /// <param name="statusCode">The new status code.</param>
        /// <returns>
        /// The result object.
        /// </returns>
        public CommandResult CreateResult(CommandStatusCode statusCode)
        {
            return new CommandResult(statusCode, Errors);
        }
    }
}
