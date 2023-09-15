using BeerDispencer.Shared;
using Newtonsoft.Json;
namespace BeerDispencer.WebUi.ViewModels.Request

{
    public class DispenserUpdateModel
    {
        [JsonProperty("updated_at")]
        public DateTime UpdatedAt { get; set; }

        [JsonProperty("status")]
        public DispencerStatus Status { get; set; }
    }
}

