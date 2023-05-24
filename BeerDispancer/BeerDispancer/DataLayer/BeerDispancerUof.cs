using System;
using System.Transactions;
using BeerDispancer.DataLayer.Abstractions;
using BeerDispancer.DataLayer.Models;
using BeerDispancer.DomainLayer;
using BeerDispancer.Extensions;
using BeerDispancer.PresentationLayer;
using BeerDispencer.DomainLayer;

namespace BeerDispancer.DataLayer
{
	public class BeerDispancerUof: IDispencerUof
    {
        private readonly BeerDispancerDbContext _dbcontext;

        public BeerDispancerUof(BeerDispancerDbContext dbcontext)
		{
            Dispencer = new DispencerRepository(dbcontext);
            Usage = new UsageRepository(dbcontext);
            _dbcontext = dbcontext;
        }

        public IDispencerRepository Dispencer { get; set; }
        public IUsageRepository Usage { get; set; }

        public async Task Complete()
        {
            await _dbcontext.SaveChangesAsync();
        }

        public void Dispose()
        {
            _dbcontext.Dispose();
        }


        public async Task<bool> UpdateDispencerStateAsync(UsageDto usageDto, DispencerStatusDto status)
        {
            using (TransactionScope scope = new TransactionScope(
                TransactionScopeOption.RequiresNew,
                new TransactionOptions
                {
                    IsolationLevel = IsolationLevel.ReadCommitted
                },
                TransactionScopeAsyncFlowOption.Enabled))
            {

                var result = Dispencer.UpdateDispencerStatus(usageDto.DispencerId, status);

                if (result == false)
                {
                    return false;
                }

                if (status == DispencerStatusDto.Open)
                {
                    var dbModel = new Models.Usage().EnrichDbModel(usageDto);
                    await Usage.AddStartUsage(dbModel);
                }

                else if (status == DispencerStatusDto.Close)
                {
                    var usageFound = Usage.GetUsageByDispencerId(usageDto.DispencerId);
                    usageDto.OpenedAt = usageFound.OpenAt;
                    usageFound.EnrichDbModel(usageDto);
                }

                await Complete();
                scope.Complete();
            }
            return true;
        }

        public IEnumerable<UsagesToDispencerStateDto> GetSpendings(Guid id)
        {
            return (from d in _dbcontext.Dispencers
                                  join u in _dbcontext.Usage
                          on d.Id equals u.DispencerId
                                  where d.Id == id
                                  select new UsagesToDispencerStateDto
                                  {
                                      OpenedAt = u.OpenAt,
                                      DispencerId = d.Id,
                                      ClosedAt = u.ClosedAt,
                                      FlowVolume = u.FlowVolume,
                                      TotalSpent = u.TotalSpent,
                                      DispencerState = d.Status
                                  }).ToList();

                             
        }
    }
}

