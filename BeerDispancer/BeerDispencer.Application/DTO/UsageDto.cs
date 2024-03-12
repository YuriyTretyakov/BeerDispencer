using BeerDispenser.Shared.Dto.Payments;

namespace BeerDispenser.Application.DTO
{
    public class UsageDto
	{
        public Guid Id { get; set; }
        public Guid DispencerId { get; set; }
        public DateTimeOffset OpenAt { get; set; }
        public DateTimeOffset? ClosedAt { get; set; }
        public decimal? FlowVolume { get; set; }
        public decimal? TotalSpent { get; set; }
        public Guid? PaidBy { get; set; }
        public PaymentStatusDto? PaymentStatus { get; set; }
        public string Reason { get; set; }
    }
}

