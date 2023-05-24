using System;
using BeerDispancer.DataLayer.Models;
using BeerDispancer.DomainLayer;

namespace BeerDispancer.DataLayer.Abstractions
{
    public interface IUsageRepository
    {
        Task AddStartUsage(Usage usage);
        Usage GetUsageByDispencerId(Guid id);
    }
}

