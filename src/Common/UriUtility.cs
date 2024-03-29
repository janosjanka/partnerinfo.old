// Copyright (c) János Janka. All rights reserved.

using System.Collections.Specialized;
using System.Security.Cryptography;
using System.Text;

namespace System.Web
{
    /// <summary>
    /// Represents a query parameter.
    /// </summary>
    internal struct UriParameter
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="UriParameter" /> struct.
        /// </summary>
        /// <param name="name">The name of the query parameter.</param>
        /// <param name="value">The value of the query parameter.</param>
        public UriParameter(string name, string value)
        {
            Name = name;
            Value = value;
        }

        /// <summary>
        /// Parameter name
        /// </summary>
        public string Name { get; }

        /// <summary>
        /// Parameter value
        /// </summary>
        public string Value { get; }
    }

    internal static class UriUtility
    {
        private const int DefaultTokenLength = 16;
        private const string UriChars = "-._~0123456789abcdefghijklmnoupqrstvwxyzABCDEFGHIJKLMNOUPQRSTVWXYZ";

        /// <summary>
        /// Gets the naked host name with port number from the specified absolute uri.
        /// If the port number is the default for the schema, it will not be included.
        /// <para>http://www.partnerinfo.tv =&gt; partnerinfo.tv</para>
        /// <para>http://localhost.partnerinfo.tv:7500 =&gt; localhost.partnerinfo.tv:7500</para>
        /// </summary>
        /// <param name="uri">The uniform resource identifier (URI).</param>
        /// <returns></returns>
        /// <exception cref="System.ArgumentNullException">Thrown when a null reference is passed to the method.</exception>
        public static string GetNakedHost(string uri)
        {
            return GetNakedHost(new Uri(uri, UriKind.Absolute));
        }

        /// <summary>
        /// Gets the naked host name with port number from the specified absolute uri.
        /// If the port number is the default for the schema, it will not be included.
        /// <para>http://www.partnerinfo.tv =&gt; partnerinfo.tv</para>
        /// <para>http://localhost.partnerinfo.tv:7500 =&gt; localhost.partnerinfo.tv:7500</para>
        /// </summary>
        /// <param name="uri">The uniform resource identifier (URI).</param>
        /// <returns></returns>
        /// <exception cref="System.ArgumentNullException">Thrown when a null reference is passed to the method.</exception>
        public static string GetNakedHost(Uri uri)
        {
            if (uri == null)
            {
                throw new ArgumentNullException(nameof(uri));
            }
            string hostName = uri.Host;
            if (hostName.StartsWith("www.", StringComparison.OrdinalIgnoreCase))
            {
                hostName = hostName.Substring(4);
            }
            if (!uri.IsDefaultPort)
            {
                hostName += ":" + uri.Port;
            }
            return hostName;
        }

        /// <summary>
        /// Creates a random byte array using RNG Crypto API.
        /// </summary>
        /// <param name="tokenLength">The length of the byte array.</param>
        /// <returns>Returns with a byte array.</returns>
        public static byte[] CreateRngRandomToken(int tokenLength)
        {
            byte[] data = new byte[tokenLength];
            using (var provider = new RNGCryptoServiceProvider())
            {
                provider.GetBytes(data);
            }
            return data;
        }

        /// <summary>
        /// Encodes an URI token from the byte array.
        /// </summary>
        /// <param name="input">The input string to encode.</param>
        /// <returns>Returns with an URI token.</returns>
        public static string EncodeUriTokenBytes(byte[] input)
        {
            return HttpServerUtility.UrlTokenEncode(input);
        }

        /// <summary>
        /// Encodes an URI token from the input string.
        /// </summary>
        /// <param name="input">The input string to encode.</param>
        /// <param name="encoding">Represents a character encoding.</param>
        /// <returns>Returns with an URI token.</returns>
        public static string EncodeUriToken(string input, Encoding encoding)
        {
            return EncodeUriTokenBytes(encoding.GetBytes(input));
        }

        /// <summary>
        /// Encodes an URI token (UTF-8) from the input string.
        /// </summary>
        /// <param name="input">The input string to encode.</param>
        /// <returns>Returns with an URI token.</returns>
        public static string EncodeUriToken(string input)
        {
            return EncodeUriTokenBytes(Encoding.UTF8.GetBytes(input));
        }

        /// <summary>
        /// Encodes a random URL token using RNG Crypto API.
        /// </summary>
        /// <param name="tokenLength">The length of the token (16).</param>
        /// <returns>Returns with a random URI token.</returns>
        public static string EncodeRandomUriToken(int tokenLength = DefaultTokenLength)
        {
            return EncodeUriTokenBytes(CreateRngRandomToken(tokenLength));
        }

        /// <summary>
        /// Creates a URL value.
        /// </summary>
        public static byte[] DecodeUriTokenBytes(string input)
        {
            return HttpServerUtility.UrlTokenDecode(input);
        }

        /// <summary>
        /// Creates an URL value.
        /// </summary>
        public static string DecodeUriToken(string token, Encoding encoding)
        {
            return encoding.GetString(DecodeUriTokenBytes(token));
        }

        /// <summary>
        /// Creates an URL value.
        /// </summary>
        public static string DecodeUriToken(string token)
        {
            return Encoding.UTF8.GetString(DecodeUriTokenBytes(token));
        }

        /// <summary>
        /// Parses a query string into a <see cref="System.Collections.Specialized.NameValueCollection" /> using the specified <see cref="System.Text.Encoding" />.
        /// </summary>
        /// <param name="query">The query string to parse.</param>
        /// <returns>
        /// A <see cref="System.Collections.Specialized.NameValueCollection" /> of query parameters.
        /// </returns>
        public static NameValueCollection ParseQueryString(string query)
        {
            return HttpUtility.ParseQueryString(query);
        }

        /// <summary>
        /// Parses a query string into a <see cref="System.Collections.Specialized.NameValueCollection" /> using the specified <see cref="System.Text.Encoding" />.
        /// </summary>
        /// <param name="query">The query string to parse.</param>
        /// <param name="encoding">The <see cref="System.Text.Encoding" /> to use.</param>
        /// <returns>
        /// A <see cref="System.Collections.Specialized.NameValueCollection" /> of query parameters.
        /// </returns>
        public static NameValueCollection ParseQueryString(string query, Encoding encoding)
        {
            return HttpUtility.ParseQueryString(query, encoding);
        }

        /// <summary>
        /// Removes a trailing slash mark (/) from a path.
        /// </summary>
        /// <param name="builder">The URI builder.</param>
        /// <exception cref="System.ArgumentNullException">Thrown when a null reference is passed to the method.</exception>
        public static void RemoveLastTrailingSlashes(StringBuilder builder)
        {
            if (builder == null)
            {
                throw new ArgumentNullException(nameof(builder));
            }

            int i = builder.Length;
            int l = i - 1;
            while (--i >= 0)
            {
                char ch = builder[i];
                if (ch != '/' && ch != '\\' && !char.IsWhiteSpace(ch))
                {
                    break;
                }
            }
            if (i < l)
            {
                builder.Remove(i + 1, l - i);
            }
        }

        /// <summary>
        /// Removes a trailing slash mark (/) from a path.
        /// </summary>
        /// <param name="path">The path to parse.</param>
        /// <returns>
        /// A <see cref="String" /> representing a fixed path.
        /// </returns>
        /// <exception cref="System.ArgumentNullException">Thrown when a null reference is passed to the method.</exception>
        public static string RemoveLastTrailingSlashes(string path)
        {
            if (path == null)
            {
                throw new ArgumentNullException("path");
            }
            var builder = new StringBuilder(path);
            RemoveLastTrailingSlashes(builder);
            return builder.ToString();
        }

        /// <summary>
        /// Creates a query string using the given parameters.
        /// </summary>
        /// <param name="parameters">The query parameters to encode.</param>
        /// <returns>
        /// A <see cref="String" /> representing well-formed query parameters.
        /// </returns>
        /// <exception cref="System.ArgumentNullException">Thrown when a null reference is passed to the method.</exception>
        public static string CreateQueryString(params UriParameter[] parameters)
        {
            return RecodeQueryString(string.Empty, parameters);
        }

        /// <summary>
        /// Recodes a query string using the given parameters.
        /// </summary>
        /// <param name="query">The query string to recode.</param>
        /// <param name="parameters">The query parameters to add.</param>
        /// <returns>
        /// A <see cref="String" /> representing well-formed query parameters.
        /// </returns>
        /// <exception cref="System.ArgumentNullException">Thrown when a null reference is passed to the method.</exception>
        public static string RecodeQueryString(string query, params UriParameter[] parameters)
        {
            if (query == null)
            {
                throw new ArgumentNullException(nameof(query));
            }

            var collection = ParseQueryString(query);
            if (collection == null)
            {
                return string.Empty;
            }
            for (int i = 0; i < parameters.Length; ++i)
            {
                var p = parameters[i];
                collection[p.Name] = p.Value;
            }
            return collection.ToString();
        }

        /// <summary>
        /// Creates a new url with the specified query parameters.
        /// </summary>
        /// <param name="uriString">A string that identifies the resource to be represented by the <see cref="System.Uri" /> instance.</param>
        /// <param name="parameters">The query parameters to add.</param>
        /// <returns>
        /// A <see cref="String" /> representing a web resource.
        /// </returns>
        /// <exception cref="System.ArgumentNullException">Thrown when a null reference is passed to the method.</exception>
        public static string MakeUri(string uriString, params UriParameter[] parameters)
        {
            if (uriString == null)
            {
                throw new ArgumentNullException(nameof(uriString));
            }

            return MakeUri(new Uri(uriString), parameters);
        }

        /// <summary>
        /// Creates a new url with the specified query parameters.
        /// </summary>
        /// <param name="uriString">A string that identifies the resource to be represented by the <see cref="System.Uri" /> instance.</param>
        /// <param name="uriKind">Specifies whether the URI string is a relative URI, absolute URI, or is indeterminate.</param>
        /// <param name="parameters">The query parameters to add.</param>
        /// <returns>
        /// A <see cref="String" /> representing a web resource.
        /// </returns>
        /// <exception cref="System.ArgumentNullException">Thrown when a null reference is passed to the method.</exception>
        public static string MakeUri(string uriString, UriKind uriKind, params UriParameter[] parameters)
        {
            if (uriString == null)
            {
                throw new ArgumentNullException(nameof(uriString));
            }

            return MakeUri(new Uri(uriString, uriKind), parameters);
        }

        /// <summary>
        /// Creates a new url with the specified query parameters.
        /// </summary>
        /// <param name="uri">The uri to create.</param>
        /// <param name="parameters">The query parameters to add.</param>
        /// <returns>
        /// A <see cref="String" /> representing a web resource.
        /// </returns>
        /// <exception cref="System.ArgumentNullException">Thrown when a null reference is passed to the method.</exception>
        public static string MakeUri(Uri uri, params UriParameter[] parameters)
        {
            if (uri == null)
            {
                throw new ArgumentNullException(nameof(uri));
            }

            var builder = new StringBuilder(uri.GetLeftPart(UriPartial.Path), 256);

            RemoveLastTrailingSlashes(builder);

            string query = RecodeQueryString(uri.Query, parameters);
            if (!string.IsNullOrEmpty(query))
            {
                builder.Append('?');
                builder.Append(query);
            }

            if (!string.IsNullOrEmpty(uri.Fragment))
            {
                builder.Append(uri.Fragment);
            }

            return builder.ToString();
        }

        /// <summary>
        /// Determines whether the specified URL part (slug) is a well-formed URI part.
        /// </summary>
        /// <param name="uriPart">The URI part (slug) to check.</param>
        /// <returns>
        ///   <c>true</c> if the URI part is well-formed; otherwise, <c>false</c>.
        /// </returns>
        /// <exception cref="System.ArgumentNullException">Thrown when a null reference is passed to the method.</exception>
        public static bool IsWellFormedUriPart(string uriPart)
        {
            if (uriPart == null)
            {
                throw new ArgumentNullException(nameof(uriPart));
            }

            for (int i = 0; i < uriPart.Length; ++i)
            {
                if (UriChars.IndexOf(uriPart[i]) < 0)
                {
                    return false;
                }
            }
            return true;
        }

        /// <summary>
        /// Returns a normalized representation of the specified <paramref name="uriPart" />.
        /// </summary>
        /// <param name="uriPart">The uri to normalize.</param>
        /// <returns>
        /// A normalized representation of the specified <paramref name="uriPart" />.
        /// </returns>
        /// <exception cref="System.ArgumentNullException">uriPart</exception>
        public static string Normalize(string uriPart)
        {
            if (uriPart == null)
            {
                throw new ArgumentNullException(nameof(uriPart));
            }

            // Remove all leading and trailing white-space characters and normalize 
            // the given URI part using full compatibility decomposition (FormKD).
            string normalizedUri = uriPart
                .Trim()
                .Normalize(NormalizationForm.FormKD)
                .ToLower();

            // Remove both non-standard and non-friendly URI characters. E.g.
            // "Janka  János Zoltán   " => "janka-janos-zoltan"
            // "  janka-jános   Zoltán" => "janka-janos-zoltan"
            // " 0123456789-ok.4555HA~" => "0123456789-ok.4555ha~"
            var uriPartSep = true;
            var uriBuilder = new StringBuilder(normalizedUri.Length);
            for (var i = 0; i < normalizedUri.Length; ++i)
            {
                char ch = normalizedUri[i];
                if (ch >= 'a' && ch <= 'z' || ch >= '0' && ch <= '9' ||
                    ch == '-' || ch == '.' || ch == '_' || ch == '~')
                {
                    uriBuilder.Append(ch);
                    uriPartSep = false;
                }
                else if (!uriPartSep && char.IsWhiteSpace(ch))
                {
                    uriBuilder.Append('-');
                    uriPartSep = true;
                }
            }
            return uriBuilder.ToString();
        }
    }

    internal static class UriBuilderExtensions
    {
        /// <summary>
        /// Recodes a query string using the given parameters.
        /// </summary>
        /// <param name="builder">The <see cref="UriBuilder" />.</param>
        /// <param name="parameters">The query parameters to add.</param>
        /// <exception cref="System.ArgumentNullException">Thrown when a null reference is passed to the method.</exception>
        public static void SetQuery(this UriBuilder builder, params UriParameter[] parameters)
        {
            if (builder == null)
            {
                throw new ArgumentNullException(nameof(builder));
            }

            builder.Query = UriUtility.RecodeQueryString(builder.Query ?? string.Empty, parameters);
        }
    }
}
