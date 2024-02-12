using BeerDispenser.Application.Abstractions;
using BeerDispenser.Application.DTO;
using BeerDispenser.Application.Implementation.Commands;
using BeerDispenser.Application.Implementation.Messaging.Events;
using BeerDispenser.Application.Implementation.Messaging.Publishers;
using BeerDispenser.Domain.Abstractions;
using BeerDispenser.Domain.Entity;
using BeerDispenser.Messaging.Core;
using BeerDispenser.Shared.Dto;
using MediatR;
using Newtonsoft.Json;

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

                EventHolder<PaymentToProcessEvent> paymentEvent;
                OutboxDto outboxEntry;

                using (var transaction = _dispencerUof.StartTransaction())
                {
                    await _dispencerUof.UsageRepo.UpdateAsync(new UsageDto { Id = recentUsage.Id, PaymentStatus = PaymentStatusDto.Pending });

                    var defaultCard = await _dispencerUof
                    .PaymentCardRepository
                    .GetDefaultCard(request.UserId);

                    var amount = ((long)recentUsage.TotalSpent * 100) < 50 ? 50 : ((long)recentUsage.TotalSpent * 100);

                    paymentEvent = new EventHolder<PaymentToProcessEvent>(new PaymentToProcessEvent
                    {
                        PaymentInitiatedBy = request.UserId,
                        Amount = amount,
                        Currency = "usd",
                        CustomerId = defaultCard.CustomerId,
                        CardId = defaultCard.CardId,
                        DIspenserId = dispencerDto.Id,
                        PaymentDescription = $"Payment for usage Dispenser {dispencerDto.Id} Amount: {recentUsage.TotalSpent} Volume: {recentUsage.FlowVolume}"
                    });


                    outboxEntry = new OutboxDto
                    {
                        Id = Guid.NewGuid(),
                        EventType = typeof(EventHolder<PaymentToProcessEvent>).ToString(),
                        Payload = JsonConvert.SerializeObject(paymentEvent),
                        CreatedAt = DateTime.UtcNow,
                        EventState = EventStateDto.Created
                    };

                    await _dispencerUof.OutboxRepo.AddAsync(outboxEntry);

                    await _dispencerUof.Complete();
                    _dispencerUof.CommitTransaction();
                }
               

                await _eventsTrigger.RaiseEventAsync(paymentEvent, cancellationToken);

                // how to handle if event sent to messaging but status update in db leads to error?

                outboxEntry.EventState = EventStateDto.Completed;
                outboxEntry.UpdatedAt = DateTime.UtcNow;
                await _dispencerUof.OutboxRepo.UpdateAsync(outboxEntry);
                await _dispencerUof.Complete();

                return new PaymentRequiredDto { PaymentId = recentUsage.Id.ToString() };
            }
            else
            {
                throw new NotImplementedException(nameof(request.Status));
            }
        }
    }
}

