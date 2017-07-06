// Copyright (c) János Janka. All rights reserved.

using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;

namespace Partnerinfo
{
    internal static class StringUtility
    {
        internal static readonly ISet<char> DefaultSeparators = new HashSet<char> { '+', '-', '*', '/', ',', ';', ':', '.', '!', '?', '&', '|', '^', '(', ')', '{', '}', '[', ']', '<', '>', '_', '@' };

        public static string Titlify(string value, ISet<char> separators) => Titlify(value, CultureInfo.CurrentCulture, separators);

        public static string Titlify(string value, CultureInfo culture) => Titlify(value, culture, DefaultSeparators);

        public static string Titlify(string value) => Titlify(value, CultureInfo.CurrentCulture, DefaultSeparators);

        public static string Titlify(string text, CultureInfo culture, ISet<char> delimiters)
        {
            if (text == null)
            {
                throw new ArgumentNullException("text");
            }
            if (culture == null)
            {
                throw new ArgumentNullException("culture");
            }
            if (delimiters == null)
            {
                throw new ArgumentNullException("delimiters");
            }

            if (!string.IsNullOrWhiteSpace(text))
            {
                text = text.Trim();
                char pch = '\0';
                char cch;
                var builder = new StringBuilder(128);
                for (int i = 0; i < text.Length; ++i)
                {
                    if (i > 0)
                    {
                        cch = text[i];
                    }
                    else
                    {
                        cch = char.ToUpper(text[i], culture);
                    }
                    if (char.IsWhiteSpace(pch))
                    {
                        if (char.IsWhiteSpace(cch))
                        {
                            continue;
                        }
                        cch = char.ToUpper(cch, culture);
                    }
                    else if (delimiters.Contains(pch))
                    {
                        cch = char.ToUpper(cch, culture);
                    }
                    builder.Append(cch);
                    pch = cch;
                }
                return builder.ToString();
            }
            return string.Empty;
        }
    }
}
