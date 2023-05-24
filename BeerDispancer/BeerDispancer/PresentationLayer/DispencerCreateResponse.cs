using System;
using Newtonsoft.Json;

namespace BeerDispancer.PresentationLayer
{
	public class DispencerCreateResponse
	{
        [JsonProperty("id")]
        public Guid Id { get; set; }
        [JsonProperty("flow_volume")]
        public double FlowVolume { get; set; }
    }
}

