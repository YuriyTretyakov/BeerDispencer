using System;
using BeerDispencer.WebApi.Responses;
using MediatR;
using Newtonsoft.Json;

namespace BeerDispencer.WebApi.Commands
{
    public class DispencerCreateCommand:IRequest<DispencerResponse>
    {
        [JsonProperty("flow_volume")]
        public double FlowVolume { get; set; }
    }
}

