using BeerDispencer.Application.Implementation.Response;
using BeerDispencer.Shared;
using MediatR;

namespace BeerDispancer.Application.Implementation.Commands
{
    public class DispencerUpdateCommand : IRequest<DispencerUpdateResponse>
    {
        public Guid Id { get; set; }
        public DispencerStatus Status { get; set; }
        public DateTime UpdatedAt { get; set; }
    }
}

