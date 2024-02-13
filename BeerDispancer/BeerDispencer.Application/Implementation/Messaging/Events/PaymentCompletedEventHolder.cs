using BeerDispenser.Messaging.Core;

namespace BeerDispenser.Application.Implementation.Messaging.Events
{
    public class PaymentCompletedEventHolder : EventHolder<PaymentCompletedEvent>
	{
		public PaymentCompletedEventHolder(PaymentCompletedEvent @event, int retryCount, Guid correlationId)
			: base(@event, retryCount, correlationId)
		{
		}

		public PaymentCompletedEventHolder(EventHolder<PaymentInProccessEvent> toProcessEventHolder)
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

