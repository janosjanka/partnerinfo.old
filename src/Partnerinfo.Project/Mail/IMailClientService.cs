// Copyright (c) János Janka. All rights reserved.

using System.Threading;
using System.Threading.Tasks;

namespace Partnerinfo.Project.Mail
{
    public interface IMailClientService
    {
        /// <summary>
        /// Enqueues emails asynchronously.
        /// </summary>
        /// <param name="project">The project that contains the given contacts.</param>
        /// <param name="mailHeader">The mail header that defines contacts and filter criteria.</param>
        /// <param name="mailMessage">The mail message to send.</param>
        /// <param name="cancellationToken">Propagates notification that operations should be canceled.</param>
        /// <returns>
        /// The task object representing the number of the recipients.
        /// </returns>
        Task<ValidationResult> SendAsync(ProjectItem project, MailMessageHeader mailHeader, MailMessageItem mailMessage, CancellationToken cancellationToken);
    }
}
