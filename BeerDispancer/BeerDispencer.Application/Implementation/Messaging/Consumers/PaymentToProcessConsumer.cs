using BeerDispenser.Application.Implementation.Messaging.Events;
using BeerDispenser.Application.Implementation.Messaging.Publishers;
using BeerDispenser.Messaging.Core;
using BeerDispenser.Shared.Dto.Payments;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Stripe;

namespace BeerDispenser.Application.Implementation.Messaging.Consumers
{
    public class PaymentToProcessConsumer : EventConsumerBase<PaymentInProccessEvent>
	{
        const int MAX_RETRY_COUNT = 5;

        private readonly IServiceScopeFactory _serviceScopeFactory;

        public PaymentToProcessConsumer(
            IServiceScopeFactory serviceScopeFactory,
            ILogger<PaymentToProcessConsumer> logger,
            EventHubConfig configuration)
			: base(logger, configuration)
		{
            _serviceScopeFactory = serviceScopeFactory;
        }

        protected override async Task OnNewMessage(IReadonlyEventHolder<PaymentInProccessEvent> message, CancellationToken cancellationToken)
        {

            using var scope = _serviceScopeFactory.CreateScope();
            await ProcessMessageAsync(scope, message, cancellationToken);
        }

        private async Task ProcessMessageAsync(
            IServiceScope scope,
            IReadonlyEventHolder<PaymentInProccessEvent> message,
            CancellationToken cancellationToken)
        {
            EventPublisher<PaymentCompletedEvent> paymentCompletedTrigger = null;

            EventPublisher<PaymentInProccessEvent> paymentToProcessTrigger = null;

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
                    var sucessEvent = new PaymentCompletedEvent(message.Event, PaymentStatusDto.Success);

                    var completedEventHolder = new PaymentCompletedEventHolder(sucessEvent, message.RetryCount, message.CorrelationId);
                    await paymentCompletedTrigger.RaiseEventAsync(completedEventHolder, cancellationToken);
                }

                else
                {
                    if (message.RetryCount > MAX_RETRY_COUNT)
                    {
                        var exceededRetryEvent = new PaymentCompletedEvent(message.Event, PaymentStatusDto.Failed, charge.Status.ToString());

                        var completedEventHolder = new PaymentCompletedEventHolder(exceededRetryEvent, message.RetryCount, message.CorrelationId);
                        await paymentCompletedTrigger.RaiseEventAsync(completedEventHolder, cancellationToken);
                    }

                    //TODO: Identify all use cases with possibility to fix by retry

                    await paymentToProcessTrigger.RetryAsync(message, cancellationToken);
                    return;
                }
            }

            catch (Exception ex)
            {
                var completedEvent = new PaymentCompletedEvent(message.Event, PaymentStatusDto.Failed, ex.Message);
                var completedEventHolder = new PaymentCompletedEventHolder(completedEvent, message.RetryCount, message.CorrelationId);
                await paymentCompletedTrigger.RaiseEventAsync(completedEventHolder, cancellationToken);
            }
        }
    }
}

