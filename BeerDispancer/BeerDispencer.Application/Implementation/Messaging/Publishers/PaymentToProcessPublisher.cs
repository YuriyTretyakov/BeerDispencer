using BeerDispenser.Application.Implementation.Messaging.Events;
using BeerDispenser.Kafka.Core;
using Microsoft.Extensions.Logging;

namespace BeerDispenser.Application.Implementation.Messaging.Publishers
{
    public class PaymentToProcessPublisher : EventPublisherBase<PaymentToProcessEvent>
	{
		public PaymentToProcessPublisher(ILogger<PaymentToProcessPublisher> logger, KafkaConfig configuration)
			:base(configuration, nameof(PaymentToProcessEvent), logger)
		{
		}
	}
}

