// Copyright (c) János Janka. All rights reserved.

using System;
using Newtonsoft.Json;

namespace Partnerinfo.Identity.OAuth
{
    public sealed class OAuthFacebookClientData
    {
        [JsonProperty("id")]
        public string Id { get; set; }
        [JsonProperty("email")]
        public string Email { get; set; }
        [JsonProperty("name")]
        public string Name { get; set; }
        [JsonProperty("first_name")]
        public string FirstName { get; set; }
        [JsonProperty("last_name")]
        public string LastName { get; set; }
        [JsonProperty("gender")]
        public string Gender { get; set; }
        [JsonProperty("link")]
        public Uri Link { get; set; }
        [JsonProperty("birthday")]
        public string Birthday { get; set; }
    }
}