using System;
using BeerDispancer.DomainLayer;
using Newtonsoft.Json;
namespace BeerDispancer.PresentationLayer

{
    public class DispenserUpdate
    {
        [JsonProperty("updated_at")]
        public DateTime UpdatedAt { get; set; }

        [JsonProperty("status")]
        public DispencerStatusDto Status { get; set; }
    }
}

