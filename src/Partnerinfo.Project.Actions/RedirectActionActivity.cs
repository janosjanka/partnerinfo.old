// Copyright (c) János Janka. All rights reserved.

using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using System.Web;

namespace Partnerinfo.Project.Actions
{
    public sealed class RedirectActionActivity : IActionActivity
    {
        public sealed class Options
        {
            public string Url { get; set; }

            //public bool ForwardTicket { get; set; }

            //public bool ForwardContact { get; set; }

            //public bool ForwardCustomUri { get; set; }
        }

        /// <summary>
        /// Called by the workflow runtime to execute an activity.
        /// </summary>
        /// <param name="context">The <see cref="ActionActivityContext"/> to associate with this activity and execution.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/> used to propagate notifications that the operation should be canceled.</param>
        /// <returns>
        /// The <see cref="ActionActivityResult"/> of the run task, which determines whether the activity remains in the executing state, or transitions to the closed state.
        /// </returns>
        public Task<ActionActivityResult> ExecuteAsync(ActionActivityContext context, CancellationToken cancellationToken)
        {
            var result = context.CreateResult(ActionActivityStatusCode.Failed);
            var options = context.Action.Options?.ToObject<Options>();
            if (options != null)
            {
                result.StatusCode = ActionActivityStatusCode.Success;
                result.ReturnUrl = GenerateUrl(context, options);
                //result.ReturnUrl = IsValidLocalUrl(context, options.Url) ? GenerateUrl(context, options) : options.Url;
            }
            return Task.FromResult(result);
        }

        /// <summary>
        /// Generates an action URL.
        /// </summary>
        /// <param name="context">The <see cref="ActionActivityContext" /> to associate with this activity and execution.</param>
        /// <param name="options"></param>
        /// <returns></returns>
        private string GenerateUrl(ActionActivityContext context, Options options)
        {
            var service = context.Resolve<IActionLinkService>();
            var uriParams = new List<UriParameter>
            {
                new UriParameter(
                    ResourceKeys.ActionParamName,
                    service.UrlTokenEncode(new ActionLink(context.RootAction.Id, context.Contact?.Id, context.Event.CustomUri)))
            };
            if (context.IsAuthenticated)
            {
                uriParams.Add(new UriParameter(ResourceKeys.ContactTokenParamName, AuthUtility.Protect(context.AuthTicket)));
            }
            return UriUtility.MakeUri(options.Url, UriKind.Absolute, uriParams.ToArray());
        }

        /// <summary>
        /// Gets a value indicating whether the URL represents a local address.
        /// </summary>
        /// <param name="context">The <see cref="ActionActivityContext" /> to associate with this activity and execution.</param>
        /// <param name="url">The URL to check.</param>
        /// <returns>
        /// True if the URL is local.
        /// </returns>
        /*
        private bool IsValidLocalUrl(ActionActivityContext context, string url)
        {
            if (url == null || !Uri.IsWellFormedUriString(url, UriKind.Absolute))
            {
                return false;
            }
            string hostName = UriUtility.GetNakedHost(url);
            if (hostName.EndsWith("partnerinfo.tv", StringComparison.OrdinalIgnoreCase))
            {
                return true;
            }
            var services = context.Resolve<IPersistenceServices>();
            return services.Portal.GetPageIdByDomain(hostName) != null;
        }
        */
    }
}
