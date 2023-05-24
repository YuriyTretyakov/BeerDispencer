using System;
using System.Net.NetworkInformation;
using BeerDispancer.DataLayer.Abstractions;
using BeerDispancer.DataLayer.Models;
using BeerDispancer.DomainLayer;

namespace BeerDispancer.DataLayer
{
	public class DispencerRepository: IDispencerRepository
    {
        private BeerDispancerDbContext _dbcontext;

        

        public DispencerRepository(BeerDispancerDbContext dbcontext)
        {
            _dbcontext = dbcontext;
        }

        public async Task<Dispencer> CreateAsync(Dispencer dispencer)
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

