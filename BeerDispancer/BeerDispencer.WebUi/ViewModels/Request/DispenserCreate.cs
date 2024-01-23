using System.Text.Json.Serialization;

namespace BeerDispenser.WebUi.ViewModels.Request
{
    public class DispenserCreate
    {
        [JsonPropertyName("flow_volume")]
        public decimal FlowVolume { get; set; }
    };
}


