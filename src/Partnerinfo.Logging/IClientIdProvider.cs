// Copyright (c) János Janka. All rights reserved.

namespace Partnerinfo.Logging
{
    public interface IClientIdProvider
    {
        /// <summary>
        /// Generates a Client ID from the given Client ID.
        /// </summary>
        /// <param name="clientId">The client ID.</param>
        /// <returns>
        /// The generated Client ID.
        /// </returns>
        string Generate(string clientId);
    }
}
