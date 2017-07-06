// Copyright (c) János Janka. All rights reserved.

namespace Partnerinfo.Identity.OAuth
{
    public struct OAuthName
    {
        private readonly string _firstName;
        private readonly string _lastName;

        /// <summary>
        /// Initializes a new instance of the <see cref="OAuthName"/> struct.
        /// </summary>
        /// <param name="firstName">The first name of the user.</param>
        /// <param name="lastName">The last name of the user.</param>
        public OAuthName(string firstName, string lastName)
        {
            _firstName = firstName;
            _lastName = lastName;
        }

        /// <summary>
        /// Gets the full name of the user.
        /// </summary>
        /// <value>
        /// The full name of the user.
        /// </value>
        public string FullName { get { return string.Join(" ", _firstName, _lastName); } }

        /// <summary>
        /// Gets the first name of the user.
        /// </summary>
        /// <value>
        /// The first name of the user.
        /// </value>
        public string FirstName { get { return _firstName; } }

        /// <summary>
        /// Gets the last name of the user.
        /// </summary>
        /// <value>
        /// The last name of the user.
        /// </value>
        public string LastName { get { return _lastName; } }
    }
}