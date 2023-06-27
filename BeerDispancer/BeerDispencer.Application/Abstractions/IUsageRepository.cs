using System;
using BeerDispancer.Application.DTO;
using BeerDispencer.Application.Abstractions;

namespace BeerDispencer.Application.Abstractions
{
    public interface IUsageRepository:IRepository<UsageDto>
    {
        Task<UsageDto[]> GetByDispencerIdAsync(string dispencerId);
    }
}

