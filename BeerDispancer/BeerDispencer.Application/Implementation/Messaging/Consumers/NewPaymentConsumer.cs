using BeerDispenser.Application.Implementation.Messaging.Events;
using BeerDispenser.Application.Implementation.Messaging.Publishers;
using BeerDispenser.Messaging.Core;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace BeerDispenser.Application.Implementation.Messaging.Consumers
{
    public class NewPaymentConsumer : EventConsumerBase<NewPaymentEvent>
	{
        private readonly IServiceScopeFactory _serviceScopeFactory;

        public NewPaymentConsumer(
            IServiceScopeFactory serviceScopeFactory,
            ILogger<NewPaymentConsumer> logger,
            EventHubConfig configuration)
			: base(logger, configuration)
		{
            _serviceScopeFactory = serviceScopeFactory;
        }

        protected override async Task OnNewMessage(IReadonlyEventHolder<NewPaymentEvent> message, CancellationToken cancellationToken)
        {
            using var scope = _serviceScopeFactory.CreateScope();
            await ProcessMessageAsync(scope, message, cancellationToken);
        }

        private async Task ProcessMessageAsync(
            IServiceScope scope,
            IReadonlyEventHolder<NewPaymentEvent> message,
             CancellationToken cancellationToken)
        {
            var paymentToProcessPublisher = scope.ServiceProvider.GetService<PaymentToProcessPublisher>() ??
                                       throw new ArgumentNullException(nameof(PaymentToProcessPublisher));

            var paymentToProcessEvent = new EventHolder<PaymentInProccessEvent>(message.Event);
            await paymentToProcessPublisher.RaiseEventAsync(paymentToProcessEvent, cancellationToken);
        }
    }
}

