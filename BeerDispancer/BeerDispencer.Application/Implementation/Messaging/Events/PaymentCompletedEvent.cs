using BeerDispenser.Shared.Dto.Payments;

namespace BeerDispenser.Application.Implementation.Messaging.Events
{
    public class PaymentCompletedEvent
    {
        public PaymentInProccessEvent OriginalEvent { set;  get; }
        public PaymentStatusDto Status { get;  set; }
        public string Reason { get;  set; }

        public PaymentCompletedEvent(PaymentInProccessEvent originalEvent, PaymentStatusDto status, string reason = null)
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