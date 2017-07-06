// Copyright (c) János Janka. All rights reserved.

using Newtonsoft.Json;

namespace Partnerinfo.Identity.OAuth
{
    public class OAuthMicrosoftClientEmailData
    {
        [JsonProperty("account")]
        public string Account { get; set; }
        [JsonProperty("preferred")]
        public string Preferred { get; set; }
        [JsonProperty("personal")]
        public string Personal { get; set; }
        [JsonProperty("business")]
        public string Business { get; set; }
    }
}