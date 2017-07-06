// Copyright (c) János Janka. All rights reserved.

using System;
using System.Runtime.Serialization;

namespace Partnerinfo
{
    /// <summary>
    /// Represents an immutable (thread-safe) and two-way serializable authentication ticket.
    /// </summary>
    [DataContract]
    public sealed class AuthTicket
    {
        /// <summary>
        /// User ID (Primary Key)
        /// </summary>
        [DataMember]
        public int Id { get; private set; }

        /// <summary>
        /// Facebook UserId
        /// </summary>
        [DataMember]
        public long? FacebookId { get; private set; }

        /// <summary>
        /// Email Address
        /// </summary>
        [DataMember]
        public MailAddressItem Email { get; private set; }

        /// <summary>
        /// First name
        /// </summary>
        [DataMember]
        public string FirstName { get; private set; }

        /// <summary>
        /// Last name
        /// </summary>
        [DataMember]
        public string LastName { get; private set; }

        /// <summary>
        /// Nick name
        /// </summary>
        [DataMember]
        public string NickName { get; private set; }

        /// <summary>
        /// Person gender ( male | female )
        /// </summary>
        [DataMember]
        public PersonGender Gender { get; private set; }

        /// <summary>
        /// Date when this user was born
        /// </summary>
        [DataMember]
        public DateTime? Birthday { get; private set; }

        /// <summary>
        /// Phones
        /// </summary>
        [DataMember]
        public PhoneGroupItem Phones { get; private set; }

        /// <summary>
        /// Prevents a default instance of the <see cref="AuthTicket" /> class from being created.
        /// </summary>
        internal AuthTicket()
        {
        }

        internal AuthTicket(
            int id,
            long? facebookId,
            MailAddressItem email,
            string firstName,
            string lastName,
            string nickName,
            PersonGender gender,
            DateTime? birthday,
            PhoneGroupItem phones)
        {
            Id = id;
            FacebookId = facebookId;
            Email = email;
            FirstName = firstName;
            LastName = lastName;
            NickName = nickName;
            Gender = gender;
            Birthday = birthday;
            Phones = phones;
        }
    }
}