using System.Threading;
using BeerDispenser.Application.Abstractions;
using BeerDispenser.Application.Implementation.Messaging.Consumers;
using BeerDispenser.Application.Implementation.Messaging.Events;
using BeerDispenser.Kafka.Core;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace BeerDispenser.Application.Services
{
    public class PaymentCompletedService : IHostedService
    {
        private readonly IServiceScopeFactory _serviceScopeFactory;
        private readonly PaymentCompletedConsumer _completedEventConsumer;
        private Task _consumingTask;
        CancellationToken _cancellationToken;

        public PaymentCompletedService(
            IServiceScopeFactory serviceScopeFactory,
            PaymentCompletedConsumer completedEventConsumer)
        {
            _serviceScopeFactory = serviceScopeFactory;
            _completedEventConsumer = completedEventConsumer;
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            _cancellationToken = cancellationToken;
            _completedEventConsumer.StartConsuming(cancellationToken);
            _completedEventConsumer.StartConsuming(cancellationToken);

            _consumingTask = Task
                           .Factory
                           .StartNew(
                                     ProcessConsumingAsync,
                                     cancellationToken,
                                     TaskCreationOptions.LongRunning,
                                     TaskScheduler.Default);

            return Task.CompletedTask;
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            _completedEventConsumer.Stop(cancellationToken);
            _cancellationToken = cancellationToken;
            return Task.CompletedTask;
        }

        private async void ProcessConsumingAsync()
        {
            while (!_cancellationToken.IsCancellationRequested)
            {
                var message = _completedEventConsumer.GetMessages();

                if (message is not null)
                {
                    await ProcessPaymentAsync(message);
                }
            }
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

