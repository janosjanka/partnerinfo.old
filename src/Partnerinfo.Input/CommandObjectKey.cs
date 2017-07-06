// Copyright (c) János Janka. All rights reserved.

using System;

namespace Partnerinfo.Input
{
    public class CommandObjectKey
    {
        private string _type1;
        private string _type2;

        /// <summary>
        /// Initializes a new instance of the <see cref="CommandObjectKey" /> class.
        /// </summary>
        /// <param name="type1">Primary type name.</param>
        public CommandObjectKey(string type1)
            : this(type1, null)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="CommandObjectKey" /> class.
        /// </summary>
        /// <param name="type1">Primary type name.</param>
        /// <param name="type2">Secondary type name.</param>
        public CommandObjectKey(string type1, string type2)
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
        public string Type1
        {
            get
            {
                return _type1;
            }
            private set
            {
                if (string.IsNullOrEmpty(value))
                {
                    throw new InvalidOperationException("The primary type cannot be null.");
                }
                _type1 = value.ToUpper();
            }
        }

        /// <summary>
        /// Secondary type name
        /// </summary>
        public string Type2
        {
            get
            {
                return _type2;
            }
            set
            {
                _type2 = value?.ToUpper();
            }
        }

        /// <summary>
        /// Returns the hash code for this key.
        /// </summary>
        /// <returns>
        /// The hash code.
        /// </returns>
        public override int GetHashCode()
        {
            return Type1.GetHashCode() ^ (Type2 ?? string.Empty).GetHashCode();
        }

        /// <summary>
        /// Determines whether the specified object is equal to the current object.
        /// </summary>
        /// <param name="obj">The object to compare with the current object.</param>
        /// <returns>
        /// <c>true</c> if the specified object is equal to the current object; otherwise, <c>false</c>.
        /// </returns>
        public override bool Equals(object obj)
        {
            if (ReferenceEquals(obj, this))
            {
                return true;
            }
            var otherKey = obj as CommandObjectKey;
            if (otherKey == null)
            {
                return false;
            }
            return string.Equals(Type1, otherKey.Type1, StringComparison.Ordinal)
                && string.Equals(Type2, otherKey.Type2, StringComparison.Ordinal);
        }
    }
}
