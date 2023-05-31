using System;
using Beerdispancer.Domain.Entities;
using Newtonsoft.Json;
namespace BeerDispencer.WebApi.Commands

{
    public class DispenserUpdateCommand
    {
        [JsonProperty("updated_at")]
        public DateTime UpdatedAt { get; set; }

        [JsonProperty("status")]
        public DispencerStatusDto Status { get; set; }
    }
}

