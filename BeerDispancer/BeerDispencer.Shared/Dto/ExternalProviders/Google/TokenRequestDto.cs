﻿using Newtonsoft.Json;

namespace BeerDispenser.Shared.Dto.ExternalProviders.Google
{
    public class TokenRequestDto
    {
            [JsonProperty("client_id")]
            public string ClientId { get; set; }
            [JsonProperty("client_secret")]
            public string ClientSecret { get; set; }
            [JsonProperty("code")]
            public string Code { get; set; }
            [JsonProperty("redirect_uri")]
            public string RedirectUri { get; set; }
            [JsonProperty("grant_type")]
            public string GrantType { get; set; }
    }
}
