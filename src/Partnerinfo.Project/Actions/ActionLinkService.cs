// Copyright (c) János Janka. All rights reserved.

using System;

namespace Partnerinfo.Project.Actions
{
    public sealed class ActionLinkService : IActionLinkService
    {
        /// <summary>
        /// Creates a new action link parameters that can be shared on the Web.
        /// </summary>
        /// <param name="actionLink">An object that configures the action link will be created.</param>
        /// <returns>
        /// The link.
        /// </returns>
        public string UrlTokenEncode(ActionLink actionLink) => ActionLinkHelper.UrlTokenEncode(actionLink);

        /// <summary>
        /// Parses the given string parameters and returns with a <see cref="ActionLink" /> object.
        /// </summary>
        /// <param name="paramUri">The string that contains entity ID(s).</param>
        /// <param name="customUri">The user-defined URI for the action link.</param>
        /// <returns>
        /// A <see cref="ActionLink" /> object.
        /// </returns>
        public ActionLink UrlTokenDecode(string paramUri, string customUri = null) => ActionLinkHelper.UrlTokenDecode(paramUri, customUri);

        /// <summary>
        /// Creates a new action link that can be shared on the Web.
        /// </summary>
        /// <param name="actionLink">An object that configures the action link will be created.</param>
        /// <param name="absolute">It will create an absolute URL if true.</param>
        /// <returns>
        /// The link.
        /// </returns>
        public string CreateLink(ActionLink actionLink, bool absolute) => ActionLinkHelper.CreateLink(actionLink, absolute);

        /// <summary>
        /// Parses the given string parameters and returns with a <see cref="ActionLink" /> object.
        /// </summary>
        /// <param name="link">The action link.</param>
        /// <returns>
        /// A <see cref="ActionLink" /> object.
        /// </returns>
        public ActionLink DecodeLink(string link) => ActionLinkHelper.DecodeLink(link);

        /// <summary>
        /// In a specified input string, replaces all action link parameter
        /// with another action link parameter returned by a callback function.
        /// </summary>
        /// <param name="input">The string to search for a match.</param>
        /// <param name="callback">A custom method that examines each match and returns either the original matched
        /// action link parameter or a replacement action link parameter.</param>
        public string ReplaceLinks(string input, Action<ActionLink> callback) => ActionLinkHelper.ReplaceLinks(input, callback);
    }
}
