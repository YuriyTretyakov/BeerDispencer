using System;

namespace Beerdispancer.Domain.Entities
{
	public class UsageDto
	{
        public int Id { get; set; }
        public Guid DispencerId { get; set; }
        public DateTime? OpenAt { get; set; }
        public DateTime? ClosedAt { get; set; }
        public double? FlowVolume { get; set; }
        public double? TotalSpent { get; set; }
    }
}

