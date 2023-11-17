using BeerDispenser.Application.Implementation.Messaging.Events;
using BeerDispenser.Kafka.Core;
using Microsoft.Extensions.Logging;

namespace BeerDispenser.Application.Implementation.Messaging.Publishers
{
    public class PaymentCompletedPublisher:EventPublisherBase<PaymentCompletedEvent>
	{
		public PaymentCompletedPublisher(ILogger<PaymentCompletedPublisher> logger, KafkaConfig configuration)
			:base(configuration, nameof(PaymentCompletedEvent), logger)
		{
		}
	}
}

