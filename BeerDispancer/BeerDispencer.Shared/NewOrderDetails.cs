namespace BeerDispencer.Shared
{
    public class NewOrderDetails
	{
        public Guid DispencerId { get; set; }
        public decimal Amount { get; set; }
        public string Currency { get; set; }
    }
}

