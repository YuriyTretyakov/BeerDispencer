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
	public class BeerDispencerUof: IDispencerUof
    {
        private readonly IBeerDispencerDbContext _dbcontext;
        private CancellationToken _cancellationToken = new CancellationToken();

        public BeerDispencerUof(IBeerDispencerDbContext dbcontext)
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

        private TransactionScope _transaction;

        public void StartTransaction()
        {
            _transaction = new TransactionScope(TransactionScopeOption.Required,
           new TransactionOptions { IsolationLevel = IsolationLevel.ReadCommitted });
        }

        public void CommitTransaction()
        {
            _transaction.Complete();
        }

        public void Dispose()
        {
            _transaction.Dispose();
            _dbcontext.Dispose();
        }
    }
}

