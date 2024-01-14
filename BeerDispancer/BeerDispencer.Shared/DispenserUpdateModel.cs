using BeerDispenser.Shared.Dto;
using Newtonsoft.Json;
namespace BeerDispenser.Shared

{
    public class DispenserUpdateModel
    {
        [JsonProperty("updatedAt")]
        public DateTimeOffset UpdatedAt { get; set; }

        [JsonProperty("status")]
        public DispenserStatusDto Status { get; set; }

    }
}

