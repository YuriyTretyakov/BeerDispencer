namespace BeerDispenser.Shared
{
    public class PaymentCardViewModel
    {
        public Guid Id { get; set; }
        public bool IsDefault { get; set; }
        public string City { get; set; }
        public string AdressCountry { get; set; }
        public string Line1 { get; set; }
        public string State { get; set; }
        public string Zip { get; set; }
        public string Brand { get; set; }
        public string Country { get; set; }
        public string CvcCheck { get; set; }
        public string ExpMonth { get; set; }
        public string ExpYear { get; set; }
        public string Last4 { get; set; }
        public string AccountHolderName { get; set; }
        public int Created { get; set; }
        public string Email { get; set; }
    }
}

