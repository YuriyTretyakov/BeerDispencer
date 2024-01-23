using System;
using BeerDispenser.Application.DTO;
using BeerDispenser.Application.Implementation.Response;
using MediatR;
using Newtonsoft.Json;

namespace BeerDispenser.Application.Implementation.Commands
{
    public class DispenserCreateCommand:IRequest<DispenserDto>
    {
        [JsonProperty("flow_volume")]
        public decimal FlowVolume { get; set; }
    }
}

