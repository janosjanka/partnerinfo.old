// Copyright (c) János Janka. All rights reserved.

using System;
using System.Collections.Generic;
using System.IO;
using System.Xml;
using System.Xml.Linq;
using DotNetOpenAuth.AspNet;
using DotNetOpenAuth.AspNet.Clients;
using DotNetOpenAuth.Messaging;
using DotNetOpenAuth.OAuth;
using DotNetOpenAuth.OAuth.ChannelElements;
using DotNetOpenAuth.OAuth.Messages;

namespace Partnerinfo.Identity.OAuth
{
    public class OAuthTwitterClient : OAuthClient
    {
        public static readonly ServiceProviderDescription TwitterServiceDescription = new ServiceProviderDescription
        {
            RequestTokenEndpoint = new MessageReceivingEndpoint("https://api.twitter.com/oauth/request_token", HttpDeliveryMethods.GetRequest | HttpDeliveryMethods.AuthorizationHeaderRequest),
            UserAuthorizationEndpoint = new MessageReceivingEndpoint("https://api.twitter.com/oauth/authenticate", HttpDeliveryMethods.GetRequest | HttpDeliveryMethods.AuthorizationHeaderRequest),
            AccessTokenEndpoint = new MessageReceivingEndpoint("https://api.twitter.com/oauth/access_token", HttpDeliveryMethods.GetRequest | HttpDeliveryMethods.AuthorizationHeaderRequest),
            TamperProtectionElements = new ITamperProtectionChannelBindingElement[] { new HmacSha1SigningBindingElement() }
        };

        /// <summary>
        /// Initializes a new instance of the <see cref="OAuthTwitterClient"/> class.
        /// </summary>
        /// <param name="consumerKey">The consumer key.</param>
        /// <param name="consumerSecret">The consumer secret.</param>
        public OAuthTwitterClient(string consumerKey, string consumerSecret) :
            this(consumerKey, consumerSecret, new AuthenticationOnlyCookieOAuthTokenManager())
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="OAuthTwitterClient"/> class.
        /// </summary>
        /// <param name="consumerKey">The consumer key.</param>
        /// <param name="consumerSecret">The consumer secret.</param>
        /// <param name="tokenManager">The token manager.</param>
        public OAuthTwitterClient(string consumerKey, string consumerSecret, IOAuthTokenManager tokenManager)
            : base("twitter", TwitterServiceDescription, new SimpleConsumerTokenManager(consumerKey, consumerSecret, tokenManager))
        {
        }

        /// <summary>
        /// Check if authentication succeeded after user is redirected back from the service provider.
        /// </summary>
        /// <param name="response">The response token returned from service provider</param>
        /// <returns>
        /// Authentication result
        /// </returns>
        protected override AuthenticationResult VerifyAuthenticationCore(AuthorizedTokenResponse response)
        {
            string userId = response.ExtraData["user_id"];
            string userName = response.ExtraData["screen_name"];

            var location = new Uri("https://api.twitter.com/1/users/show.xml?user_id=" + OAuthHelpers.EscapeUriDataStringRfc3986(userId));
            var profileEndpoint = new MessageReceivingEndpoint(location, HttpDeliveryMethods.GetRequest);
            var request = base.WebWorker.PrepareAuthorizedRequest(profileEndpoint, response.AccessToken);

            var dictionary = new Dictionary<string, string>() { { "accesstoken", response.AccessToken } };
            try
            {
                using (var response2 = request.GetResponse())
                {
                    using (var stream = response2.GetResponseStream())
                    {
                        var document = LoadXDocumentFromStream(stream);
                        var name = OAuthHelpers.ParseName(GetElementValue(document, "name"));

                        dictionary.Add("name", name.FullName);
                        dictionary.Add("firstName", name.FirstName);
                        dictionary.Add("lastName", name.LastName);
                        dictionary.Add("location", GetElementValue(document, "location"));
                        dictionary.Add("description", GetElementValue(document, "description"));
                        dictionary.Add("url", GetElementValue(document, "url"));
                    }
                }
            }
            catch (Exception)
            {
            }

            return new AuthenticationResult(true, base.ProviderName, userId, userName, dictionary);
        }

        /// <summary>
        /// Parser the specified XML stream.
        /// </summary>
        /// <param name="stream">The stream to parse.</param>
        /// <returns>A <see cref="XDocument"/> instance.</returns>
        private static XDocument LoadXDocumentFromStream(Stream stream)
        {
            var settings = new XmlReaderSettings
            {
                MaxCharactersFromEntities = 0x400L,
                XmlResolver = null,
                DtdProcessing = DtdProcessing.Prohibit
            };
            settings.MaxCharactersInDocument = 0x10000L;
            return XDocument.Load(XmlReader.Create(stream, settings));
        }

        /// <summary>
        /// Gets a node value.
        /// </summary>
        /// <param name="document">The document.</param>
        /// <param name="name">The name of the element.</param>
        /// <returns>The value of the element.</returns>
        private static string GetElementValue(XDocument document, string name)
        {
            var element = document.Root.Element(name);
            if (element == null)
            {
                return null;
            }
            return (string)element;
        }
    }
}