// Copyright (c) János Janka. All rights reserved.

namespace Partnerinfo.Input
{
    public interface ICommandParser
    {
        /// <summary>
        /// Parses the given text.
        /// </summary>
        /// <param name="commandText">The command text to parse.</param>
        /// <param name="htmlContent">The html content to parse.</param>
        /// <param name="textContent">The text content to parse.</param>
        /// <returns>
        /// A <see cref="Command" /> object.
        /// </returns>
        Command Parse(string commandText, string htmlContent = null, string textContent = null);
    }
}
