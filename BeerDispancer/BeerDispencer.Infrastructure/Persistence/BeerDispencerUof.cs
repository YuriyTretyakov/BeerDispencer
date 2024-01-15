using System.Transactions;

using BeerDispenser.Application.Abstractions;
using BeerDispenser.Infrastructure.Persistence.Abstractions;

namespace BeerDispenser.Infrastructure.Persistence
{
    public class BeerDispencerUof : IDispencerUof
    {
        private readonly IBeerDispencerDbContext _dbcontext;
        private CancellationToken _cancellationToken = new CancellationToken();

        public BeerDispencerUof(
            IBeerDispencerDbContext dbcontext,
            IUsageRepository usageRepository,
            IDispencerRepository dispencerRepository,
            IPaymentCardRepository paymentCardRepository,
            IOutboxRepository outboxRepository)
        {
            DispencerRepo = dispencerRepository;
            UsageRepo = usageRepository;
            PaymentCardRepository = paymentCardRepository;
            OutboxRepo = outboxRepository;
            _dbcontext = dbcontext;
        }

        public IDispencerRepository DispencerRepo { get; set; }
        public IUsageRepository UsageRepo { get; set; }
        public IPaymentCardRepository PaymentCardRepository { get ; set; }
        public IOutboxRepository OutboxRepo { get ; set; }

        public async Task Complete()
        {
            await _dbcontext.SaveChangesAsync(_cancellationToken);
        }

        private TransactionScope _transaction;

        public TransactionScope StartTransaction()
        {
            var transactionOptions = new TransactionOptions
            {
                IsolationLevel = IsolationLevel.ReadCommitted,
                Timeout = TimeSpan.FromSeconds(5)//TransactionManager.MaximumTimeout
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

