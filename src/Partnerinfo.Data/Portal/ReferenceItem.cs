// Copyright (c) János Janka. All rights reserved.

using System.Runtime.Serialization;

namespace Partnerinfo.Portal
{
    /// <summary>
    /// Represents an immutable, thread-safe reference to a web resource. This type supports two-way serialization.
    /// </summary>
    [DataContract]
    public class ReferenceItem
    {
        /// <summary>
        /// Gets or sets the mime type for this <see cref="ReferenceItem" />.
        /// </summary>
        /// <value>
        /// The type for this <see cref="ReferenceItem" />.
        /// </value>
        [DataMember]
        public string Type { get; private set; }

        /// <summary>
        /// Gets or sets the media URI for this <see cref="ReferenceItem" />.
        /// </summary>
        /// <value>
        /// The media URI for this <see cref="ReferenceItem" />.
        /// </value>
        [DataMember]
        public string Uri { get; private set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="ReferenceItem" /> class.
        /// </summary>
        internal ReferenceItem()
        {
            // An empty constructor is required for supporting serializers.
            // Also, C# allows you to initialize properties without a "private setter" in the constructor,
            // but the most of the MS serializers will crash even if you just want to serialize this type to a string.
            // JSON.NET is smarter, but we do not want a class library to be tied to a special serialization mechanism.
            // Plus, JSON.NET deserialization also does not work if you do not add a DataMember or JsonProperty attribute to this kind of properties.
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ReferenceItem" /> class.
        /// </summary>
        /// <param name="type">The media type to set.</param>
        /// <param name="uri">The media URI to set.</param>
        internal ReferenceItem(string type, string uri)
        {
            Type = type;
            Uri = uri;
        }

        /// <summary>
        /// Creates a new instance of the <see cref="ReferenceItem" /> class or returns a cached version of the immutable object.
        /// </summary>
        /// <param name="type">A <see cref="String" /> that contains the mime type associated with this reference. This parameter can be null.</param>
        /// <param name="uri">A <see cref="String" /> that contains the media URI associated with this reference. This parameter can be null.</param>
        /// <returns>
        /// The <see cref="ReferenceItem" />.
        /// </returns>
        public static ReferenceItem Create(string type, string uri) => new ReferenceItem(type, uri);
    }
}
