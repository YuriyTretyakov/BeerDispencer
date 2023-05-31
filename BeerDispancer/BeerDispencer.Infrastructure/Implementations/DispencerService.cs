using System;
using Beerdispancer.Domain.Entities;
using Beerdispancer.Infrastructure.DTO;
using BeerDispancer.Application.Abstractions;

namespace BeerDispencer.Infrastructure.Implementations
{
	public class DispencerService
	{
        private readonly IDispencerUof _dispencerUof;
        private readonly IBeerFlowSettings _beerFlowSettings;

        public DispencerService(IDispencerUof dispencerUof, IBeerFlowSettings beerFlowSettings)
		{
            _dispencerUof = dispencerUof;
            _beerFlowSettings = beerFlowSettings;
        }

        private const double Liters_Per_Second = 0.064;
        private const double Price_Per_Liter = 12.25;
        


        public async Task<DispencerDto> CreateDispencerAsync(DispencerDto dispencer)
        {
            var result =  await _dispencerUof.DispencerRepo.CreateAsync(dispencer);
            await _dispencerUof.Complete();
            dispencer.Id = result.Id;
            return dispencer;
        }


		public async Task<bool> ChangeDispancerStateAsync(DispencerUpdateDto dispencerUpdate)
		{

            return await _dispencerUof.UpdateDispencerStateAsync(dispencerUpdate, _beerFlowSettings);
        }

        
  		public SpendingsDto GetSpending(Guid id)
		{
            var usagesFound = _dispencerUof.UsageRepo.GetUsagesByDispencerId(id);
            double total = 0;

            var spendings = usagesFound.Select(x =>
            {
                var entry = new UsageDto
                {
                    OpenAt = x.OpenAt,
                    ClosedAt = x.ClosedAt,
                };

                entry.FlowVolume = x.FlowVolume ?? Calculator.GetFlowVolume(DateTime.UtcNow, x.OpenAt, Liters_Per_Second);
                entry.TotalSpent = x.TotalSpent??Calculator.GetTotalSpent(entry.FlowVolume, Price_Per_Liter);
                total += entry.TotalSpent?? 0;
                return entry;
            }).ToArray();

            return new SpendingsDto { Usages = spendings, Amount = total };
        }
	}
}

