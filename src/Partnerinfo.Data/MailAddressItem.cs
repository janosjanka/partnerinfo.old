// Copyright (c) János Janka. All rights reserved.

using System.Runtime.Serialization;

namespace Partnerinfo
{
    /// <summary>
    /// Represents an immutable, cacheable, thread-safe, and two-way serializable mail address of a user.
    /// </summary>
    [DataContract]
    public class MailAddressItem
    {
        /// <summary>
        /// A <see cref="MailAddressItem" /> instance.
        /// </summary>
        public static readonly MailAddressItem None = new MailAddressItem();

        /// <summary>
        /// Gets the e-mail address specified when this <see cref="MailAddressItem" /> was created.
        /// </summary>
        /// <value>
        /// The address.
        /// </value>
        [DataMember]
        public string Address { get; private set; }

        /// <summary>
        /// Gets the name specified when this <see cref="MailAddressItem" /> was created.
        /// </summary>
        /// <value>
        /// The name.
        /// </value>
        [DataMember]
        public string Name { get; private set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="MailAddressItem" /> class.
        /// </summary>
        internal MailAddressItem()
        {
            // An empty constructor is required for supporting serializers.
            // Also, C# allows you to initialize properties without a "private setter" in the constructor,
            // but the most of the MS serializers will crash even if you just want to serialize this type to a string.
            // JSON.NET is smarter, but we do not want a class library to be tied to a special serialization mechanism.
            // Plus, JSON.NET deserialization also does not work if you do not add a DataMember or JsonProperty attribute to this kind of properties.
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="MailAddressItem" /> class.
        /// </summary>
        /// <param name="address">A <see cref="String" /> that contains an e-mail address. This parameter can be null.</param>
        /// <param name="name">A <see cref="String" /> that contains the name associated with address. This parameter can be null.</param>
        internal MailAddressItem(string address, string name)
        {
            Address = address;
            Name = name;
        }

        /// <summary>
        /// Creates a new instance of the <see cref="MailAddressItem" /> class or returns a cached version of the immutable object.
        /// </summary>
        /// <param name="address">A <see cref="String" /> that contains an e-mail address. This parameter can be null.</param>
        /// <returns>
        /// The <see cref="MailAddressItem" />.
        /// </returns>
        public static MailAddressItem Create(string address)
        {
            if (address == null)
            {
                return None;
            }
            return new MailAddressItem(address, null);
        }

        /// <summary>
        /// Creates a new instance of the <see cref="MailAddressItem" /> class or returns a cached version of the immutable object.
        /// </summary>
        /// <param name="address">A <see cref="String" /> that contains an e-mail address. This parameter can be null.</param>
        /// <param name="name">A <see cref="String" /> that contains the name associated with address. This parameter can be null.</param>
        /// <returns>
        /// The <see cref="MailAddressItem" />.
        /// </returns>
        public static MailAddressItem Create(string address, string name)
        {
            if (address == null && name == null)
            {
                return None;
            }
            return new MailAddressItem(address, name);
        }
    }
}
