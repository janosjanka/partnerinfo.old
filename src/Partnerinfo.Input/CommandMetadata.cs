// Copyright (c) János Janka. All rights reserved.

using System;

namespace Partnerinfo.Input
{
    public class CommandMetadata
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="CommandMetadata" /> class.
        /// </summary>
        /// <param name="type1">Primary type name.</param>
        public CommandMetadata(string type1)
            : this(type1, null)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="CommandMetadata" /> class.
        /// </summary>
        /// <param name="type1">Primary type name.</param>
        public CommandMetadata(string type1, string type2)
        {
            if (type1 == null)
            {
                throw new ArgumentNullException("type1");
            }
            Type1 = type1;
            Type2 = type2;
        }

        /// <summary>
        /// Primary type name
        /// </summary>
        public string Type1 { get; private set; }

        /// <summary>
        /// Secondary type name
        /// </summary>
        public string Type2 { get; set; }
    }
}
