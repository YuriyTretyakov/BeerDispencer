using System.Threading;
using BeerDispenser.Application.Abstractions;
using BeerDispenser.Application.Implementation.Messaging.Consumers;
using BeerDispenser.Application.Implementation.Messaging.Events;
using BeerDispenser.Domain.Abstractions;
using BeerDispenser.Domain.Entity;
using BeerDispenser.Kafka.Core;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;


namespace BeerDispenser.Application.Services
{
    public class PaymentCompletedService : IHostedService
    {
        private readonly ILogger<PaymentCompletedService> _logger;
        private readonly IServiceScopeFactory _serviceScopeFactory;
        private readonly PaymentCompletedConsumer _completedEventConsumer;
        private readonly IBeerFlowSettings _beerFlowSettings;
        CancellationToken _cancellationToken;

        public PaymentCompletedService(ILogger<PaymentCompletedService> logger,
            IServiceScopeFactory serviceScopeFactory,
            PaymentCompletedConsumer completedEventConsumer,
            IBeerFlowSettings beerFlowSettings)
        {
            _logger = logger;
            _serviceScopeFactory = serviceScopeFactory;
            _completedEventConsumer = completedEventConsumer;
            _beerFlowSettings = beerFlowSettings;
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            _completedEventConsumer.OnNewMessage += OnNewMessage;

            _ = _completedEventConsumer.StartAsync(cancellationToken);
            
             Task.Factory.StartNew(
                () =>
                {},
                TaskCreationOptions.LongRunning)
                .ConfigureAwait(false);

            return Task.CompletedTask;
        }

        private async void OnNewMessage(object sender, EventConsumerBase<PaymentCompletedEvent>.NewMessageEvent e)
        {
            if (e.Event is not null)
            {
                await ProcessPaymentAsync(e.Event);
              //  _completedEventConsumer.Commit(e.Event);
            }
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            _completedEventConsumer.Stop(_cancellationToken);
            _completedEventConsumer.Dispose();
            return Task.CompletedTask;
        }

        private async Task ProcessPaymentAsync(
            EventHolder<PaymentCompletedEvent> message)
        {
            try
            {

            using var scope = _serviceScopeFactory.CreateScope();

            var uow = scope.ServiceProvider.GetRequiredService<IDispencerUof>();

            using (var transaction = uow.StartTransaction())
            {
                _logger.LogInformation("Transaction started");

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

                _logger.LogInformation("Usages table updated {@usage}", usageDto);

                await uow.DispencerRepo.UpdateAsync(dispenser.ToDto());

                _logger.LogInformation("dispenser table updated {@usage}", dispenser.ToDto());
                await uow.Complete();
                uow.CommitTransaction();
                _logger.LogInformation("Data saved");

                }
            
            }
            catch(Exception ex)
            {
                _logger.LogError("Error: {@err}",ex);
            }
        }
    }

}