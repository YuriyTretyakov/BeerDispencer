namespace BeerDispenser.Shared
{
    public class OrderResponseDetails
	{
		public Guid ProductId { get; set; }
		public bool IsSuccess { get; set; }
		public string Details { get; set; }
		public decimal AmountTotal { get; set; }
		public string Email { get; set; }
    }
}

