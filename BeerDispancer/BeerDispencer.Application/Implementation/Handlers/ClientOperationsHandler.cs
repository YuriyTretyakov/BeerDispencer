using BeerDispenser.Application.Abstractions;
using BeerDispenser.Application.DTO;
using BeerDispenser.Application.Implementation.Commands;
using BeerDispenser.Application.Implementation.Messaging.Events;
using BeerDispenser.Application.Implementation.Messaging.Publishers;
using BeerDispenser.Domain.Abstractions;
using BeerDispenser.Domain.Entity;
using BeerDispenser.Kafka.Core;
using BeerDispenser.Shared;
using MediatR;

namespace BeerDispenser.Application.Implementation.Handlers
{
    public class ClientOperationsHandler : IRequestHandler<ClientOperationsCommand, PaymentRequiredDto>
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

        public async Task<PaymentRequiredDto> Handle(ClientOperationsCommand request, CancellationToken cancellationToken)
        {
            var dispencerDto = await _dispencerUof
                  .DispencerRepo
                  .GetByIdAsync(request.Id);

            var usagesDto = await _dispencerUof.UsageRepo.GetByDispencerIdAsync(request.Id);

            var usages = usagesDto.ToDomain(_beerFlowSettings);

            var dispenser = Dispenser.CreateDispenser(
                dispencerDto.Id,
                dispencerDto.Volume.Value,
                dispencerDto.Status.Value,
                dispencerDto.IsActive.Value,
                usages.ToList(),
                _beerFlowSettings);


            if (request.Status == DispenserStatusDto.Opened)
            {
                var usageDto = dispenser.Open().ToDto();
                await _dispencerUof.UsageRepo.AddAsync(usageDto);
                await _dispencerUof.DispencerRepo.UpdateAsync(dispenser.ToDto());
                await _dispencerUof.Complete();

                return new PaymentRequiredDto ();
            }

            else if (request.Status == DispenserStatusDto.Closed)
            {
                var recentUsage = dispenser.Close().ToDto();

                await _dispencerUof.UsageRepo.UpdateAsync(new UsageDto { Id = recentUsage.Id, PaymentStatus = PaymentStatusDto.Pending});

                var defaultCard = await _dispencerUof
                    .PaymentCardRepository
                    .GetDefaultCard(request.UserId);

                var paymentEvent = new EventHolder<PaymentToProcessEvent>(new PaymentToProcessEvent
                {
                    PaymentInitiatedBy = request.UserId,
                    Amount = (long)recentUsage.TotalSpent * 100,
                    Currency = "usd",
                    CustomerId = defaultCard.CustomerId,
                    CardId = defaultCard.CardId,
                    DIspenserId = dispencerDto.Id,
                    PaymentDescription = $"Payment for usage Dispenser {dispencerDto.Id} Amount: {recentUsage.TotalSpent} Volume: {recentUsage.FlowVolume}"
                });

                await _eventsTrigger.RaiseEventAsync(paymentEvent, cancellationToken);

                return new PaymentRequiredDto { PaymentId = recentUsage.Id.ToString() };
            }
            else
            {
                throw new NotImplementedException(nameof(request.Status));
            }
        }
    }
}

