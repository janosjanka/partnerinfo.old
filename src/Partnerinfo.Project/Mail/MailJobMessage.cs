// Copyright (c) János Janka. All rights reserved.

using System.Collections.Generic;

namespace Partnerinfo.Project.Mail
{
    public sealed class MailJobMessage
    {
        /// <summary>
        /// MailMessage ID
        /// </summary>
        public int MessageId { get; set; }

        /// <summary>
        /// Project ID
        /// </summary>
        public int ProjectId { get; set; }

        /// <summary>
        /// A mail address that represents the sender
        /// </summary>
        public MailJobAddress From { get; set; }

        /// <summary>
        /// A mail address that represents the recipient
        /// </summary>
        public MailJobAddress To { get; set; }

        /// <summary>
        /// The subject line
        /// </summary>
        public string Subject { get; set; }

        /// <summary>
        /// The message body
        /// </summary>
        public string Body { get; set; }

        /// <summary>
        /// Users who can be notified
        /// </summary>
        public IList<int> Users { get; set; }
    }
}
