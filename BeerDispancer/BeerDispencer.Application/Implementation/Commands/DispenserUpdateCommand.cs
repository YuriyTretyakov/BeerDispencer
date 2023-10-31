using BeerDispenser.Application.Implementation.Response;
using BeerDispenser.Shared;
using MediatR;

namespace BeerDispenser.Application.Implementation.Commands
{
    public class DispenserUpdateCommand : IRequest<DispenserUpdateResponse>
    {
        public Guid Id { get; set; }
        public DispenserStatus Status { get; set; }
        public DateTimeOffset UpdatedAt { get; set; }
    }
}

