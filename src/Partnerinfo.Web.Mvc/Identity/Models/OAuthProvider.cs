// Copyright (c) János Janka. All rights reserved.

using System.Collections.Generic;

namespace Partnerinfo.Identity.Models
{
    public class OAuthProvider
    {
        public string Provider { get; set; }

        public string ProviderDisplayName { get; set; }

        public string ProviderUserId { get; set; }

        public IDictionary<string, object> ExtraData { get; set; }
    }
}