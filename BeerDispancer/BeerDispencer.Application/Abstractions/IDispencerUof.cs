using System;
using BeerDispancer.Application.DTO;
using BeerDispencer.Application.Abstractions;
using BeerDispencer.Application.DTO;


namespace BeerDispancer.Application.Abstractions
{
	public interface IDispencerUof : IDisposable
	{
        IDispencerRepository DispencerRepo { get; set; }
		IUsageRepository UsageRepo { get; set; }
		Task Complete();
		//Task<bool> UpdateDispencerStateAsync(DispencerUpdateDto dispencerUpdate);
	}
}

