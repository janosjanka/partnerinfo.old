﻿// Copyright (c) János Janka. All rights reserved.

using System;
using System.Globalization;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;

namespace Partnerinfo.Project.Actions
{
    internal static class ActionLinkHelper
    {
        private const char RootPrefixChar = 'a';
        private const char ValueSeparator = '.';
        private static readonly string s_routePrefix = $"{RootPrefixChar}{ValueSeparator}";

        /// <summary>
        /// A regular expression that helps deserialize link parameters.
        /// <![CDATA[ a\.(?<crc>(\d+))\.(?<action>(\d+))\.?(?<contact>(\d+)?)\/?(?<uri>([-\._~a-zA-Z0-9]+)?) ]]>
        /// </summary>
        private static readonly Regex s_scannerRegex = new Regex(
            $"{RootPrefixChar}\\{ValueSeparator}(?<checksum>(\\d+))\\{ValueSeparator}(?<actionId>(\\d+))\\{ValueSeparator}?(?<contactId>(\\d+)?)/?(?<customUri>([-\\._~a-zA-Z0-9]+)?)",
            RegexOptions.Singleline | RegexOptions.ExplicitCapture | RegexOptions.Compiled);

        /// <summary>
        /// An internal value that is used to generate a CRC checksum.
        /// </summary>
        private static readonly byte[] s_salt = { 66, 75, 54, 48, 49, 50, 48, 49, 74, 74, 53, 57, 48, 53, 48, 53 };

        /// <summary>
        /// Creates a new action link parameters that can be shared on the Web.
        /// </summary>
        /// <param name="actionLink">An object that configures the action link will be created.</param>
        /// <returns>
        /// The link.
        /// </returns>
        public static string UrlTokenEncode(ActionLink actionLink)
        {
            if (actionLink == null)
            {
                throw new ArgumentNullException(nameof(actionLink));
            }

            var customUri = actionLink.CustomUri != null ? UriUtility.Normalize(actionLink.CustomUri) : default(string);
            var builder = new StringBuilder();

            builder.Append(GetChecksum(actionLink.ActionId, actionLink.ContactId, customUri));
            builder.Append(ValueSeparator);
            builder.Append(actionLink.ActionId);

            if (actionLink.ContactId != null)
            {
                builder.Append(ValueSeparator);
                builder.Append(actionLink.ContactId);
            }

            if (actionLink.CustomUri != null)
            {
                builder.Append('/');
                builder.Append(customUri);
            }

            return builder.ToString();
        }

        /// <summary>
        /// Parses the given string parameters and returns with a <see cref="ActionLink" /> object.
        /// </summary>
        /// <param name="paramUri">The string that contains entity ID(s).</param>
        /// <param name="customUri">The user-defined URI for the action link.</param>
        /// <returns>
        /// A <see cref="ActionLink" /> object.
        /// </returns>
        public static ActionLink UrlTokenDecode(string paramUri, string customUri = null)
        {
            if (string.IsNullOrEmpty(paramUri))
            {
                throw new ArgumentNullException($"The {nameof(paramUri)} cannot be null or empty.");
            }

            var slots = paramUri.Split(new char[] { ValueSeparator }, 3, StringSplitOptions.None);
            var checksum = GetUIntAt(slots, 0);
            var actionId = GetSIntAt(slots, 1);
            if (checksum == null || actionId == null)
            {
                throw new InvalidOperationException(string.Format(CultureInfo.CurrentCulture, ActionLinkResources.InvalidLinkParameter, paramUri));
            }
            var contactId = GetSIntAt(slots, 2);
            if (checksum != GetChecksum((int)actionId, contactId, customUri))
            {
                throw new InvalidOperationException(string.Format(CultureInfo.CurrentCulture, ActionLinkResources.InvalidLinkParameter, paramUri));
            }
            return new ActionLink((int)actionId, contactId, customUri);
        }

        /// <summary>
        /// Creates a new action link that can be shared on the Web.
        /// </summary>
        /// <param name="actionLink">An object that configures the action link will be created.</param>
        /// <param name="absolute">It will create an absolute URL if true.</param>
        /// <returns>
        /// The link.
        /// </returns>
        public static string CreateLink(ActionLink actionLink, bool absolute)
        {
            var uri = new Uri(ServerPaths.BaseUri, s_routePrefix + UrlTokenEncode(actionLink));
            return absolute ? uri.AbsoluteUri : uri.AbsolutePath;
        }

        /// <summary>
        /// Parses the given action link and returns with a <see cref="ActionLink" /> object.
        /// </summary>
        /// <param name="link">The action link.</param>
        /// <returns>
        /// A <see cref="ActionLink" /> object.
        /// </returns>
        public static ActionLink DecodeLink(string link)
        {
            if (string.IsNullOrEmpty(link))
            {
                throw new ArgumentNullException($"The {nameof(link)} cannot be null or empty.");
            }

            var uri = new Uri(ServerPaths.BaseUri, link);
            link = uri.AbsolutePath;

            if (link.StartsWith("/" + s_routePrefix, StringComparison.OrdinalIgnoreCase))
            {
                link = link.Substring(s_routePrefix.Length + 1);
            }

            var parts = link.Split(new[] { '/' }, StringSplitOptions.RemoveEmptyEntries);
            if (parts.Length == 0)
            {
                throw new InvalidOperationException(string.Format(CultureInfo.CurrentCulture, ActionLinkResources.InvalidLinkParameter, link));
            }

            return UrlTokenDecode(parts[0], parts.Length > 1 ? parts[1] : null);
        }

        /// <summary>
        /// In a specified input string, replaces each link parameter with another link parameter returned by a callback function.
        /// </summary>
        /// <param name="input">The string to search for a match.</param>
        /// <param name="callback">
        /// A custom method that examines each match and returns either the original matched link parameter or a replacement link parameter.
        /// </param>
        public static string ReplaceLinks(string input, Action<ActionLink> callback)
        {
            if (input == null)
            {
                throw new ArgumentNullException(nameof(input));
            }
            if (callback == null)
            {
                throw new ArgumentNullException(nameof(callback));
            }

            return s_scannerRegex.Replace(input, match =>
            {
                int actionId, contactId;
                if (!int.TryParse(match.Groups["actionId"].Value, out actionId))
                {
                    return match.Value;
                }
                var linkParams = new ActionLink
                {
                    ActionId = actionId,
                    CustomUri = match.Groups["customUri"]?.Value
                };
                if (int.TryParse(match.Groups["contactId"]?.Value, out contactId))
                {
                    linkParams.ContactId = contactId;
                }
                callback(linkParams);
                return s_routePrefix + UrlTokenEncode(linkParams);
            });
        }

        /// <summary>
        /// Gets the CRC checksum for the current parameter.
        /// </summary>
        /// <returns>
        /// The checksum.
        /// </returns>
        private static uint GetChecksum(int actionId, int? contactId, string customUri)
        {
            byte[] actBytes = BitConverter.GetBytes(actionId);
            byte[] conBytes = contactId != null ? BitConverter.GetBytes((int)contactId) : new byte[0];
            byte[] uriBytes = customUri != null ? Encoding.ASCII.GetBytes(customUri) : new byte[0];
            byte[] retBytes = new byte[actBytes.Length + conBytes.Length + uriBytes.Length + s_salt.Length];

            Buffer.BlockCopy(actBytes, 0, retBytes, 0, actBytes.Length);
            if (conBytes.Length > 0)
            {
                Buffer.BlockCopy(conBytes, 0, retBytes, actBytes.Length, conBytes.Length);
            }
            if (uriBytes.Length > 0)
            {
                Buffer.BlockCopy(uriBytes, 0, retBytes, actBytes.Length + conBytes.Length, uriBytes.Length);
            }
            Buffer.BlockCopy(s_salt, 0, retBytes, actBytes.Length + conBytes.Length + uriBytes.Length, s_salt.Length);

            return Crc32.Compute(retBytes);
        }

        /// <summary>
        /// Returns the number represented by the given text.
        /// </summary>
        /// <param name="str">The string that will be converted to an integer.</param>
        /// <returns>
        /// The number or null.
        /// </returns>
        private static int? GetSIntAt(string[] str, int index)
        {
            int ret;
            return index < str.Length && int.TryParse(str[index], out ret) ? ret : default(int?);
        }

        /// <summary>
        /// Returns the number represented by the given text.
        /// </summary>
        /// <param name="str">The string that will be converted to an integer.</param>
        /// <returns>
        /// The number or null.
        /// </returns>
        private static uint? GetUIntAt(string[] str, int index)
        {
            uint ret;
            return index < str.Length && uint.TryParse(str[index], out ret) ? ret : default(uint?);
        }
    }
}
