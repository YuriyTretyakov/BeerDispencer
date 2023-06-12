using System;
using BeerDispancer.Application.DTO;
using BeerDispencer.Application.Abstractions;

namespace BeerDispencer.Application.Abstractions
{
    public interface IUsageRepository:IRepository<UsageDto,int>
    {
        Task<UsageDto[]> GetByDispencerIdAsync(Guid dispencerId);
    }
}

