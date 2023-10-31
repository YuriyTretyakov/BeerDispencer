using System;
using BeerDispenser.Domain.Entity;

namespace BeerDispenser.Domain.Implementations
{
	public class PaymentCardManager
	{
        List<PaymentCard> _paymentCards = new();


        public bool PaymentsAllowed => _paymentCards.Any();


        public PaymentCardManager(IEnumerable<PaymentCard> paymentCards)
		{
			_paymentCards = paymentCards.ToList();
        }

		public PaymentCard AddCard(PaymentCard paymentCard)
		{
			if (paymentCard == null)
			{
				throw new ArgumentNullException(nameof(paymentCard));
			}

			paymentCard.Validate();

			if (_paymentCards.Any(x=>x.Id == paymentCard.Id))
			{
                throw new InvalidOperationException($"Card {paymentCard} already added");
            }

            if (!_paymentCards.Any())
			{
				paymentCard.IsDefault = true;
            }

			_paymentCards.Add(paymentCard);

			return paymentCard;
        }

		public (PaymentCard RemovedCard, PaymentCard NewDefaultCard) DeleteCard(PaymentCard paymentCard)
		{
            if (paymentCard is null)
            {
                throw new InvalidOperationException($"Card {paymentCard} not found");
            }

            PaymentCard removedCard, newDefaultCard = null;

           if (! _paymentCards.Remove(paymentCard))
			{
                throw new InvalidOperationException($"Card {paymentCard} not found");
            }

            removedCard = paymentCard;

            if (paymentCard.IsDefault)
            {
                var firstCard = _paymentCards.FirstOrDefault();

                if (firstCard != null)
                {
                    SetDefaultCard(firstCard);
                }

                newDefaultCard = firstCard;
            }

            return (removedCard, newDefaultCard);
        }

        public (PaymentCard InitialDefaultCard, PaymentCard NewDefaultCard) SetDefaultCard(PaymentCard paymentCard)
        {
            if (paymentCard is null)
            {
                throw new InvalidOperationException($"Card {paymentCard} not found");
            }

            PaymentCard initialDefaultCard, newDefaultCard = null;

            var card = _paymentCards.FirstOrDefault(x => x.Id == paymentCard.Id); ;

            if (card is null)
            {
                throw new InvalidOperationException($"Card {paymentCard} not found");
            }

            initialDefaultCard = _paymentCards.FirstOrDefault(x => x.IsDefault);

            if (initialDefaultCard is not null)
            {
                initialDefaultCard.IsDefault = false;
            }

            newDefaultCard = card;
            newDefaultCard.IsDefault = true;

            return (initialDefaultCard, newDefaultCard);
        }
    }
}

