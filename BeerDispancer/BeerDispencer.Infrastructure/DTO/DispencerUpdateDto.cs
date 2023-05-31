using System;
using Beerdispancer.Domain.Entities;
using BeerDispancer.Application.Abstractions;

namespace Beerdispancer.Infrastructure.DTO
{
    public class DispencerUpdateDto : IDispencerUpdate
    {
        public Guid Id { get; set; }
        public DispencerStatusDto Status { get; set; }
        public DateTime UpdatedAt { get; set; }
    }
}

