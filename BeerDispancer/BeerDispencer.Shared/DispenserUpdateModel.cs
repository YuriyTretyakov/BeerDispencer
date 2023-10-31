using Newtonsoft.Json;
namespace BeerDispenser.Shared

{
    public class DispenserUpdateModel
    {
        [JsonProperty("updatedAt")]
        public DateTimeOffset UpdatedAt { get; set; }

        [JsonProperty("status")]
        public DispenserStatus Status { get; set; }

    }
}

