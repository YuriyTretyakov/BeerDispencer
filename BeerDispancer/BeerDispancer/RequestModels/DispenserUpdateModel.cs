using System;
using System.Net;
using Beerdispancer.Domain.Entities;
using MediatR;
using Newtonsoft.Json;
namespace BeerDispencer.WebApi.RequestModels

{
    public class DispenserUpdateModel
    {
        [JsonProperty("updated_at")]
        public DateTime UpdatedAt { get; set; }

        [JsonProperty("status")]
        public DispencerStatusDto Status { get; set; }
    }
}

