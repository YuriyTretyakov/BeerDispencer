using System;
using System.Net.NetworkInformation;
using Beerdispancer.Domain.Entities;
using BeerDispancer.Application.Abstractions;

namespace BeerDispencer.Infrastructure.Persistence
{
	public class DispencerRepository: IDispencerRepository
    {
        private IBeerDispancerDbContext _dbcontext;


        public DispencerRepository(IBeerDispancerDbContext dbcontext)
        {
            _dbcontext = dbcontext;
        }


        public async Task<DispencerDto> CreateAsync(DispencerDto dispencer)
        {
            await _dbcontext.Dispencers.AddAsync(dispencer);
            return dispencer;
        }

        public bool UpdateDispencerStatus(Guid id, DispencerStatusDto status)
        {
            var dispencer = _dbcontext.Dispencers.SingleOrDefault(x => x.Id.Equals(id));

            if (dispencer == null || status == dispencer.Status)
            {
                return false;
            }

            dispencer.Status = status;

            return true;
        }
    }
}

