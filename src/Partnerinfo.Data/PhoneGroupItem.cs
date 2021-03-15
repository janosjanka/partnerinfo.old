// Copyright (c) János Janka. All rights reserved.

using System.Runtime.Serialization;

namespace Partnerinfo
{
    /// <summary>
    /// Represents an immutable (thread-safe) and two-way serializable phones of a user.
    /// </summary>
    [DataContract]
    public class PhoneGroupItem
    {
        /// <summary>
        /// Represents an empty <see cref="PhoneGroupItem" /> instance.
        /// </summary>
        public static readonly PhoneGroupItem Empty = new PhoneGroupItem();

        /// <summary>
        /// Personal phone
        /// </summary>
        [DataMember]
        public string Personal { get; private set; }

        /// <summary>
        /// Business phone
        /// </summary>
        [DataMember]
        public string Business { get; private set; }

        /// <summary>
        /// Mobile phone
        /// </summary>
        [DataMember]
        public string Mobile { get; private set; }

        /// <summary>
        /// Other phone
        /// </summary>
        [DataMember]
        public string Other { get; private set; }

        /// <summary>
        /// Prevents a default instance of the <see cref="PhoneGroupItem" /> class from being created.
        /// </summary>
        private PhoneGroupItem()
        {
        }

        /// <summary>
        /// Creates a new instance of the <see cref="PhoneGroupItem" /> class or returns a cached version of the immutable object.
        /// </summary>
        /// <param name="personal">Personal phone.</param>
        /// <param name="business">Business phone.</param>
        /// <param name="mobile">Mobile phone.</param>
        /// <param name="other">Other phone.</param>
        /// <returns>
        /// The <see cref="PhoneGroupItem" />.
        /// </returns>
        public static PhoneGroupItem Create(string personal, string business, string mobile, string other)
        {
            if (personal == null && business == null && mobile == null && other == null)
            {
                return Empty;
            }
            return new PhoneGroupItem { Personal = personal, Business = business, Mobile = mobile, Other = other };
        }
    }
}
