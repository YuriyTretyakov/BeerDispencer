using BeerDispenser.Application.Implementation.Messaging.Events;
using BeerDispenser.Kafka.Core;
using Microsoft.Extensions.Logging;

namespace BeerDispenser.Application.Implementation.Messaging.Consumers
{
    public class PaymentCompletedConsumer : EventConsumerBase<PaymentCompletedEvent>
	{
		public PaymentCompletedConsumer(
			ILogger<PaymentCompletedConsumer> logger,
			KafkaConfig configuration) : base(logger, configuration)
		{
		}

        public override string ConfigSectionName => nameof(PaymentCompletedEvent);
    }
}

