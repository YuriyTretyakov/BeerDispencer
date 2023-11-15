using BeerDispenser.Application.Implementation.Messaging.Events;
using BeerDispenser.Kafka.Core;

namespace BeerDispenser.Application.Implementation.Messaging.Publishers
{
    public class PaymentCompletedPublisher:EventPublisherBase<PaymentCompletedEvent>
	{
		public PaymentCompletedPublisher(KafkaConfig configuration)
			:base(configuration, nameof(PaymentCompletedEvent))
		{
		}
	}
}

