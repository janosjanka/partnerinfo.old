// Copyright (c) János Janka. All rights reserved.

using System.Text;
using System.Threading;
using Partnerinfo.Properties;
using WebMatrix.WebData;

namespace System.Web.Mvc.Html
{
    public static class HtmlExtensions
    {
        public static MvcHtmlString ScriptConfig(this HtmlHelper htmlHelper)
        {
            var builder = new StringBuilder(512);
            builder.AppendLine("<script type=\"text/javascript\">//<![CDATA[");
            builder.Append("(function(g){");
#if DEBUG
            builder.Append("g.DEBUG=1;");
#endif
            builder.Append("g.PI={");
            builder.Append("Server:{mapPath:function(path){return\"/assets\"+path}},");
            builder.Append("config:{");
            builder.Append("version:");
            builder.Append(HttpUtility.JavaScriptStringEncode(Settings.Default.AppVersion, true));
            builder.Append(",");
            builder.Append("language:");
            builder.Append(HttpUtility.JavaScriptStringEncode(Thread.CurrentThread.CurrentUICulture.TwoLetterISOLanguageName, true));
            builder.Append("},");
            builder.Append("identity:");
            var identity = htmlHelper.ViewContext.HttpContext.User.Identity;
            if (identity.IsAuthenticated)
            {
                builder.Append("{");
                builder.Append("id:");
                builder.Append(WebSecurity.CurrentUserId);
                builder.Append(",");
                builder.Append("email:{");
                builder.Append("address:");
                builder.Append(HttpUtility.JavaScriptStringEncode(WebSecurity.CurrentUserName, true));
                builder.Append("}");
                builder.Append("}");
            }
            else
            {
                builder.Append("null");
            }
            builder.Append("};})(window)//]]></script>");
            return new MvcHtmlString(builder.ToString());
        }

        /// <summary>
        /// Generates a Google Analytics code tracking both domains and subdomains.
        /// </summary>
        /// <param name="htmlHelper">The HTML helper to extend.</param>
        /// <param name="accountId">The unique identifier of the Google Analytics account.</param>
        /// <returns>
        /// The generated code snippet.
        /// </returns>
        public static MvcHtmlString GoogleAnalytics(this HtmlHelper htmlHelper, string accountId)
        {
            return new MvcHtmlString(HtmlUtility.GenerateGoogleAnalytics(accountId, null));
        }

        /// <summary>
        /// Generates a Google Analytics code tracking both domains and subdomains.
        /// </summary>
        /// <param name="htmlHelper">The HTML helper to extend.</param>
        /// <param name="accountId">The unique identifier of the Google Analytics account.</param>
        /// <param name="domainName">The domain name for the GATC cookies. There are three modes to this method: ("auto" | "none" | [domain]).
        /// By default, the method is set to auto, which attempts to resolve the domain name based on the document.domain property in the DOM.</param>
        /// <returns>
        /// The generated code snippet.
        /// </returns>
        public static MvcHtmlString GoogleAnalytics(this HtmlHelper htmlHelper, string accountId, string domainName)
        {
            return new MvcHtmlString(HtmlUtility.GenerateGoogleAnalytics(accountId, domainName));
        }

        /// <summary>
        /// Generates a JavaScript string literal or a null literal.
        /// </summary>
        /// <param name="htmlHelper">The HTML helper to extend.</param>
        /// <param name="value">The string to format.</param>
        /// <returns>
        /// The generated code snippet.
        /// </returns>
        public static MvcHtmlString StringOrNull(this HtmlHelper htmlHelper, string value)
        {
            return new MvcHtmlString(value == null ? "null" : HttpUtility.JavaScriptStringEncode(value, true));
        }
    }
}