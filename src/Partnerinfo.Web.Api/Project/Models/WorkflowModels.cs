// Copyright (c) János Janka. All rights reserved.

using System;

namespace Partnerinfo.Project.Models
{
    public sealed class WorkflowDto
    {
        /// <summary>
        /// Gets or sets the custom URI.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Contact data that belongs to the form record
        /// </summary>
        public WorkflowContactDto Contact { get; set; }

        /// <summary>
        /// Invitation model
        /// </summary>
        public WorkflowInvitationDto Invitation { get; set; }
    }

    public sealed class WorkflowInvitationDto
    {
        /// <summary>
        /// A task object that represents a business task
        /// </summary>
        public string Action { get; set; }

        /// <summary>
        /// A message that will be forwarded to the contacts below
        /// </summary>
        public string Message { get; set; }

        /// <summary>
        /// A collection of contacts who will be invited
        /// </summary>
        public WorkflowContactDto[] To { get; set; }
    }

    public sealed class WorkflowEmailDto
    {
        /// <summary>
        /// Email 
        /// </summary>
        [Obsolete]
        public string Email { get; set; }

        /// <summary>
        /// Address
        /// </summary>
        public string Address { get; set; }

        /// <summary>
        /// Name
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Converts this model to a <see cref="MailAddressItem" /> type.
        /// </summary>
        /// <returns>
        /// The mail address.
        /// </returns>
        public MailAddressItem ToMailAddress()
        {
            return MailAddressItem.Create(Address ?? Email, Name);
        }
    }

    public sealed class WorkflowContactDto
    {
        /// <summary>
        /// Sponsor Id
        /// </summary>
        public int? SponsorId { get; set; }

        /// <summary>
        /// Gets or sets the facebook identifier.
        /// </summary>
        /// <value>
        /// The facebook identifier.
        /// </value>
        public long? FacebookId { get; set; }

        /// <summary>
        /// Email
        /// </summary>
        public WorkflowEmailDto Email { get; set; }

        /// <summary>
        /// First name
        /// </summary>
        public string FirstName { get; set; }

        /// <summary>
        /// Last name
        /// </summary>
        public string LastName { get; set; }

        /// <summary>
        /// Person gender ( male | female )
        /// </summary>
        public PersonGender Gender { get; set; }

        /// <summary>
        /// Date when this user was born
        /// </summary>
        public DateTime? Birthday { get; set; }

        /// <summary>
        /// Phone numbers
        /// </summary>
        public PhoneGroupItem Phones { get; set; }

        /// <summary>
        /// Extra comment
        /// </summary>
        public string Comment { get; set; }

        /// <summary>
        /// Converts this binding model to a contact entity.
        /// </summary>
        /// <returns>The contact.</returns>
        public ContactItem ToContact()
        {
            return new ContactItem
            {
                Sponsor = SponsorId != null ? new ContactItem { Id = (int)SponsorId } : null,
                FacebookId = FacebookId,
                Email = Email?.ToMailAddress(),
                FirstName = FirstName,
                LastName = LastName,
                Gender = Gender,
                Birthday = Birthday,
                Phones = Phones,
                Comment = Comment
            };
        }
    }

    public sealed class WorkflowIdentityResultDto
    {
        /// <summary>
        /// Authorization token
        /// </summary>
        public string AccessToken { get; set; }

        /// <summary>
        /// Token type (Bearer)
        /// </summary>
        public string TokenType { get; set; }

        /// <summary>
        /// Authenticated contact
        /// </summary>
        public AuthTicket Identity { get; set; }
    }
}