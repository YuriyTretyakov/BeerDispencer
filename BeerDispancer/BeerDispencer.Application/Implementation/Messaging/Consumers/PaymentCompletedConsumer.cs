using BeerDispenser.Application.Implementation.Messaging.Events;
using BeerDispenser.Kafka.Core;

namespace BeerDispenser.Application.Implementation.Messaging.Consumers
{
    public class PaymentCompletedConsumer : EventConsumerBase<PaymentCompletedEvent>
	{
		public PaymentCompletedConsumer(KafkaConfig configuration) : base(configuration)
		{
		}

        public override string ConfigSectionName => nameof(PaymentCompletedEvent);
    }
}

