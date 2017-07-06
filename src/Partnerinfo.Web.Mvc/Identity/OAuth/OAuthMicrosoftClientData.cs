// Copyright (c) János Janka. All rights reserved.

using System;
using Newtonsoft.Json;

namespace Partnerinfo.Identity.OAuth
{
    public sealed class OAuthMicrosoftClientData
    {
        [JsonProperty("id")]
        public string Id { get; set; }
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
        [JsonProperty("birth_day")]
        public int BirthDay { get; set; }
        [JsonProperty("birth_month")]
        public int BirthMonth { get; set; }
        [JsonProperty("birth_year")]
        public int BirthYear { get; set; }
        [JsonProperty("emails")]
        public OAuthMicrosoftClientEmailData Emails { get; set; }
    }
}