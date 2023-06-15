using System;
using System.Threading;
using System.Transactions;

using BeerDispancer.Application.Abstractions;
using BeerDispancer.Application.DTO;
using BeerDispencer.Application.Abstractions;
using BeerDispencer.Application.DTO;
using BeerDispencer.Infrastructure.Persistence.Abstractions;
using Microsoft.EntityFrameworkCore;

namespace BeerDispencer.Infrastructure.Persistence
{
	public class BeerDispancerUof: IDispencerUof
    {
        private readonly IBeerDispancerDbContext _dbcontext;
        private CancellationToken _cancellationToken = new CancellationToken();

        public BeerDispancerUof(IBeerDispancerDbContext dbcontext)
		{
            
            DispencerRepo = new DispencerRepository(dbcontext);
            UsageRepo = new UsageRepository(dbcontext);
            _dbcontext = dbcontext;
        }

       
        public IDispencerRepository DispencerRepo { get; set; }
        public IUsageRepository UsageRepo { get ; set; }

        public async Task Complete()
        {
            await _dbcontext.SaveChangesAsync(_cancellationToken); 
        }

        public void Dispose()
        {
            _dbcontext.Dispose();
        }
    }
}

