// Copyright (c) János Janka. All rights reserved.

using System;
using System.Linq;
using System.Net.Mail;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Hangfire;
using Partnerinfo.Logging;

namespace Partnerinfo.Project.Mail
{
    public class MailJobClient : IDisposable
    {
        private bool _disposed;
        private IMailViewProvider _mailTextProvider;

        /// <summary>
        /// Initializes a new instance of the <see cref="MailJobClient" /> class.
        /// </summary>
        /// <param name="smtp"></param>
        public MailJobClient(SmtpClient smtp)
        {
            if (smtp == null)
            {
                throw new ArgumentNullException(nameof(smtp));
            }
            Smtp = smtp;
        }

        /// <summary>
        /// The service that manages log events
        /// </summary>
        public LogManager LogManager { get; set; }

        /// <summary>
        /// Simple Mail Transfer Protocol (SMTP) service
        /// </summary>
        protected SmtpClient Smtp { get; set; }

        /// <summary>
        /// A view provider that creates different message formats for the same message
        /// </summary>
        public IMailViewProvider MailTextProvider
        {
            get
            {
                return _mailTextProvider ?? MailViewProvider.Default;
            }
            set
            {
                _mailTextProvider = value;
            }
        }

        /// <summary>
        /// Sends this mail.
        /// </summary>
        [AutomaticRetry(Attempts = 3, OnAttemptsExceeded = AttemptsExceededAction.Delete)]
        public virtual async void Send(MailJobMessage mail)
        {
            if (_disposed)
            {
                throw new ObjectDisposedException(GetType().Name);
            }
            if (mail == null)
            {
                throw new ArgumentNullException(nameof(mail));
            }
            var message = ComposeMessage(mail);
            if (message == null)
            {
                throw new InvalidOperationException("The message cannot be null.");
            }

            Smtp.Send(message);

            try
            {
                await LogMessageAsync(mail, CancellationToken.None);
            }
            catch
            {
                // An exception in this method should not requeue the job
            }
        }

        /// <summary>
        /// Composes a new mail message from DB data.
        /// </summary>
        /// <param name="mail">The DB message to convert to a mail message.</param>
        /// <returns>
        /// The mail message.
        /// </returns>
        protected virtual MailMessage ComposeMessage(MailJobMessage mail)
        {
            var message = new MailMessage
            {
                From = new MailAddress(mail.From.Email, mail.From.Name, Encoding.UTF8),
                To = { new MailAddress(mail.To.Email, mail.To.Name, Encoding.UTF8) },
                SubjectEncoding = Encoding.UTF8,
                Subject = mail.Subject
            };

            if (mail.Body != null)
            {
                // Create a simple text view that helps avoid spam filters
                message.AlternateViews.Add(MailTextProvider.GetTextView(mail.Body));
                message.AlternateViews.Add(MailTextProvider.GetHtmlView(mail.Body));
            }

            return message;
        }

        /// <summary>
        /// Logs the given mail message.
        /// </summary>
        /// <param name="mail">The mail message to log.</param>
        /// <param name="cancellationToken">Propagates notification that operations should be canceled.</param>
        /// <returns>
        /// The task object representing the number of the recipients.
        /// </returns>
        protected virtual Task LogMessageAsync(MailJobMessage mail, CancellationToken cancellationToken)
        {
            if (LogManager == null || mail.Users == null || mail.Users.Count == 0)
            {
                return Task.FromResult(0);
            }

            return LogManager.LogAsync(new EventItem
            {
                ObjectType = ObjectType.MailMessage,
                ObjectId = mail.MessageId,
                Contact = new AccountItem { Id = mail.To.Id },
                Project = new ProjectItem { Id = mail.ProjectId }
            },
            mail.Users.Select(id => new AccountItem { Id = id }), cancellationToken);
        }

        /// <summary>
        /// Releases unmanaged and - optionally - managed resources
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// If disposing, calls dispose on the Context. Always nulls out the Context
        /// </summary>
        /// <param name="disposing"><c>true</c> to release both managed and unmanaged resources; <c>false</c> to release only unmanaged resources</param>
        protected virtual void Dispose(bool disposing)
        {
            if (!_disposed)
            {
                Smtp.Dispose();
                _disposed = true;
            }
        }
    }
}
