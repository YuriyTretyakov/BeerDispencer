namespace BeerDispencer.Shared
{
    public class OrderResponseDetails
	{
		public bool IsSuccess { get; set; }
		public string Details { get; set; }
		public decimal AmountTotal { get; set; }
		public string Email { get; set; }
    }
}

