// Copyright (c) János Janka. All rights reserved.

namespace Partnerinfo.Project.Mail
{
    public sealed class MailJobAddress
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="MailJobAddress" /> class.
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <param name="email">The email address.</param>
        /// <param name="name">The name.</param>
        public MailJobAddress(int id, string email, string name)
        {
            Id = id;
            Email = email;
            Name = name;
        }

        /// <summary>
        /// Gets or sets the primary key for this <see cref="MailJobAddress" />.
        /// </summary>
        /// <value>
        /// The primary key.
        /// </value>
        public int Id { get; }

        /// <summary>
        /// Gets or sets the email address for the <see cref="MailJobAddress" />.
        /// </summary>
        /// <value>
        /// The email address.
        /// </value>
        public string Email { get; }

        /// <summary>
        /// Gets or sets the name for the <see cref="MailJobAddress" />.
        /// </summary>
        /// <value>
        /// The name.
        /// </value>
        public string Name { get; }
    }
}
