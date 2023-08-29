using System;
using BeerDispancer.Application.DTO;
using BeerDispencer.Application.Abstractions;


namespace BeerDispencer.Application.Abstractions
{ 
    public interface IDispencerRepository: IRepository<DispencerDto>
	{
	}
}

