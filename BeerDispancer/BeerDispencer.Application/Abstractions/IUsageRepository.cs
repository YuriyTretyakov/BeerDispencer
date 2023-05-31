using System;
using Beerdispancer.Domain.Entities;

namespace BeerDispancer.Application.Abstractions
{
    public interface IUsageRepository
    {
        Task AddSUsageAsync(UsageDto usage);
        IEnumerable<UsageDto> GetUsagesByDispencerId(Guid id);
        UsageDto GetActiveUsageByDispencerId(Guid id);
    }
}

