using BeerDispenser.Application.Implementation.Messaging.Events;
using BeerDispenser.Kafka.Core;

namespace BeerDispenser.Application.Implementation.Messaging.Publishers
{
    public class PaymentToProcessPublisher : EventPublisherBase<PaymentToProcessEvent>
	{
		public PaymentToProcessPublisher(KafkaConfig configuration)
			:base(configuration, nameof(PaymentToProcessEvent))
		{
		}
	}
}

