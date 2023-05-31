using System;
using Beerdispancer.Domain.Entities;
using BeerDispancer.Application.Abstractions;


namespace BeerDispencer.Infrastructure.Persistence.Models
{
    public class UsageRepository : IUsageRepository
    {
        private readonly IBeerDispancerDbContext _dbcontext;

        public UsageRepository(IBeerDispancerDbContext dbcontext)
        {
            _dbcontext = dbcontext;
        }

        public async Task AddSUsageAsync(UsageDto usage)
        {
            await _dbcontext
                .Usage
                .AddAsync(usage);
        }

        public IEnumerable<UsageDto> GetUsagesByDispencerId(Guid id)
        {
            return _dbcontext
                .Usage.Where(x => x.DispencerId == id);
        }

        public UsageDto GetActiveUsageByDispencerId(Guid id)
        {
            return _dbcontext
                .Usage.SingleOrDefault(x => x.DispencerId == id && x.ClosedAt==null && x.OpenAt!=null);
        }

    }
}

