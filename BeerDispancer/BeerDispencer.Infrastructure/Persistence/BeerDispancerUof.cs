using System;
using System.Threading;
using System.Transactions;
using Beerdispancer.Domain.Abstractions;
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


        //public async Task<bool> UpdateDispencerStateAsync(
        //    DispencerUpdateDto dispencerUpdate)
        //{
        //    using (TransactionScope scope = new TransactionScope(
        //        TransactionScopeOption.RequiresNew,
        //        new TransactionOptions
        //        {
        //            IsolationLevel = IsolationLevel.ReadUncommitted
        //        },
        //        TransactionScopeAsyncFlowOption.Enabled))
        //    {

        //        var result = DispencerRepo.UpdateAsync(dispencerUpdate);

        //        if (result == false)
        //        {
        //            return false;
        //        }

        //        if (dispencerUpdate.Status == DispencerStatusDto.Open)
        //        {
        //            await UsageRepo.AddAsync(new UsageDto { DispencerId = dispencerUpdate.Id, OpenAt = dispencerUpdate.UpdatedAt } );
        //        }

        //        else if (dispencerUpdate.Status == DispencerStatusDto.Close)
        //        {
        //            var usagesFound = await UsageRepo.GetByDispencerIdAsync(dispencerUpdate.Id);

        //            var activeUsage = usagesFound.SingleOrDefault(x => x.ClosedAt == null);

        //            activeUsage.ClosedAt = dispencerUpdate.UpdatedAt;
        //            activeUsage.FlowVolume = _calculator.GetFlowVolume(activeUsage.ClosedAt, activeUsage.OpenAt);
        //            activeUsage.TotalSpent = _calculator.GetTotalSpent(activeUsage.FlowVolume);
        //        }

        //        await Complete();
        //        scope.Complete();
        //    }
        //    return true;
        //}
    }
}

