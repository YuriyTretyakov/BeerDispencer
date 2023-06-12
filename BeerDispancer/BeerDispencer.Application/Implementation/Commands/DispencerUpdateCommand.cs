using System;
using System.Net;
using BeerDispancer.Application.DTO;
using BeerDispencer.Application.DTO;
using BeerDispencer.Application.Implementation.Response;
using MediatR;

namespace BeerDispancer.Application.Implementation.Commands
{
    public class DispencerUpdateCommand : IRequest<DispencerUpdateResponse>
    {
        public Guid Id { get; set; }
        public DispencerStatusDto Status { get; set; }
        public DateTime UpdatedAt { get; set; }
    }
}

