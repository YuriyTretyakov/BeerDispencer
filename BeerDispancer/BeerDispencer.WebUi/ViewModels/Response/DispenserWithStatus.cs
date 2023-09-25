
using BeerDispenser.Shared;
using Newtonsoft.Json;

namespace BeerDispenser.WebUi.ViewModels.Response
{
    public class DispenserWithStatus
	{
        public Guid Id { get; set; }
        public double Volume { get; set; }
        [JsonProperty("status")]
        public DispenserStatus Status { get; set; }
        public string ReservedFor { get; set; }
    }
}

