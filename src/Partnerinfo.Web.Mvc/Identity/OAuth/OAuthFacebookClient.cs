// Copyright (c) János Janka. All rights reserved.

using System;
using System.Collections.Generic;
using System.Net;
using System.Web;
using DotNetOpenAuth.AspNet.Clients;

namespace Partnerinfo.Identity.OAuth
{
    public sealed class OAuthFacebookClient : OAuth2Client
    {
        private const string AuthorizationEndpoint = "https://www.facebook.com/dialog/oauth";
        private const string TokenEndpoint = "https://graph.facebook.com/oauth/access_token";
        private const string UserDataEndpoint = "https://graph.facebook.com/me?access_token=";
        private const string Scope = "email,user_birthday";

        private readonly string _appId;
        private readonly string _appSecret;

        /// <summary>
        /// Initializes a new instance of the <see cref="OAuthFacebookClient" /> class.
        /// </summary>
        /// <param name="appId">The app id.</param>
        /// <param name="appSecret">The app secret.</param>
        public OAuthFacebookClient(string appId, string appSecret)
            : base("facebook")
        {
            if (appId == null)
            {
                throw new ArgumentNullException("appId");
            }
            if (appSecret == null)
            {
                throw new ArgumentNullException("appSecret");
            }

            _appId = appId;
            _appSecret = appSecret;
        }

        /// <summary>
        /// Gets the full url pointing to the login page for this client. The url should include the specified return url so that when the login completes, user is redirected back to that url.
        /// </summary>
        /// <param name="returnUrl">The return URL.</param>
        /// <returns>
        /// An absolute URL.
        /// </returns>
        protected override Uri GetServiceLoginUrl(Uri returnUrl)
        {
            var builder = new UriBuilder(AuthorizationEndpoint);

            builder.SetQuery(
                new UriParameter("client_id", _appId),
                new UriParameter("scope", Scope),
                new UriParameter("redirect_uri", returnUrl.AbsoluteUri));

            return builder.Uri;
        }

        /// <summary>
        /// Given the access token, gets the logged-in user's data. The returned dictionary must include two keys 'id', and 'username'.
        /// </summary>
        /// <param name="accessToken">The access token of the current user.</param>
        /// <returns>
        /// A dictionary contains key-value pairs of user data
        /// </returns>
        protected override IDictionary<string, string> GetUserData(string accessToken)
        {
            OAuthFacebookClientData data;

            using (var response = WebRequest.Create(UserDataEndpoint + HttpUtility.UrlEncode(accessToken)).GetResponse())
            {
                using (var stream = response.GetResponseStream())
                {
                    data = OAuthHelpers.Deserialize<OAuthFacebookClientData>(stream);
                }
            }

            return OAuthHelpers.CreateResponse(data.Id, data.Email, data.Name, data.FirstName, data.LastName, data.Gender, data.Link, data.Birthday);
        }

        /// <summary>
        /// Normalizes hexa encoding.
        /// </summary>
        private static string NormalizeHexEncoding(string url)
        {
            char[] charArray = url.ToCharArray();

            for (int i = 0; i < charArray.Length - 2; i++)
            {
                if (charArray[i] == '%')
                {
                    charArray[i + 1] = char.ToUpperInvariant(charArray[i + 1]);
                    charArray[i + 2] = char.ToUpperInvariant(charArray[i + 2]);
                    i += 2;
                }
            }

            return new string(charArray);
        }

        /// <summary>
        /// Queries the access token from the specified authorization code.
        /// </summary>
        /// <param name="returnUrl">The return URL.</param>
        /// <param name="authorizationCode">The authorization code.</param>
        /// <returns>
        /// The access token
        /// </returns>
        protected override string QueryAccessToken(Uri returnUrl, string authorizationCode)
        {
            var builder = new UriBuilder(TokenEndpoint);

            builder.SetQuery(
                 new UriParameter("client_id", _appId),
                 new UriParameter("client_secret", _appSecret),
                 new UriParameter("scope", Scope),
                 new UriParameter("code", authorizationCode),
                 new UriParameter("redirect_uri", NormalizeHexEncoding(returnUrl.AbsoluteUri)));

            using (var client = new WebClient())
            {
                string str = client.DownloadString(builder.Uri);
                if (string.IsNullOrEmpty(str))
                {
                    return null;
                }
                return HttpUtility.ParseQueryString(str)["access_token"];
            }
        }
    }
}