using BeerDispenser.Application.Implementation.Messaging.Events;
using BeerDispenser.Messaging.Core;
using Microsoft.Extensions.Logging;

namespace BeerDispenser.Application.Implementation.Messaging.Consumers
{
    public class PaymentCompletedConsumer : EventConsumerBase<PaymentCompletedEvent>
	{
		public PaymentCompletedConsumer(
			ILogger<PaymentCompletedConsumer> logger,
			EventHubConfig configuration)
			: base(logger, configuration)
		{
		}
    }
}

