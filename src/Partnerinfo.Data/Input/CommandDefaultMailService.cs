// Copyright (c) János Janka. All rights reserved.

using System;
using System.Net.Mail;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using Partnerinfo.Properties;

namespace Partnerinfo.Input
{
    public class CommandDefaultMailService : ICommandMailService
    {
        public static readonly CommandDefaultMailService Default = new CommandDefaultMailService();

        /// <summary>
        /// Initializes a new instance of the <see cref="CommandDefaultMailService" /> class.
        /// </summary>
        public CommandDefaultMailService()
        {
            HtmlBody = CommandResources.MailService_HtmlBody;
#if DEBUG
            RouteLink = string.Format("http://{0}/c", Settings.Default.AppHost);
#else
            RouteLink = string.Format("http://www.{0}/c", Settings.Default.AppHost);
#endif
        }

        /// <summary>
        /// Html content of the mail
        /// </summary>
        public string HtmlBody { get; set; }

        /// <summary>
        /// A link to commit the transaction
        /// </summary>
        public string RouteLink { get; set; }

        /// <summary>
        /// Sends a command in email to the given recipient asynchronously.
        /// </summary>
        /// <param name="to">The recipient.</param>
        /// <param name="command">The command to send.</param>
        /// <param name="returnUrl">Return URL.</param>
        /// <param name="cancellationToken">Propagates notification that operations should be canceled.</param>
        /// <returns>
        /// The task object representing the number of the recipients.
        /// </returns>
        public virtual async Task SendAsync(MailAddressItem to, CommandItem command, string returnUrl, CancellationToken cancellationToken)
        {
            if (to == null)
            {
                throw new ArgumentNullException("to");
            }
            if (command == null)
            {
                throw new ArgumentNullException("command");
            }
            var message = ComposeMessage(to, command, returnUrl);
            if (message == null)
            {
                throw new InvalidOperationException("The command object cannot be null.");
            }
            using (var smtpClient = new SmtpClient())
            {
                await smtpClient.SendMailAsync(message);
            }
        }

        /// <summary>
        /// Composes a new mail message from DB data.
        /// </summary>
        /// <param name="mail">The DB message to convert to a mail message.</param>
        /// <returns>
        /// The mail message.
        /// </returns>
        protected virtual MailMessage ComposeMessage(MailAddressItem to, CommandItem command, string returnUrl)
        {
            var routeLink = string.Join("/", RouteLink, command.Uri);
            var commitLink = UriUtility.MakeUri(routeLink, UriKind.Absolute, new UriParameter("returnurl", returnUrl));
            var rollbackLink = UriUtility.MakeUri(routeLink, UriKind.Absolute, new UriParameter("rollback", "true"), new UriParameter("returnurl", returnUrl));
            var message = new MailMessage
            {
                SubjectEncoding = Encoding.UTF8,
                Subject = command.Line,
                BodyEncoding = Encoding.UTF8,
                Body = string.Format(HtmlBody, commitLink, rollbackLink),
                IsBodyHtml = true
            };
            message.To.Add(new MailAddress(to.Address, to.Name, Encoding.UTF8));
            return message;
        }
    }
}
