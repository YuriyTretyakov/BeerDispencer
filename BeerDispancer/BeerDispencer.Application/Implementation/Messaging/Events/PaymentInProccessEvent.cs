namespace BeerDispenser.Application.Implementation.Messaging.Events
{
    public class PaymentInProccessEvent
    {
        public string CustomerId { get; set; }
        public string CardId { get; set; }
        public long Amount { get; set; }
        public string Currency { get; set; }
        public string PaymentDescription { get; set; }
        public Guid DIspenserId { get; set; }
        public Guid PaymentInitiatedBy { get; set; }
    }
}