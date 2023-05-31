using System;
using System.Threading;
using System.Transactions;
using Beerdispancer.Domain.Entities;
using BeerDispancer.Application.Abstractions;
using BeerDispencer.Application.Abstractions;
using Microsoft.EntityFrameworkCore;

namespace BeerDispencer.Infrastructure.Persistence.Models
{
	public class BeerDispancerUof: IDispencerUof
    {
        private readonly IBeerDispancerDbContext _dbcontext;
        private readonly IBeerFlowCalculator _calculator;
        private CancellationToken _cancellationToken = new CancellationToken();

        public BeerDispancerUof(IBeerDispancerDbContext dbcontext, IBeerFlowCalculator calculator)
		{
            DispencerRepo = new DispencerRepository(dbcontext);
            UsageRepo = new UsageRepository(dbcontext);
            _dbcontext = dbcontext;
            _calculator = calculator;
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


        public async Task<bool> UpdateDispencerStateAsync(
            IDispencerUpdate dispencerUpdate,
            IBeerFlowSettings beerFlowSettings)
        {
            using (TransactionScope scope = new TransactionScope(
                TransactionScopeOption.RequiresNew,
                new TransactionOptions
                {
                    IsolationLevel = IsolationLevel.ReadCommitted
                },
                TransactionScopeAsyncFlowOption.Enabled))
            {

                var result = DispencerRepo.UpdateDispencerStatus(dispencerUpdate.Id, dispencerUpdate.Status);

                if (result == false)
                {
                    return false;
                }

                if (dispencerUpdate.Status == DispencerStatusDto.Open)
                {
                    await UsageRepo.AddSUsageAsync(new UsageDto { DispencerId = dispencerUpdate.Id, OpenAt = dispencerUpdate.UpdatedAt } );
                }

                else if (dispencerUpdate.Status == DispencerStatusDto.Close)
                {
                    var usageFound = UsageRepo.GetActiveUsageByDispencerId(dispencerUpdate.Id);
                    usageFound.ClosedAt = dispencerUpdate.UpdatedAt;
                    usageFound.FlowVolume = _calculator.GetFlowVolume(usageFound.ClosedAt, usageFound.OpenAt, beerFlowSettings.LitersPerSecond);
                    usageFound.TotalSpent = _calculator.GetTotalSpent(usageFound.FlowVolume, beerFlowSettings.PricePerLiter);
                }

                await Complete();
                scope.Complete();
            }
            return true;
        }

        public IEnumerable<UsageDto> GetSpendings(Guid id)
        {
            return UsageRepo.GetUsagesByDispencerId(id);       
        }   
    }
}

