using System;
using Newtonsoft.Json;

namespace BeerDispenser.Shared
{

    public class Card
    {
        [JsonProperty("id")]
        public string Id { get; set; }
        [JsonProperty("object")]
        public string CardObject { get; set; }
        [JsonProperty("address_city")]
        public string City { get; set; }
        [JsonProperty("address_country")]
        public string AdressCountry { get; set; }
        [JsonProperty("address_line1")]
        public string Line1 { get; set; }
        [JsonProperty("address_line1_check")]
        public string Line1Check { get; set; }
        [JsonProperty("address_line2")]
        public string Line2 { get; set; }
        [JsonProperty("address_state")]
        public string State { get; set; }
        [JsonProperty("address_zip")]
        public string Zip { get; set; }
        [JsonProperty("address_zip_check")]
        public string ZipXheck { get; set; }
        [JsonProperty("brand")]
        public string Brand { get; set; }
        [JsonProperty("country")]
        public string Country { get; set; }
        [JsonProperty("cvc_check")]
        public string CvcCheck { get; set; }
        [JsonProperty("dynamic_last4")]
        public string Dynamiclast4 { get; set; }
        [JsonProperty("exp_month")]
        public string ExpMonth { get; set; }
        [JsonProperty("exp_year")]
        public string ExpYear { get; set; }
        [JsonProperty("funding")]
        public string Funding { get; set; }
        [JsonProperty("last4")]
        public string Last4 { get; set; }
        [JsonProperty("name")]
        public string Name { get; set; }
        [JsonProperty("tokenization_method")]
        public string TokenizationMethod { get; set; }
        [JsonProperty("wallet")]
        public string Wallet { get; set; }
    }

    public class Token
    {
        [JsonProperty("id")]
        public string Id { get; set; }
        [JsonProperty("object")]
        public string TokenObject { get; set; }
        [JsonProperty("card")]
        public Card Card { get; set; }
        [JsonProperty("client_ip")]
        public string ClientIp { get; set; }
        [JsonProperty("created")]
        public int Created { get; set; }
        [JsonProperty("email")]
        public string Email { get; set; }
        [JsonProperty("livemode")]
        public bool Livemode { get; set; }
        [JsonProperty("type")]
        public string Type { get; set; }
        [JsonProperty("used")]
        public bool Used { get; set; }
    }

    public class StripeTokenResponse
    {
        [JsonProperty("token")]
        public Token Token { get; set; }
    }
}
