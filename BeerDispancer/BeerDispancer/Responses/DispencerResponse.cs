using System;
using Newtonsoft.Json;

namespace BeerDispencer.WebApi.Responses
{
	public class DispencerResponse
	{
        [JsonProperty("id")]
        public Guid Id { get; set; }
        [JsonProperty("flow_volume")]
        public double FlowVolume { get; set; }
    }
}

