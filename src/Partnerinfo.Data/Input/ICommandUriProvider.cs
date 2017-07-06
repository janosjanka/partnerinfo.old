// Copyright (c) János Janka. All rights reserved.

namespace Partnerinfo.Input
{
    public interface ICommandUriProvider
    {
        /// <summary>
        /// Generates a valid URL from the given URL.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns>
        /// The generated URI.
        /// </returns>
        string Generate(string value);
    }
}
