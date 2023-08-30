using System;
using BeerDispancer.Application.DTO;
using BeerDispancer.Application.Implementation.Response;
using MediatR;
using Newtonsoft.Json;

namespace BeerDispancer.Application.Implementation.Commands
{
    public class DispencerCreateCommand:IRequest<DispencerDto>
    {
        [JsonProperty("flow_volume")]
        public decimal FlowVolume { get; set; }
    }
}

