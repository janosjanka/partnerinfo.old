// Copyright (c) János Janka. All rights reserved.

using System;
using System.Threading;
using System.Threading.Tasks;
using OpenPop.Mime;
using OpenPop.Pop3;
using Partnerinfo.Properties;

namespace Partnerinfo.Input.MailClient
{
    public class CommandPop3Client : CommandClient
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="CommandPop3Client" /> class.
        /// </summary>
        /// <param name="commandInvoker">The command invoker.</param>
        /// <exception cref="System.ArgumentNullException"></exception>
        public CommandPop3Client(ICommandInvoker commandInvoker)
        {
            if (commandInvoker == null)
            {
                throw new ArgumentNullException(nameof(commandInvoker));
            }
            CommandInvoker = commandInvoker;
        }

        /// <summary>
        /// Command invoker
        /// </summary>
        protected ICommandInvoker CommandInvoker { get; set; }

        /// <summary>
        /// Host name
        /// </summary>
        public string HostName { get; set; } = Settings.Default.CmdPop3HostName;

        /// <summary>
        /// Port
        /// </summary>
        public int Port { get; set; } = Settings.Default.CmdPop3Port;

        /// <summary>
        /// True if SSL is enabled
        /// </summary>
        public bool EnableSsl { get; set; } = Settings.Default.CmdPop3EnableSsl;

        /// <summary>
        /// User name
        /// </summary>
        public string UserName { get; set; } = Settings.Default.CmdPop3UserName;

        /// <summary>
        /// Password
        /// </summary>
        public string Password { get; set; } = Settings.Default.CmdPop3Password;

        /// <summary>
        /// Asynchronously authorizes the client.
        /// </summary>
        /// <returns>
        /// A task that represents the asynchronous operation.
        /// </returns>
        public override Task AuthorizeAsync(CancellationToken cancellationToken)
        {
            return Task.FromResult(true);
        }

        /// <summary>
        /// Asynchronusly executes the client.
        /// </summary>
        /// <returns>
        /// A task that represents the asynchronous operation.
        /// </returns>
        public override async Task ExecuteAsync(CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            using (var pop = new Pop3Client())
            {
                pop.Connect(HostName, Port, EnableSsl);
                pop.Authenticate(UserName, Password);
                for (int id = pop.GetMessageCount(); id > 0; --id)
                {
                    var message = pop.GetMessage(id);
                    if (message != null)
                    {
                        await ParseMessageAsync(message, cancellationToken);
                    }
                }
                pop.DeleteAllMessages();
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="message"></param>
        /// <param name="cancellationToken"></param>
        protected virtual Task ParseMessageAsync(Message message, CancellationToken cancellationToken = default(CancellationToken))
        {
            cancellationToken.ThrowIfCancellationRequested();
            if (message == null)
            {
                throw new ArgumentNullException("message");
            }
            var subject = message.Headers.Subject;
            if (string.IsNullOrEmpty(subject))
            {
                return Task.FromResult(false);
            }
            var htmlPart = message.FindFirstHtmlVersion();
            var textPart = message.FindFirstPlainTextVersion();
            return CommandInvoker.InvokeAsync(
                subject,
                htmlPart != null ? htmlPart.GetBodyAsText() : null,
                textPart != null ? textPart.GetBodyAsText() : null,
                cancellationToken);
        }
    }
}
