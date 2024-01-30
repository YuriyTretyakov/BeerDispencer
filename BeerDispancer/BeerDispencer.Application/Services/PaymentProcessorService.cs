using BeerDispenser.Application.Implementation.Messaging.Consumers;
using BeerDispenser.Application.Implementation.Messaging.Events;
using BeerDispenser.Application.Implementation.Messaging.Publishers;
using BeerDispenser.Kafka.Core;
using BeerDispenser.Shared.Dto;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Stripe;

namespace BeerDispenser.Application.Services
{
    public class PaymentInprocessService : IHostedService
    {
        const int MAX_RETRY_COUNT= 5;

        private readonly IServiceScopeFactory _serviceScopeFactory;
        private readonly PaymentToProcessConsumer _toProcessConsumer;
        private CancellationToken _cancellationToken;
        private Task _consumingTask;

        public PaymentInprocessService(
            IServiceScopeFactory serviceScopeFactory,
            PaymentToProcessConsumer toProcessConsumer)
        {
            _serviceScopeFactory = serviceScopeFactory;
            _toProcessConsumer = toProcessConsumer;
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            _toProcessConsumer.OnNewMessage += OnNewMessage;

            _= _toProcessConsumer.StartAsync(cancellationToken);
           

             Task.Factory.StartNew(
                () =>
                {},
                TaskCreationOptions.LongRunning)
                .ConfigureAwait(false);

            return Task.CompletedTask;
        }

        private async void OnNewMessage(object sender, EventConsumerBase<PaymentToProcessEvent>.NewMessageEvent e)
        {
            if (e.Event is not null)
            {
                using var scope = _serviceScopeFactory.CreateScope();
                await ProcessMessageAsync(scope, e.Event);
             //   _toProcessConsumer.Commit(e.Event);
            }
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            _toProcessConsumer.Stop(cancellationToken);
            _toProcessConsumer.Dispose();
            return Task.CompletedTask;
        }

        private async Task ProcessMessageAsync(
            IServiceScope scope,
            IReadonlyEventHolder<PaymentToProcessEvent> message)
        {
            EventPublisher<PaymentCompletedEvent> paymentCompletedTrigger = null;

            EventPublisher<PaymentToProcessEvent> paymentToProcessTrigger = null;

            try
            {
                paymentCompletedTrigger = scope.ServiceProvider.GetService<PaymentCompletedPublisher>()??
                                             throw new Exception(nameof(PaymentCompletedPublisher));

                paymentToProcessTrigger = scope.ServiceProvider.GetService<PaymentToProcessPublisher>() ??
                                          throw new Exception(nameof(PaymentToProcessPublisher));


                var options = new ChargeCreateOptions
                {
                    Customer = message.Event.CustomerId,
                    Amount = message.Event.Amount,
                    Currency = message.Event.Currency,
                    Source = message.Event.CardId,
                    Description = message.Event.PaymentDescription
                };

                var service = new ChargeService();
                var charge = service.Create(options);

                if (charge.Status.Equals("succeeded", StringComparison.OrdinalIgnoreCase))
                {
                    var sucessEvent = new PaymentCompletedEvent(message.Event, PaymentStatusDto.Success);

                    var completedEventHolder = new PaymentCompletedEventHolder(sucessEvent, message.RetryCount, message.CorrelationId);
                    await paymentCompletedTrigger.RaiseEventAsync(completedEventHolder, _cancellationToken);
                }

                else
                {
                    if (message.RetryCount > MAX_RETRY_COUNT)
                    {
                        var exceededRetryEvent = new PaymentCompletedEvent(message.Event, PaymentStatusDto.Failed, charge.Status.ToString());

                        var completedEventHolder = new PaymentCompletedEventHolder(exceededRetryEvent, message.RetryCount, message.CorrelationId);
                        await paymentCompletedTrigger.RaiseEventAsync(completedEventHolder, _cancellationToken);
                    }

                    //TODO: Identify all use cases with possibility to fix by retry

                    await paymentToProcessTrigger.RetryAsync(message, _cancellationToken);
                    return;
                }
            }

            catch (Exception ex)
            {
                var completedEvent = new PaymentCompletedEvent(message.Event, PaymentStatusDto.Failed, ex.Message);
                var completedEventHolder = new PaymentCompletedEventHolder(completedEvent, message.RetryCount, message.CorrelationId);
                await paymentCompletedTrigger.RaiseEventAsync(completedEventHolder, _cancellationToken);
            }
        }
    }
}