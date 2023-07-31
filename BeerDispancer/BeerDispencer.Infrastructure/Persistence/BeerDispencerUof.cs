using System;
using System.Text.Json.Serialization;
using System.Threading;
using System.Transactions;

using BeerDispancer.Application.Abstractions;
using BeerDispancer.Application.DTO;
using BeerDispencer.Application.Abstractions;
using BeerDispencer.Application.DTO;
using BeerDispencer.Infrastructure.Persistence.Abstractions;
using BeerDispencer.Infrastructure.Persistence.Entities;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;

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

        public async Task ProcessPaymentAsync(Guid dispencerId, double amount)
        {

            using var transaction = StartTransaction();

            var payment = new Payments
            {
                DispencerId = dispencerId,
                Amount = amount,
                Status = PaymentStatus.Completed,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            var outbox = new Outbox
            {
                Data = JsonConvert.SerializeObject(payment),
                MessageType =nameof(Payments),
                CreatedAt = DateTime.UtcNow,
                Status = OperationStatus.Created
            };

            await _dbcontext.Payments.AddAsync(payment);

            await _dbcontext.Outbox.AddAsync(outbox);
            await _dbcontext.SaveChangesAsync(_cancellationToken);
            CommitTransaction();
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

