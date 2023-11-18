using BeerDispenser.Application.Abstractions;
using BeerDispenser.Application.Implementation.Messaging.Consumers;
using BeerDispenser.Application.Implementation.Messaging.Events;
using BeerDispenser.Kafka.Core;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace BeerDispenser.Application.Services
{
    public class PaymentCompletedService : BackgroundService
    {
        private readonly IServiceScopeFactory _serviceScopeFactory;
        private readonly PaymentCompletedConsumer _completedEventConsumer;
        CancellationToken _cancellationToken;

        public PaymentCompletedService(
            IServiceScopeFactory serviceScopeFactory,
            PaymentCompletedConsumer completedEventConsumer)
        {
            _serviceScopeFactory = serviceScopeFactory;
            _completedEventConsumer = completedEventConsumer;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _cancellationToken = stoppingToken;
            _completedEventConsumer.StartConsuming(stoppingToken);
            ProcessConsumingAsync();
        }

        private async Task ProcessConsumingAsync()
        {
            while (!_cancellationToken.IsCancellationRequested)
            {
                var message =  await _completedEventConsumer.ConsumeAsync(_cancellationToken);

                if (message is not null)
                {
                    await ProcessPaymentAsync(message);
                }
            }
            _completedEventConsumer.Stop(_cancellationToken);
            _completedEventConsumer.Dispose();
        }

        private async Task ProcessPaymentAsync(
            IReadonlyEventHolder<PaymentCompletedEvent> message)
        {
            using var scope = _serviceScopeFactory.CreateScope();

            var uow = scope.ServiceProvider.GetRequiredService<IDispencerUof>();

            ///uow.UsageRepo. save payment to db

        }
    }
}

