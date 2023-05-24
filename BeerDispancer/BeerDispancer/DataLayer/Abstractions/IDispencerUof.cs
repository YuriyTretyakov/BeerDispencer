using System;
using BeerDispancer.DomainLayer;
using BeerDispencer.DomainLayer;

namespace BeerDispancer.DataLayer.Abstractions
{
	public interface IDispencerUof: IDisposable
	{
		 IDispencerRepository Dispencer { get; set; }
		 IUsageRepository Usage { get; set; }
         Task Complete();
		 Task<bool> UpdateDispencerStateAsync(UsageDto usageDto, DispencerStatusDto status);
		 IEnumerable<UsagesToDispencerStateDto> GetSpendings(Guid id);

    }
}

