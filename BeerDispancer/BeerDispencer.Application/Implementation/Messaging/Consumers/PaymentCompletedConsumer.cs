using BeerDispenser.Application.Abstractions;
using BeerDispenser.Application.Implementation.Messaging.Events;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using BeerDispenser.Domain.Abstractions;
using BeerDispenser.Domain.Entity;
using BeerDispenser.Messaging.Core;

namespace BeerDispenser.Application.Implementation.Messaging.Consumers
{
    public class PaymentCompletedConsumer : EventConsumerBase<PaymentCompletedEvent>
	{
        private readonly IServiceScopeFactory _serviceScopeFactory;
        private readonly IBeerFlowSettings _beerFlowSettings;

        public PaymentCompletedConsumer(
            IServiceScopeFactory serviceScopeFactory,
            ILogger<PaymentCompletedConsumer> logger,
            IBeerFlowSettings beerFlowSettings,

            EventHubConfig configuration)
			: base(logger, configuration)
		{
            _serviceScopeFactory = serviceScopeFactory;
            _beerFlowSettings = beerFlowSettings;
        }

        protected override async Task OnNewMessage(IReadonlyEventHolder<PaymentCompletedEvent> message, CancellationToken cancellationToken)
        {
            await ProcessPaymentAsync(message);
        }

        private async Task ProcessPaymentAsync(
            IReadonlyEventHolder<PaymentCompletedEvent> message)
        {
            try
            {
                using var scope = _serviceScopeFactory.CreateScope();

                using var uow = scope.ServiceProvider.GetRequiredService<IDispencerUof>();
                using var transaction = uow.StartTransaction();

                LogInfo("Transaction started");
               

                var dispencerDto = await uow
                  .DispencerRepo
                  .GetByIdAsync(message.Event.OriginalEvent.DIspenserId);

                var usagesDto = await uow.UsageRepo.GetByDispencerIdAsync(dispencerDto.Id);

                var usages = usagesDto.ToDomain(_beerFlowSettings);

                var dispenser = Dispenser.CreateDispenser(
                    dispencerDto.Id,
                    dispencerDto.Volume.Value,
                    dispencerDto.Status.Value,
                    dispencerDto.IsActive.Value,
                    usages.ToList(),
                    _beerFlowSettings);

                var usageDto = dispenser.Close().ToDto();

                usageDto.PaidBy = message.Event.OriginalEvent.PaymentInitiatedBy;
                usageDto.PaymentStatus = message.Event.Status;
                usageDto.Reason = message.Event.Reason;

                await uow.UsageRepo.UpdateAsync(usageDto);

                LogInfo("Usages table updated {@usage}", usageDto);

                await uow.DispencerRepo.UpdateAsync(dispenser.ToDto());
                LogInfo("dispenser table updated {@usage}", dispenser.ToDto());

                await uow.Complete();
                uow.CommitTransaction();
                LogInfo("Data saved");
            }
            catch (Exception ex)
            {
                LogError("Error: {@err}", ex);
            }
        }
    }
}