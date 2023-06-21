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
    public class BeerDispencerUof : IDispencerUof
    {
        private readonly IBeerDispencerDbContext _dbcontext;
        private CancellationToken _cancellationToken = new CancellationToken();

        public BeerDispencerUof(
            IBeerDispencerDbContext dbcontext,
            IUsageRepository usageRepository,
            IDispencerRepository dispencerRepository)
        {

            DispencerRepo = dispencerRepository;
            UsageRepo = usageRepository;
            _dbcontext = dbcontext;
        }


        public IDispencerRepository DispencerRepo { get; set; }
        public IUsageRepository UsageRepo { get; set; }

        public async Task Complete()
        {
            await _dbcontext.SaveChangesAsync(_cancellationToken);
        }

        private TransactionScope _transaction;

        //public void StartTransaction()
        //{
        //    var transactionOptions = new TransactionOptions { IsolationLevel = IsolationLevel.ReadUncommitted };
        //    new TransactionScope(transactionOptions,TimeSpan.MaxValue)

        //    _transaction = new TransactionScope(
        //        new TransactionScopeOption { }
        //            IsolationLevel =  },TransactionScopeAsyncFlowOption.Enabled
        //   );
        //}

        public TransactionScope StartTransaction()
        {
            var transactionOptions = new TransactionOptions
            {
                IsolationLevel = IsolationLevel.ReadUncommitted,
                Timeout = TransactionManager.MaximumTimeout
            };
            _transaction = new TransactionScope(TransactionScopeOption.Required, transactionOptions, TransactionScopeAsyncFlowOption.Enabled);
            return _transaction;
        }

        public void CommitTransaction()
        {
            _transaction.Complete();
        }

        public void Dispose()
        {
            _transaction?.Dispose();
            _dbcontext?.Dispose();
        }
    }
}

