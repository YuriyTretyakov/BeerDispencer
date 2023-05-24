using System;
using BeerDispancer.DataLayer.Abstractions;
using BeerDispancer.DataLayer.Models;
using BeerDispancer.DomainLayer;
using BeerDispancer.Extensions;
using BeerDispancer.PresentationLayer;
using BeerDispencer.DomainLayer;

namespace BeerDispancer.ApplicationLayer
{
	public class DispencerManager
	{
        private readonly IDispencerUof _dispencerUof;

        public DispencerManager(IDispencerUof dispencerUof)
		{
            _dispencerUof = dispencerUof;
        }

        private const double Liters_Per_Second = 0.064;
        private const double Price_Per_Liter = 12.25;
        //To do so, we will use a reference value of 12.25€/l.
        //So, if the dispenser has configured the flow volume ratio as 0.064 litres/second and the tap was open for


        public async Task<DispencerDto> CreateDispencerAsync(DispencerDto dispencer)
        {
            var dbDispencer = dispencer.ToDbModel();

            var result =  await _dispencerUof.Dispencer.CreateAsync(dbDispencer);
            await _dispencerUof.Complete();
            dispencer.Id = result.Id;
            return dispencer;
        }


		public async Task<bool> ChangeDispancerStateAsync(DispencerUpdateDto dispencerUpdate)
		{

            var usageDto = new UsageDto
            {
                DispencerId = dispencerUpdate.Id,
                LitersPerSecond = Liters_Per_Second,
                PricePerLiter = Price_Per_Liter,
            };

            if (dispencerUpdate.Status == DispencerStatusDto.Open)
            {
                usageDto.OpenedAt = dispencerUpdate.UpdatedAt;
            }

            else if (dispencerUpdate.Status == DispencerStatusDto.Close)
            {
                usageDto.ClosedAt = dispencerUpdate.UpdatedAt;
            }

            return await _dispencerUof.UpdateDispencerStateAsync(usageDto,dispencerUpdate.Status);
        }

        
  		public BeerDispancer.PresentationLayer.Usage GetSpending(Guid id)
		{
            var spendingsDto = _dispencerUof.GetSpendings(id);
            double? totalAmount = 0;
            var usageEntries = spendingsDto.Select(x =>
            {
                var entry = new UsageEntry
                {
                    OpenedAt = x.OpenedAt,
                    ClosedAt =x.ClosedAt,
                };

                entry.FlowVolume = x.FlowVolume ?? Calculator.GetFlowVolume(DateTime.UtcNow, x.OpenedAt, Liters_Per_Second);
                entry.TotalSpent = x.TotalSpent??Calculator.GetTotalSpent(entry.FlowVolume, Price_Per_Liter);
                totalAmount += entry.TotalSpent;
                return entry;
            }).ToArray();

            return new BeerDispancer.PresentationLayer.Usage
            {
                Amount = totalAmount.HasValue ? totalAmount.Value : 0,
                Usages = usageEntries
            };
        }
	}
}

