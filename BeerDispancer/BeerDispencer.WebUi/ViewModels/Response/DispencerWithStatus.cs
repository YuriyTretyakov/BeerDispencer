using BeerDispencer.Shared;
using Newtonsoft.Json;

namespace BeerDispencer.WebUi.ViewModels.Response
{
    public class DispencerWithStatus
	{
        public Guid Id { get; set; }
        public double Volume { get; set; }
        [JsonProperty("status")]
        public DispencerStatus Status { get; set; }
    }
}

