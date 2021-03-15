// Copyright (c) János Janka. All rights reserved.

using System;
using System.ComponentModel.DataAnnotations;

namespace Partnerinfo
{
    public class AccountItem : UniqueItem
    {
        /// <summary>
        /// Gets or sets the facebook identifier for this contact.
        /// </summary>
        /// <value>
        /// The facebook identifier for this contact.
        /// </value>
        public long? FacebookId { get; set; }

        /// <summary>
        /// Gets or sets the email address of this <see cref="AccountItem" />.
        /// </summary>
        /// <value>
        /// The email address.
        /// </value>
        [TemplateField("Address", Name = "TemplateField_Email_Address", ResourceType = typeof(Resources))]
        [TemplateField("Name", Name = "TemplateField_Email_Name", ResourceType = typeof(Resources))]
        public MailAddressItem Email { get; set; }

        /// <summary>
        /// Gets or sets the first name of this <see cref="AccountItem" />.
        /// </summary>
        /// <value>
        /// The first name.
        /// </value>
        [TemplateField(Name = "TemplateField_FirstName", ResourceType = typeof(Resources))]
        public string FirstName { get; set; }

        /// <summary>
        /// Gets or sets the last name of this <see cref="AccountItem" />.
        /// </summary>
        /// <value>
        /// The last name.
        /// </value>
        [TemplateField(Name = "TemplateField_LastName", ResourceType = typeof(Resources))]
        public string LastName { get; set; }

        /// <summary>
        /// Gets or sets the nick name of this <see cref="AccountItem" />.
        /// </summary>
        /// <value>
        /// The nick name.
        /// </value>
        [TemplateField(Name = "TemplateField_NickName", ResourceType = typeof(Resources))]
        public string NickName { get; set; }

        /// <summary>
        /// Person gender ( male | female )
        /// </summary>
        /// <value>
        /// The gender.
        /// </value>
        [TemplateField(Name = "TemplateField_Gender", ResourceType = typeof(Resources))]
        public PersonGender Gender { get; set; }

        /// <summary>
        /// Date when this user was born
        /// </summary>
        /// <value>
        /// The birthday.
        /// </value>
        [TemplateField(Name = "TemplateField_Birthday", ResourceType = typeof(Resources))]
        public DateTime? Birthday { get; set; }
    }
}
