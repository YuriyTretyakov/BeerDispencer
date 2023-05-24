using System;
using BeerDispancer.DataLayer.Abstractions;
using BeerDispancer.DataLayer.Models;
using BeerDispancer.DomainLayer;

namespace BeerDispancer.DataLayer
{
	public class UsageRepository: IUsageRepository
    {
        private readonly BeerDispancerDbContext _dbcontext;

        public UsageRepository(BeerDispancerDbContext dbcontext)
		{
            _dbcontext = dbcontext;
        }

        public async Task AddStartUsage(Usage usage)
        {
            await _dbcontext
                .Usage
                .AddAsync(usage);
        }

        public Usage GetUsageByDispencerId(Guid id)
        {
            return  _dbcontext
                .Usage.SingleOrDefault(x => x.DispencerId == id);
        }

            }
}

