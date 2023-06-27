using System.Transactions;

using BeerDispancer.Application.Abstractions;
using BeerDispencer.Application.Abstractions;
using BeerDispencer.Infrastructure.Persistence.Abstractions;
using MongoDB.Driver;

namespace BeerDispencer.Infrastructure.Persistence
{
    public class BeerDispencerUof : IDispencerUof
    {
        private readonly IBeerDispencerDbContext _dbcontext;

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

        private IClientSessionHandle _session;

        public void Dispose()
        {
            _session?.Dispose();
        }
    }
}

