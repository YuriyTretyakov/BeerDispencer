namespace BeerDispenser.Shared
{
    public class NewOrderDetails
	{
        public Guid DispenserId { get; set; }
        public decimal Amount { get; set; }
        public string Currency { get; set; }
    }
}

