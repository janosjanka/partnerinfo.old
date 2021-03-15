// Copyright (c) János Janka. All rights reserved.

using System.Collections.Generic;

namespace Partnerinfo.Project
{
    public class MailMessageHeader
    {
        /// <summary>
        /// Gets or sets the sender of the <see cref="MailMessageItem" />.
        /// </summary>
        /// <value>
        /// The sender of the <see cref="MailMessageItem" />.
        /// </value>
        public ContactItem From { get; set; }

        /// <summary>
        /// Gets the address collection that contains the recipients of this mail message.
        /// </summary>
        /// <value>
        /// The address collection that contains the recipients of this mail message.
        /// </value>
        public ICollection<int> To { get; } = new HashSet<int>();

        /// <summary>
        /// Gets a collection of <see cref="BusinessTagItem" /> identifiers to be included recipients associated with these tags.
        /// </summary>
        /// <value>
        /// A collection of <see cref="BusinessTagItem" /> identifiers.
        /// </value>
        public ICollection<int> IncludeWithTags { get; } = new HashSet<int>();

        /// <summary>
        /// Gets a collection of <see cref="BusinessTagItem" /> identifiers to be excluded recipients associated with these tags.
        /// </summary>
        /// <value>
        /// A collection of <see cref="BusinessTagItem" /> identifiers.
        /// </value>
        public ICollection<int> ExcludeWithTags { get; } = new HashSet<int>();

        /// <summary>
        /// Gets a collection of key/value pairs that can be substituted.
        /// </summary>
        /// <value>
        /// A collection of key/value pairs.
        /// </value>
        public PropertyDictionary Placeholders { get; } = new PropertyDictionary();
    }
}
