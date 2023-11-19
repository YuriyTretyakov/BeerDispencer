using BeerDispenser.Kafka.Core;
using BeerDispenser.Shared;

namespace BeerDispenser.Application.Implementation.Messaging.Events
{
    public class PaymentCompletedEvent
    {
        public PaymentToProcessEvent OriginalEvent { set;  get; }
        public PaymentStatusDto Status { get;  set; }
        public string Reason { get;  set; }

        public PaymentCompletedEvent(PaymentToProcessEvent originalEvent, PaymentStatusDto status, string reason = null)
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