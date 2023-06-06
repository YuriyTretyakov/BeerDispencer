using System;
using System.Net;
using Beerdispancer.Domain.Entities;
using BeerDispancer.Application.Abstractions;
using BeerDispencer.WebApi.Responses;
using MediatR;

namespace BeerDispencer.WebApi.Commands
{
    public class DispencerUpdateCommand : IRequest<bool>, IDispencerUpdate
    {
        public Guid Id { get; set; }
        public DispencerStatusDto Status { get; set; }
        public DateTime UpdatedAt { get; set; }
    }
}

