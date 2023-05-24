using System;
using BeerDispencer.DomainLayer;
using Newtonsoft.Json;

namespace BeerDispancer.DomainLayer
{
	public class UsageDto
	{
        public Guid DispencerId { get; set; }
        public DateTime? OpenedAt { get; set; }
        public DateTime? ClosedAt { get; set; }
        public double LitersPerSecond { get; set; }
        public double PricePerLiter { get; set; }

        public double? GetFlowVolume()
        {
           return Calculator.GetFlowVolume(ClosedAt, OpenedAt, LitersPerSecond);
        }

        public double? GetTotalSpent()
        {
            return Calculator.GetTotalSpent(GetFlowVolume(), PricePerLiter);
        }

        public void Validate()
        {
            if (ClosedAt < OpenedAt)
                throw new ArgumentException($"{nameof(ClosedAt)} < {nameof(OpenedAt)}");

            if (LitersPerSecond==default(double))
                throw new ArgumentException($"{nameof(LitersPerSecond)} should be set");

            if (PricePerLiter == default(double))
                throw new ArgumentException($"{nameof(PricePerLiter)} should be set");
        }
    }
}

