using BeerDispenser.Application.Implementation.Messaging.Events;
using BeerDispenser.Kafka.Core;

namespace BeerDispenser.Application.Implementation.Messaging.Consumers
{
    public class PaymentToProcessConsumer : EventConsumerBase<PaymentToProcessEvent>
	{
		public PaymentToProcessConsumer(KafkaConfig configuration) : base(configuration)
		{
		}

        public override string ConfigSectionName => nameof(PaymentToProcessEvent);
    }
}

