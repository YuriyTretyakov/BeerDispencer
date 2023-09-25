using System;
using BeerDispenser.Application.DTO;
using BeerDispenser.Application.Abstractions;

namespace BeerDispenser.Application.Abstractions
{
    public interface IUsageRepository:IRepository<UsageDto>
    {
        Task<UsageDto[]> GetByDispencerIdAsync(Guid dispencerId);
    }
}

