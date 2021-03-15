// Copyright (c) János Janka. All rights reserved.

namespace Partnerinfo
{
    public static class ResourceKeys
    {
        /// <summary>
        /// The name of the cookie that identifies a culture.
        /// </summary>
        public static readonly string LocaleCookieName = "LOC";

        /// <summary>
        /// The name of the query parameter that identifies a project action.
        /// </summary>
        public static readonly string ActionParamName = "ref";

        /// <summary>
        /// The name of the cookie that is used by the anti-forgery system.
        /// </summary>
        public static readonly string AntiForgeryTokenName = "AFG";

        /// <summary>
        /// The name of the cookie that identifies an anomymous user.
        /// </summary>
        public static readonly string AnonymIdCookieName = "AID";

        /// <summary>
        /// The name of the cookie that identifies a user.
        /// </summary>
        public static readonly string IdentityTokenCookieName = "ITK";

        /// <summary>
        /// The name of the cookie that identifies a project contact.
        /// </summary>
        public static readonly string ContactTokenCookieName = "CTK";

        /// <summary>
        /// The name of the query parameter that identifies a project contact.
        /// </summary>
        public static readonly string ContactTokenParamName = "ctk";
    }
}
