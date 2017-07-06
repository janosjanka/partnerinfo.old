// Copyright (c) János Janka. All rights reserved.

using System.Net.Mail;

namespace Partnerinfo.Project.Mail
{
    public interface IMailViewProvider
    {
        /// <summary>
        /// Gets the text representation of the given HTML message.
        /// </summary>
        /// <param name="html">The HTML to convert.</param>
        /// <returns>
        /// The text format of the message.
        /// </returns>
        AlternateView GetTextView(string html);

        /// <summary>
        /// Gets the text representation of the given HTML message.
        /// </summary>
        /// <param name="html">The HTML to convert.</param>
        /// <returns>
        /// The HTML format of the message.
        /// </returns>
        AlternateView GetHtmlView(string html);
    }
}
