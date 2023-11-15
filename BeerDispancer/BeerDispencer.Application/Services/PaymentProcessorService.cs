using BeerDispenser.Application.Implementation.Messaging.Consumers;
using BeerDispenser.Application.Implementation.Messaging.Events;
using BeerDispenser.Application.Implementation.Messaging.Publishers;
using BeerDispenser.Kafka.Core;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Stripe;

namespace BeerDispenser.Application.Services
{
    public class PaymentInprocessService : IHostedService /*, IDisposable */
    {
        const int MAX_RETRY_COUNT= 5;

        private readonly IServiceScopeFactory _serviceScopeFactory;
        private readonly PaymentToProcessConsumer _toProcessConsumer;
        private CancellationToken _cancellationToken;
        private Thread _consumingThread;

        public PaymentInprocessService(
            IServiceScopeFactory serviceScopeFactory,
            PaymentToProcessConsumer toProcessConsumer)

        {
            _serviceScopeFactory = serviceScopeFactory;
            _toProcessConsumer = toProcessConsumer;
            _consumingThread = new Thread(ProcessConsuming);
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            _cancellationToken = cancellationToken;
            _toProcessConsumer.StartConsuming(cancellationToken);
            _consumingThread.Start();
           
             return Task.CompletedTask;
        }

        public Task StopAsync(CancellationToken cancellationToken)
        { 
            _toProcessConsumer.Stop(cancellationToken);
            _consumingThread.Join();
            return Task.CompletedTask;
        }

        private void ProcessConsuming()
        {
            while (!_cancellationToken.IsCancellationRequested)
            {
                var message = _toProcessConsumer.GetMessages();

                if (message is not null)
                {
                    using var scope = _serviceScopeFactory.CreateScope();
                    ProcessMessageAsync(
                      scope,
                      message,
                      _cancellationToken).ConfigureAwait(false);
                }
                Thread.Yield();
            }
        }

        private async Task ProcessMessageAsync(
            IServiceScope scope,
            IReadonlyEventHolder<PaymentToProcessEvent> message,
            CancellationToken cancellationToken)
        {
            IEventPublisher<PaymentCompletedEvent> paymentCompletedTrigger = null;

            IEventPublisher<PaymentToProcessEvent> paymentToProcessTrigger = null;

            try
            {
                paymentCompletedTrigger = scope.ServiceProvider.GetService<PaymentCompletedPublisher>() ??
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
                    var sucessEvent = new PaymentCompletedEvent(message, PaymentStatus.Success);
                    await paymentCompletedTrigger.RaiseEventAsync(new EventHolder<PaymentCompletedEvent>(sucessEvent), cancellationToken);
                }

                else
                {
                    if (message.Event.RetryCount > MAX_RETRY_COUNT)
                    {
                        var exceededRetryEvent = new PaymentCompletedEvent(message, PaymentStatus.Failed, charge.Status.ToString());
                        await paymentCompletedTrigger.RaiseEventAsync(new EventHolder<PaymentCompletedEvent>(exceededRetryEvent), cancellationToken);
                    }

                    //TODO: Identify all use cases with possibility to fix by retry

                    await paymentToProcessTrigger.RetryAsync(message, cancellationToken);
                    return;
                }
            }

            catch (Exception ex)
            {
                var completedEvent = new PaymentCompletedEvent(message, PaymentStatus.Failed, ex.Message);
                await paymentCompletedTrigger?.RaiseEventAsync(new EventHolder<PaymentCompletedEvent>(completedEvent), cancellationToken);
            }
        }
    }
}