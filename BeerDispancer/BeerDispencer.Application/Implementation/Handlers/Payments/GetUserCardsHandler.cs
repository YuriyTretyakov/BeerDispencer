using BeerDispenser.Application.Abstractions;
using BeerDispenser.Application.DTO;
using BeerDispenser.Application.Implementation.Queries;
using MediatR;

namespace BeerDispenser.Application.Implementation.Handlers.Payments
{
    public class GetUserCardsHandler:IRequestHandler<GetUserCardsQuery, PaymentCardDto[]>
	{
        private readonly IDispencerUof _dispencerUof;

        public GetUserCardsHandler(IDispencerUof dispencerUof)
		{
            _dispencerUof = dispencerUof;
        }

        public async Task<PaymentCardDto[]> Handle(GetUserCardsQuery request, CancellationToken cancellationToken)
        {
            var cards = await _dispencerUof.PaymentCardRepository.GetUserCards(request.UserId);
            return cards.OrderBy(x=>x.Created).ToArray();
        }
    }
}

