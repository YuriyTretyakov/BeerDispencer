using Newtonsoft.Json;

namespace BeerDispenser.Application.DTO.Authorization
{
    internal class FBUserProfileDto
    {
        [JsonProperty("email")]
        public string Email { get; set; }
        [JsonProperty("id")]
        public string Id { get; set; }
        [JsonProperty("first_name")]
        public string FirstName { get; set; }
        [JsonProperty("last_name")]
        public string LastName { get; set; }
        [JsonProperty("name")]
        public string Name { get; set; }
        public string PictureUrl { get;set; }
    }
}
