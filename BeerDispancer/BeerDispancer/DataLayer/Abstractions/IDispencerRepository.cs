using System;
using BeerDispancer.DataLayer.Models;
using BeerDispancer.DomainLayer;

namespace BeerDispancer.DataLayer.Abstractions
{
	public interface IDispencerRepository
	{
		public Task<Dispencer> CreateAsync(Dispencer dispencer);
		public bool UpdateDispencerStatus(Guid id, DispencerStatusDto status);
	}
}

