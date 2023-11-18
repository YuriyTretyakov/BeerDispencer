using System;
using BeerDispenser.Kafka.Core;

namespace BeerDispenser.Application.Implementation.Messaging.Events
{
	public class PaymentCompletedEventHolder : EventHolder<PaymentCompletedEvent>
	{
		public PaymentCompletedEventHolder(PaymentCompletedEvent @event, int retryCount, Guid correlationId)
			: base(@event, retryCount, correlationId)
		{
		}

		public PaymentCompletedEventHolder(EventHolder<PaymentToProcessEvent> toProcessEventHolder)
			: base(
				 new PaymentCompletedEvent
				 {
					 OriginalEvent = toProcessEventHolder.Event
				 },
				 toProcessEventHolder.RetryCount,
				 toProcessEventHolder.CorrelationId)
		{ }
	}
}

