using System;
using Newtonsoft.Json;

namespace BeerDispancer.PresentationLayer
{
    public class DispencerCreate
    {
        [JsonProperty("flow_volume")]
        public double FlowVolume { get; set; }
    }
}

