// Copyright (c) János Janka. All rights reserved.

using System;
using System.Threading;
using System.Threading.Tasks;

namespace Partnerinfo.Input
{
    public class CommandInvoker : ICommandInvoker
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="CommandInvoker" /> class.
        /// </summary>
        /// <param name="resolver">The dependency resolver used by actions.</param>
        /// <param name="processor">The start activity.</param>
        /// <exception cref="System.ArgumentNullException">
        /// </exception>
        public CommandInvoker(IServiceResolver resolver, ICommandProcessor processor)
        {
            if (resolver == null)
            {
                throw new ArgumentNullException(nameof(resolver));
            }
            if (processor == null)
            {
                throw new ArgumentNullException(nameof(processor));
            }
            Resolver = resolver;
            Processor = processor;
        }

        /// <summary>
        /// Dependency Resolver
        /// </summary>
        protected IServiceResolver Resolver { get; private set; }

        /// <summary>
        /// Command Processor
        /// </summary>
        protected ICommandProcessor Processor { get; private set; }

        /// <summary>
        /// The command parser that converts the given input string(s) to CLR objects.
        /// </summary>
        public ICommandParser Parser { get; set; } = CommandParser.Default;

        /// <summary>
        /// Called by the runtime to execute a command.
        /// </summary>
        public virtual Task<CommandResult> InvokeAsync(string command, string htmlContent, string textContent, CancellationToken cancellationToken)
        {
            Command dataCommand;
            if (Parser == null || command == null)
            {
                dataCommand = new Command { Line = command, HtmlContent = htmlContent, TextContent = textContent };
            }
            else
            {
                try
                {
                    dataCommand = Parser.Parse(command, htmlContent, textContent);
                }
                catch (CommandParserException)
                {
                    dataCommand = new Command { Line = command, HtmlContent = htmlContent, TextContent = textContent };
                }
            }
            return InvokeAsync(dataCommand, cancellationToken);
        }

        /// <summary>
        /// Called by the runtime to execute a command.
        /// </summary>
        public virtual async Task<CommandResult> InvokeAsync(Command command, CancellationToken cancellationToken)
        {
            if (command == null)
            {
                throw new ArgumentNullException("command");
            }
            return await Processor.ExecuteAsync(new CommandContext(Resolver, command), cancellationToken);
        }
    }
}
