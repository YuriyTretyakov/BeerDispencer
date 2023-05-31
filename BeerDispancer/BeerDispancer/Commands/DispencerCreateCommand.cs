using System;
using Newtonsoft.Json;

namespace BeerDispencer.WebApi.Commands
{
    public class DispencerCreateCommand
    {
        [JsonProperty("flow_volume")]
        public double FlowVolume { get; set; }
    }
}

