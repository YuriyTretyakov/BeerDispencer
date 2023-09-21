using BeerDispencer.Shared;
using Newtonsoft.Json;
namespace BeerDispencer.WebApi.ViewModels.Request

{
    public class DispenserUpdateModel
    {
        [JsonProperty("updated_at")]
        public DateTime UpdatedAt { get; set; }

        [JsonProperty("status")]
        public DispenserStatus Status { get; set; }
        
    }
}

