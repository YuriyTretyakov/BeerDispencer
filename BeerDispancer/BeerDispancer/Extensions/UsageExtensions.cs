using System;
using BeerDispancer.DataLayer.Models;
using BeerDispancer.DomainLayer;

namespace BeerDispancer.Extensions
{
	public static class UsageExtensions
	{
		public static Usage EnrichDbModel(this Usage initialModel, UsageDto dto)
		{
			dto.Validate();

			initialModel.OpenAt = dto.OpenedAt?? initialModel.OpenAt;
            initialModel.ClosedAt = dto.ClosedAt??initialModel.ClosedAt;
            initialModel.FlowVolume = dto.GetFlowVolume();
            initialModel.TotalSpent = dto.GetTotalSpent();
			initialModel.DispencerId = dto.DispencerId;
            return initialModel;

        }
	}
}

