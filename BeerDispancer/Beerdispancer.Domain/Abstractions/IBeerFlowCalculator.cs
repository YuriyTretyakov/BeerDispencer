﻿using System;
namespace Beerdispancer.Domain.Abstractions
{
	public interface IBeerFlowCalculator
	{
        public double? GetFlowVolume(DateTime? closedAt, DateTime? openAt, double litersPerSec);
        public double? GetTotalSpent(double? volume, double pricePerLiter);     
    }
}
