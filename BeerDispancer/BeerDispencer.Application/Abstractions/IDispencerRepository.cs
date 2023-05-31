using System;
using Beerdispancer.Domain.Entities;

namespace BeerDispancer.Application.Abstractions
{
	public interface IDispencerRepository
	{
		public Task<DispencerDto> CreateAsync(DispencerDto dispencer);
		public bool UpdateDispencerStatus(Guid id, DispencerStatusDto status);
	}
}

