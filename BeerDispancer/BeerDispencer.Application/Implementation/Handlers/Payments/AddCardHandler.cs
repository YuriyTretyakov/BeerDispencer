using BeerDispenser.Application.Abstractions;
using BeerDispenser.Application.Implementation.Commands.Payments;
using BeerDispenser.Domain.Entity;
using BeerDispenser.Domain.Implementations;
using MediatR;
using Stripe;

namespace BeerDispenser.Application.Implementation.Handlers.Payments
{
    public class AddCardHandler : IRequestHandler<AddPaymentDetailsCommand, bool>
    {
        private readonly IDispencerUof _dispencerUof;

        public AddCardHandler(IDispencerUof dispencerUof)
        {
            _dispencerUof = dispencerUof;
        }

        public async Task<bool> Handle(AddPaymentDetailsCommand request, CancellationToken cancellationToken)
        {
            var userCards = await _dispencerUof.PaymentCardRepository.GetUserCards(request.UserId);
            var domainCards = userCards.Select(x => x.ToDomain());

            var cardManager = new PaymentCardManager(domainCards);

            var tokenObject = request.StripeData.Token;

            var customerService = new CustomerService();
            var customerCreateOptions = new CustomerCreateOptions
            {
                Name = tokenObject.Card.Name,
                Email = tokenObject.Email,
                Address = new AddressOptions {
                    City = tokenObject.Card.City,
                    Country = tokenObject.Card.AdressCountry,
                    Line1 = tokenObject.Card.Line1 },
                Source = tokenObject.Id
            };


            var customer = await customerService.CreateAsync(customerCreateOptions);


            var newCard = request.StripeData.ToDomain(customer.Id, request.UserId);

            var newCardDto = cardManager
                .AddCard(newCard)
                .ToDto();

            await _dispencerUof.PaymentCardRepository.AddAsync(newCardDto);
            await _dispencerUof.Complete();
            return true;
        }
    }
}

