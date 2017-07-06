// Copyright (c) János Janka. All rights reserved.

using System.Threading;
using System.Threading.Tasks;

namespace Partnerinfo.Input
{
    public interface ICommandMailService
    {
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
        Task SendAsync(MailAddressItem to, CommandItem command, string returnUrl, CancellationToken cancellationToken);
    }
}
