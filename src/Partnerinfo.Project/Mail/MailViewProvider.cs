// Copyright (c) János Janka. All rights reserved.

using System;
using System.Collections.Immutable;
using System.Net.Mail;
using System.Net.Mime;
using System.Text;
using System.Text.RegularExpressions;

namespace Partnerinfo.Project.Mail
{
    internal sealed class MailViewProvider : IMailViewProvider
    {
        private static readonly Regex s_htmlTagRegex = new Regex(
            "<(?<tag>/?[A-Z0-9-]*).*?>", RegexOptions.IgnoreCase | RegexOptions.Compiled);

        private static readonly ImmutableHashSet<string> s_htmlTags = ImmutableHashSet.Create(
            StringComparer.OrdinalIgnoreCase, "p", "br", "hr", "/td", "/li");

        /// <summary>
        /// An instance of the <see cref="MailViewProvider" /> class.
        /// </summary>
        internal static readonly MailViewProvider Default = new MailViewProvider();

        /// <summary>
        /// Gets the text representation of the given HTML message.
        /// </summary>
        /// <param name="html">The HTML to convert.</param>
        /// <returns>
        /// The text format of the message.
        /// </returns>
        public AlternateView GetTextView(string html)
        {
            if (html == null)
            {
                throw new ArgumentNullException(nameof(html));
            }
            var text = s_htmlTagRegex.Replace(html, match => s_htmlTags.Contains(match.Groups["tag"].Value) ? Environment.NewLine : string.Empty);
            var view = AlternateView.CreateAlternateViewFromString(html, Encoding.UTF8, MediaTypeNames.Text.Plain);
            view.TransferEncoding = TransferEncoding.QuotedPrintable;
            return view;
        }

        /// <summary>
        /// Gets the text representation of the given HTML message.
        /// </summary>
        /// <param name="html">The HTML to convert.</param>
        /// <returns>
        /// The HTML format of the message.
        /// </returns>
        public AlternateView GetHtmlView(string html)
        {
            if (html == null)
            {
                throw new ArgumentNullException(nameof(html));
            }
            var view = AlternateView.CreateAlternateViewFromString(html, Encoding.UTF8, MediaTypeNames.Text.Html);
            view.TransferEncoding = TransferEncoding.QuotedPrintable;
            return view;
        }
    }
}
