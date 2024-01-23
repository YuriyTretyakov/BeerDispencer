using BeerDispenser.Application.Abstractions;
using BeerDispenser.Application.Implementation.Commands.Payments;
using BeerDispenser.Domain.Implementations;
using MediatR;

namespace BeerDispenser.Application.Implementation.Handlers.Payments
{
    public class SetDefaultCardHandler : IRequestHandler<SetDefaultCardCommand, bool>
    {
        private readonly IDispencerUof _dispencerUof;

        public SetDefaultCardHandler(IDispencerUof dispencerUof)
		{
            _dispencerUof = dispencerUof;
        }

        public async Task<bool> Handle(SetDefaultCardCommand request, CancellationToken cancellationToken)
        {
            var userCards = await _dispencerUof.PaymentCardRepository.GetUserCards(request.UserId);

            var domainCards = userCards.Select(x => x.ToDomain());

            var cardManager = new PaymentCardManager(domainCards);

            var cardToSetDefault = domainCards.FirstOrDefault(x => x.Id == request.CardId);

            var (initialDefaultcard, newDefaultcard) = cardManager.SetDefaultCard(cardToSetDefault);

            using (var transaction = _dispencerUof.StartTransaction())
            {
                await _dispencerUof.PaymentCardRepository.UpdateAsync(initialDefaultcard.ToDto());
                await _dispencerUof.PaymentCardRepository.UpdateAsync(newDefaultcard.ToDto());

                await _dispencerUof.Complete();
                _dispencerUof.CommitTransaction();
            }

            return true;
        }
    }
}

