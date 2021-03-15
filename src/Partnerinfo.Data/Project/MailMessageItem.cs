// Copyright (c) János Janka. All rights reserved.

using System;

namespace Partnerinfo.Project
{
    public class MailMessageItem : UniqueItem
    {
        /// <summary>
        /// Gets or sets the project which owns this mail message.
        /// </summary>
        /// <value>
        /// The project which owns this mail message.
        /// </value>
        public UniqueItem Project { get; set; }

        /// <summary>
        /// Gets or sets the subject line of this <see cref="MailMessageItem" />.
        /// </summary>
        /// <value>
        /// The subject line.
        /// </value>
        public string Subject { get; set; }

        /// <summary>
        /// Gets or sets the body of this <see cref="MailMessageItem" />.
        /// </summary>
        /// <value>
        /// The body.
        /// </value>
        public string Body { get; set; }

        /// <summary>
        /// Gets or sets the date and time, in UTC, when this <see cref="MailMessageItem" /> was last modified.
        /// </summary>
        /// <value>
        /// The date and time, in UTC, when this <see cref="MailMessageItem" /> was last modified.
        /// </value>
        public DateTime ModifiedDate { get; set; } = DateTime.UtcNow;
    }
}
