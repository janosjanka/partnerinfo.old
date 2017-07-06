// Copyright (c) János Janka. All rights reserved.

using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Web;
using DotNetOpenAuth.AspNet.Clients;

namespace Partnerinfo.Identity.OAuth
{
    /// <summary>
    /// A DotNetOpenAuth client for logging in to Google using OAuth2.
    /// Reference: https://developers.google.com/accounts/docs/OAuth2
    /// </summary>
    public class OAuthGoogleClient : OAuth2Client
    {
        private const string AuthorizationEndpoint = "https://accounts.google.com/o/oauth2/auth";
        private const string TokenEndpoint = "https://accounts.google.com/o/oauth2/token";
        private const string UserDataEndpoint = "https://www.googleapis.com/oauth2/v1/userinfo";
        private const string Scope = "https://www.googleapis.com/auth/userinfo.email https://www.googleapis.com/auth/userinfo.profile";

        private readonly string _appId;
        private readonly string _appSecret;

        /// <summary>
        /// Initializes a new instance of the <see cref="OAuthGoogleClient" /> class.
        /// </summary>
        /// <param name="appId">The app id.</param>
        /// <param name="appSecret">The app secret.</param>
        public OAuthGoogleClient(string appId, string appSecret)
            : base("google")
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
                new UriParameter("response_type", "code"),
                new UriParameter("client_id", _appId),
                new UriParameter("scope", Scope),
                new UriParameter("redirect_uri", returnUrl.GetLeftPart(UriPartial.Path)), // Google OAuth 2 doesn't like query parameters.
                new UriParameter("state", string.IsNullOrEmpty(returnUrl.Query) ? string.Empty : returnUrl.Query.Substring(1)));

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
            var builder = new UriBuilder(UserDataEndpoint);
            builder.SetQuery(new UriParameter("access_token", accessToken));

            OAuthGoogleClientData data;
            using (var response = WebRequest.Create(builder.Uri).GetResponse())
            {
                using (var stream = response.GetResponseStream())
                {
                    data = OAuthHelpers.Deserialize<OAuthGoogleClientData>(stream);
                }
            }

            return OAuthHelpers.CreateResponse(data.Id, data.Email, data.Name, data.FirstName, data.LastName, data.Gender, data.Link, data.Birthday);
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
                new UriParameter("redirect_uri", returnUrl.GetLeftPart(UriPartial.Path))); // Google OAuth 2 doesn't like query parameters.

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

        /// <summary>
        /// Google requires that all return data be packed into a "state" parameter.
        /// This should be called before verifying the request, so that the url is rewritten to support this.
        /// </summary>
        public static void RewriteRequest()
        {
            var context = HttpContext.Current;
            string state = HttpUtility.UrlDecode(context.Request.QueryString["state"]);
            if (state != null && state.IndexOf("__provider__=google") >= 0)
            {
                var collection = HttpUtility.ParseQueryString(state);
                collection.Add(context.Request.QueryString);
                collection.Remove("state");
                context.RewritePath(context.Request.Path + "?" + collection);
            }
        }
    }
}