using BeerDispenser.Kafka.Core;

namespace BeerDispenser.Application.Implementation.Messaging.Events
{
    public class PaymentCompletedEvent
    {
        public PaymentToProcessEvent OriginalEvent { set;  get; }
        public PaymentStatus Status { get;  set; }
        public string Reason { get;  set; }

        public PaymentCompletedEvent(PaymentToProcessEvent originalEvent, PaymentStatus status, string reason = null)
        {
            Status = status;
            Reason = reason;
            OriginalEvent = originalEvent;
        }

        public PaymentCompletedEvent()
        {
        }
    }
}