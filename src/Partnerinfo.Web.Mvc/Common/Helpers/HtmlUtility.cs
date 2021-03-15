// Copyright (c) János Janka. All rights reserved.

using System.Text;

namespace System.Web.Mvc.Html
{
    public static class HtmlUtility
    {
        /// <summary>
        /// Generates Google Analytics code tracking both domains and subdomains.
        /// </summary>
        /// <param name="accountId">The unique identifier of the Google Analytics account.</param>
        /// <param name="domainName">The domain name for the GATC cookies. There are three modes to this method: ("auto" | "none" | [domain]).
        /// By default, the method is set to auto, which attempts to resolve the domain name based on the document.domain property in the DOM.</param>
        /// <returns>
        /// The generated code snippet.
        /// </returns>
        public static string GenerateGoogleAnalytics(string accountId, string domainName)
        {
            var builder = new StringBuilder(512);

            builder.AppendLine("<script type=\"text/javascript\">//<![CDATA[");
            builder.Append("var _gaq=_gaq||[];");
            builder.Append("_gaq.push([\"_setAccount\",\"");
            builder.Append(accountId);
            builder.Append("\"]);");
            builder.Append("_gaq.push([\"_trackPageview\"]);");

            if (domainName != null)
            {
                builder.Append("_gaq.push([\"_setDomainName\",\"");
                builder.Append(domainName);
                builder.Append("\"]);");
            }

            builder.Append("(function(){");
            builder.Append("var a=document.createElement(\"script\"),b;");
            builder.Append("a.type=\"text/javascript\";a.async=true;");
            builder.Append("a.src=(\"https:\"==document.location.protocol?\"https://ssl\":\"http://www\")+\".google-analytics.com/ga.js\";");
            builder.Append("b=document.getElementsByTagName(\"script\")[0];");
            builder.Append("b.parentNode.insertBefore(a,b);");
            builder.Append("})();//]]></script>");

            return builder.ToString();
        }
    }
}
