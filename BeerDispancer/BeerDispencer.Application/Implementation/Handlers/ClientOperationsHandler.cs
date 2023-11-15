using BeerDispenser.Application.Abstractions;
using BeerDispenser.Application.Implementation.Commands;
using BeerDispenser.Application.Implementation.Messaging.Events;
using BeerDispenser.Application.Implementation.Messaging.Publishers;
using BeerDispenser.Domain.Abstractions;
using BeerDispenser.Domain.Entity;
using BeerDispenser.Kafka.Core;
using MediatR;

namespace BeerDispenser.Application.Implementation.Handlers
{
    public class ClientOperationsHandler : IRequestHandler<ClientOperationsCommand>
	{
        private readonly IDispencerUof _dispencerUof;
        private readonly IBeerFlowSettings _beerFlowSettings;
        private readonly PaymentToProcessPublisher _eventsTrigger;

        public ClientOperationsHandler(
            IDispencerUof dispencerUof,
            IBeerFlowSettings beerFlowSettings,
            PaymentToProcessPublisher eventsTrigger)
		{
            _dispencerUof = dispencerUof;
            _beerFlowSettings = beerFlowSettings;
            _eventsTrigger = eventsTrigger;
        }

        public async Task Handle(ClientOperationsCommand request, CancellationToken cancellationToken)
        {
           
            if (request.Status == Shared.DispenserStatus.Closed)
            {
                var dispencerDto = await _dispencerUof
                   .DispencerRepo
                   .GetByIdAsync(request.Id);

                var usagesDto = await _dispencerUof.UsageRepo.GetByDispencerIdAsync(request.Id);

                var usages = usagesDto.ToDomain(_beerFlowSettings);

                var dispencer = Dispenser.CreateDispenser(
                    dispencerDto.Id,
                    dispencerDto.Volume.Value,
                    dispencerDto.Status.Value,
                    dispencerDto.IsActive.Value,
                    usages.ToList(),
                    _beerFlowSettings);

                var spendings = dispencer.GetSpendings();

                var recentUsage = spendings
                    .Usages
                    .FirstOrDefault(x => x.ClosedAt.Equals(spendings.Usages.Max(x => x.ClosedAt)));

                var amountToCharge = recentUsage.TotalSpent;

                var defaultCard = await _dispencerUof
                    .PaymentCardRepository
                    .GetDefaultCard(request.UserId);

                var paymentEvent = new EventHolder<PaymentToProcessEvent>(new PaymentToProcessEvent {
                    Amount = (long)amountToCharge * 100,
                    Currency = "usd",
                    CustomerId = defaultCard.CustomerId,
                    CardId = defaultCard.CardId,
                    PaymentDescription = $"Payment for usage Dispenser {dispencerDto.Id} Amount: {amountToCharge} Volume: {recentUsage.FlowVolume}"
                });

             //   _eventsTrigger.

                await _eventsTrigger.RaiseEventAsync(paymentEvent, cancellationToken);
            }
        }
    }
}

