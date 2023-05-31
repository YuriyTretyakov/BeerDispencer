using System;
using Beerdispancer.Domain.Entities;

namespace BeerDispancer.Application.Abstractions
{
	public interface IDispencerUof: IDisposable
	{
		 IDispencerRepository DispencerRepo { get; set; }
		 IUsageRepository UsageRepo { get; set; }
         Task Complete();
		 Task<bool> UpdateDispencerStateAsync(IDispencerUpdate dispencerUpdate, IBeerFlowSettings beerFlowSettings);
		

    }
}

