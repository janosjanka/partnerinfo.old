// Copyright (c) János Janka. All rights reserved.

using System;
using Newtonsoft.Json;
using Partnerinfo.Security;

namespace Partnerinfo.Portal.ViewModels
{
    public sealed class EngineResultViewModel
    {
        /// <summary>
        /// Gets or sets the anonymous user identifier that identifies a user without authentication.
        /// </summary>
        /// <value>
        /// The anonymous user identifier.
        /// </value>
        public Guid AnonymId { get; set; }

        /// <summary>
        /// Gets or sets the <see cref="AuthTicket" /> that identifies an authenticated <see cref="Partnerinfo.Project.ContactItem" />.
        /// </summary>
        /// <value>
        /// The <see cref="AuthTicket" />.
        /// </value>
        public AuthTicket Identity { get; set; }

        /// <summary>
        /// Gets or sets a bearer token for the specified <see cref="Identity" /> that can be used for API calls.
        /// </summary>
        /// <value>
        /// The bearer token.
        /// </value>
        public string IdentityToken { get; set; }

        /// <summary>
        /// Gets or sets the security result for this page.
        /// </summary>
        /// <value>
        /// The security result for this page.
        /// </value>
        [JsonIgnore]
        public SecurityResult Security { get; set; }

        /// <summary>
        /// Gets or sets the <see cref="PortalResult" /> that contains information about the portal.
        /// </summary>
        /// <value>
        /// The <see cref="PortalResult" />.
        /// </value>
        public EnginePortalViewModel Portal { get; set; }

        /// <summary>
        /// Gets or sets the <see cref="PageItem" /> that contains the compiled page.
        /// </summary>
        /// <value>
        /// The compiled page.
        /// </value>
        public EnginePageViewModel Page { get; set; }

        /// <summary>
        /// Gets or sets the <see cref="EngineEventViewModel" />.
        /// </summary>
        /// <value>
        /// The <see cref="EngineEventViewModel" />.
        /// </value>
        public EngineEventViewModel Event { get; set; }
    }
}