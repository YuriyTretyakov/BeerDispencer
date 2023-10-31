using BeerDispenser.Application.DTO;
using BeerDispenser.Shared;
using MediatR;

namespace BeerDispenser.Application.Implementation.Queries
{
    public class GetUserCardsQuery : IRequest<PaymentCardDto[]>
    {
        public Guid UserId { get; set; }
    }
}

