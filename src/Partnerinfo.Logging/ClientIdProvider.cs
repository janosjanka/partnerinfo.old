// Copyright (c) János Janka. All rights reserved.

using System.Text;

namespace Partnerinfo.Logging
{
    public class ClientIdProvider : IClientIdProvider
    {
        public static readonly IClientIdProvider Default = new ClientIdProvider();

        /// <summary>
        /// Generates a Client ID from the given Client ID.
        /// </summary>
        /// <param name="clientId">The client ID.</param>
        /// <returns>
        /// The generated Client ID.
        /// </returns>
        public virtual string Generate(string clientId)
        {
            if (string.IsNullOrEmpty(clientId))
            {
                return null;
            }

            // This is a simple generator that reverses the string and
            // removes all the IP separator characters (ipV4 = . / ipv6 = :)
            var strBuilder = new StringBuilder();
            for (int i = clientId.Length; --i >= 0;)
            {
                char c = clientId[i];
                if (c != '.' && c != ':')
                {
                    strBuilder.Append(c);
                }
            }
            return strBuilder.ToString();
        }
    }
}
