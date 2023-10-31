using System;
using BeerDispenser.Application.Abstractions;
using BeerDispenser.Application.Implementation.Commands.Payments;
using BeerDispenser.Domain.Implementations;
using MediatR;

namespace BeerDispenser.Application.Implementation.Handlers.Payments
{
	public class DeleteCardHandler: IRequestHandler<DeleteCardCommand, bool>
    {
        private readonly IDispencerUof _dispencerUof;

        public DeleteCardHandler(IDispencerUof dispencerUof)
		{
            _dispencerUof = dispencerUof;
        }

        public async Task<bool> Handle(DeleteCardCommand request, CancellationToken cancellationToken)
        {
            var userCards = await _dispencerUof.PaymentCardRepository.GetUserCards(request.UserId);

            var domainCards = userCards.Select(x => x.ToDomain());

            var cardManager = new PaymentCardManager(domainCards);

            var cardToDelete = domainCards.FirstOrDefault(x => x.Id == request.CardId);

            var (deletedCard, newDefaultcard) = cardManager.DeleteCard(cardToDelete);

            using (var transaction = _dispencerUof.StartTransaction())
            {
                await _dispencerUof.PaymentCardRepository.DeleteAsync(deletedCard.Id);

                if (newDefaultcard is not null)
                {
                    await _dispencerUof.PaymentCardRepository.UpdateAsync(newDefaultcard.ToDto());
                }

                await _dispencerUof.Complete();
                _dispencerUof.CommitTransaction();
            }
            return true;
        }
    }
}

