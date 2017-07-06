// Copyright (c) János Janka. All rights reserved.

using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Newtonsoft.Json;

namespace Partnerinfo.Identity.OAuth
{
    internal static class OAuthHelpers
    {
        private static readonly string[] s_uriRfc3986CharsToEscape = { "!", "*", "'", "(", ")" };

        /// <summary>
        /// Deserializes a stream to the specified type using JSON.NET.
        /// </summary>
        /// <typeparam name="T">The type of the deserialized object.</typeparam>
        /// <param name="stream">The stream to deserialize.</param>
        /// <returns>
        /// The deserialized object.
        /// </returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2202:Do not dispose objects multiple times")]
        public static T Deserialize<T>(Stream stream)
        {
            if (stream == null)
            {
                throw new ArgumentNullException(nameof(stream));
            }

            var serializer = new JsonSerializer();
            using (var reader = new StreamReader(stream))
            {
                using (var jsonReader = new JsonTextReader(reader))
                {
                    return serializer.Deserialize<T>(jsonReader);
                }
            }
        }

        /// <summary>
        /// Creates a new dictionary with a single data structure.
        /// </summary>
        /// <param name="id">The id.</param>
        /// <param name="email">The email.</param>
        /// <param name="name">The name.</param>
        /// <param name="firstName">The first name.</param>
        /// <param name="lastName">The last name.</param>
        /// <param name="gender">The gender.</param>
        /// <param name="link">The link.</param>
        /// <param name="birthday">The birthday.</param>
        /// <returns></returns>
        public static IDictionary<string, string> CreateResponse(
            string id, string email, string name, string firstName, string lastName, string gender, Uri link, string birthday)
        {
            return new Dictionary<string, string>
            {
                { "id", id },
                { "email", email },
                { "name", name },
                { "firstName", firstName },
                { "lastName", lastName },
                { "link", link == null ? null : link.AbsoluteUri },
                { "gender", gender },
                { "birthday", birthday }
            };
        }

        /// <summary>
        /// Gets the first/last name of the user.
        /// </summary>
        /// <param name="fullName">The full name of the user.</param>
        /// <returns>
        /// The name data.
        /// </returns>
        public static OAuthName ParseName(string fullName)
        {
            string firstName = null, lastName = null;

            if (!string.IsNullOrEmpty(fullName))
            {
                int separator = fullName.LastIndexOf(' ');

                // The index value must be greater than the first character,
                // but it also must be less than the last character.
                // FirstName: [ János Zoltán ], LastName: [ Janka ]
                if (separator > 0 && (separator < fullName.Length - 1))
                {
                    firstName = fullName.Substring(0, separator);
                    lastName = fullName.Substring(separator + 1);
                }
                else
                {
                    // It is just a first name.
                    firstName = fullName;
                }
            }

            return new OAuthName(firstName, lastName);
        }

        /// <summary>
        /// Escapes URI string (RFC3986).
        /// </summary>
        /// <param name="value">The value to escape.</param>
        /// <returns>
        /// The URI.
        /// </returns>
        public static string EscapeUriDataStringRfc3986(string value)
        {
            if (value == null)
            {
                throw new ArgumentNullException(nameof(value));
            }

            var builder = new StringBuilder(Uri.EscapeDataString(value));
            for (int i = 0; i < s_uriRfc3986CharsToEscape.Length; ++i)
            {
                builder.Replace(s_uriRfc3986CharsToEscape[i], Uri.HexEscape(s_uriRfc3986CharsToEscape[i][0]));
            }
            return builder.ToString();
        }
    }
}