using BeerDispencer.Application.Implementation.Response;
using BeerDispencer.Shared;
using MediatR;

namespace BeerDispancer.Application.Implementation.Commands
{
    public class DispenserUpdateCommand : IRequest<DispenserUpdateResponse>
    {
        public Guid Id { get; set; }
        public DispenserStatus Status { get; set; }
        public DateTime UpdatedAt { get; set; }
    }
}

