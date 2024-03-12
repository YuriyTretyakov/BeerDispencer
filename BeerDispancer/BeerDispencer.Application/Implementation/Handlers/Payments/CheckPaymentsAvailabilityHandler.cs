using BeerDispenser.Application.Abstractions;
using BeerDispenser.Application.Implementation.Queries;
using BeerDispenser.Shared.Dto.Payments;
using MediatR;

namespace BeerDispenser.Application.Implementation.Handlers.Payments
{
    public class CheckPaymentsAvailabilityHandler : IRequestHandler<CheckPaymentAvailabilityQuery, PaymentAvailabilityDto>
    {
        private readonly IDispencerUof _dispencerUof;

        public CheckPaymentsAvailabilityHandler(IDispencerUof dispencerUof)
        {
            _dispencerUof = dispencerUof;
        }
        public async Task<PaymentAvailabilityDto> Handle(CheckPaymentAvailabilityQuery request, CancellationToken cancellationToken)
        {
            var cards = await _dispencerUof.PaymentCardRepository.GetUserCards(request.UserId);

            if (cards?.Any() is not true)
            {
                return new PaymentAvailabilityDto { IsAvailable = false, Details = "You didn't add payment methods to your account" };
            }

            return new PaymentAvailabilityDto { IsAvailable = true };
        }
    }
}
