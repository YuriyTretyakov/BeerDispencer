using BeerDispenser.Application.Implementation.Messaging.Consumers;
using BeerDispenser.Application.Implementation.Messaging.Events;
using BeerDispenser.Application.Implementation.Messaging.Publishers;
using BeerDispenser.Messaging.Core;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace BeerDispenser.Application.Services
{
    public class NewPaymentService : IHostedService
    {
        private readonly IServiceScopeFactory _serviceScopeFactory;
        private readonly NewPaymentConsumer _newPaymentConsumer;
        private CancellationToken _cancellationToken;
        private Task _consumingTask;

        public NewPaymentService(
            IServiceScopeFactory serviceScopeFactory,
            NewPaymentConsumer newPaymentConsumer)
        {
            _serviceScopeFactory = serviceScopeFactory;
            _newPaymentConsumer = newPaymentConsumer;
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            _newPaymentConsumer.OnNewMessage += OnNewMessage;

            _= _newPaymentConsumer.Start(cancellationToken);
           
             Task.Factory.StartNew(
                () =>
                {},
                TaskCreationOptions.LongRunning)
                .ConfigureAwait(false);

            return Task.CompletedTask;
        }

        private async void OnNewMessage(object sender, EventConsumerBase<NewPaymentEvent>.NewMessageEvent e)
        {
            if (e.Event is not null)
            {
                using var scope = _serviceScopeFactory.CreateScope();
                await ProcessMessageAsync(scope, e.Event);
            }
        }

        public async Task StopAsync(CancellationToken cancellationToken)
        {
           await _newPaymentConsumer.Stop(cancellationToken);
        }

        private async Task ProcessMessageAsync(
            IServiceScope scope,
            IReadonlyEventHolder<NewPaymentEvent> message)
        {
            var paymentToProcessPublisher = scope.ServiceProvider.GetService<PaymentToProcessPublisher>() ??
                                       throw new Exception(nameof(PaymentToProcessPublisher));

            var paymentToProcessEvent = new EventHolder<PaymentInProccessEvent>(message.Event);
            await paymentToProcessPublisher.RaiseEventAsync(paymentToProcessEvent, CancellationToken.None);
        }
    }
}