// Copyright (c) János Janka. All rights reserved.

using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Web;
using DotNetOpenAuth.AspNet.Clients;

namespace Partnerinfo.Identity.OAuth
{
    public class OAuthMicrosoftClient : OAuth2Client
    {
        private const string AuthorizationEndpoint = "https://oauth.live.com/authorize";
        private const string TokenEndpoint = "https://oauth.live.com/token";
        private const string UserDataEndpoint = "https://apis.live.net/v5.0/me?access_token=";
        private const string Scope = "wl.basic,wl.emails,wl.birthday";

        private readonly string _appId;
        private readonly string _appSecret;

        /// <summary>
        /// Initializes a new instance of the <see cref="OAuthMicrosoftClient" /> class.
        /// </summary>
        /// <param name="appId">The app id.</param>
        /// <param name="appSecret">The app secret.</param>
        public OAuthMicrosoftClient(string appId, string appSecret)
            : this("microsoft", appId, appSecret)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="OAuthMicrosoftClient" /> class.
        /// </summary>
        /// <param name="providerName">Name of the provider.</param>
        /// <param name="appId">The app id.</param>
        /// <param name="appSecret">The app secret.</param>
        protected OAuthMicrosoftClient(string providerName, string appId, string appSecret)
            : base(providerName)
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
                new UriParameter("response_type", "code"),
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
            OAuthMicrosoftClientData data;

            using (var response = WebRequest.Create(UserDataEndpoint + HttpUtility.UrlEncode(accessToken)).GetResponse())
            {
                using (var stream = response.GetResponseStream())
                {
                    data = OAuthHelpers.Deserialize<OAuthMicrosoftClientData>(stream);
                }
            }

            // Try to select an email address for the user. :S
            string email = data.Emails.Preferred ?? data.Emails.Account ?? data.Emails.Personal ?? data.Emails.Business;

            string birthday;
            try
            {
                birthday = new DateTime(data.BirthYear, data.BirthMonth, data.BirthDay).ToString();
            }
            catch (ArgumentOutOfRangeException)
            {
                birthday = null;
            }

            return OAuthHelpers.CreateResponse(data.Id, email, data.Name, data.FirstName, data.LastName, data.Gender, data.Link, birthday);
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
            string formData = UriUtility.CreateQueryString(
                new UriParameter("client_id", _appId),
                new UriParameter("client_secret", _appSecret),
                new UriParameter("code", authorizationCode),
                new UriParameter("grant_type", "authorization_code"),
                new UriParameter("redirect_uri", returnUrl.AbsoluteUri));

            var request = WebRequest.Create(TokenEndpoint);
            request.ContentType = "application/x-www-form-urlencoded";
            request.ContentLength = formData.Length;
            request.Method = "POST";

            using (var stream = request.GetRequestStream())
            {
                var writer = new StreamWriter(stream);
                writer.Write(formData);
                writer.Flush();
            }

            var response = (HttpWebResponse)request.GetResponse();
            if (response.StatusCode == HttpStatusCode.OK)
            {
                using (var stream = response.GetResponseStream())
                {
                    var data = OAuthHelpers.Deserialize<OAuth2AccessTokenData>(stream);
                    if (data != null)
                    {
                        return data.AccessToken;
                    }
                }
            }
            return null;
        }
    }
}