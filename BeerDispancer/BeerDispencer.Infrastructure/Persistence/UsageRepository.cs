using System;
using BeerDispancer.Application.DTO;
using BeerDispencer.Application.Abstractions;
using BeerDispencer.Infrastructure.Extensions;
using BeerDispencer.Infrastructure.Persistence.Abstractions;
using BeerDispencer.Infrastructure.Persistence.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;

namespace BeerDispencer.Infrastructure.Persistence
{
    public class UsageRepository : IUsageRepository
    {
        private readonly IBeerDispancerDbContext _dbcontext;

        public UsageRepository(IBeerDispancerDbContext dbcontext)
        {
            _dbcontext = dbcontext;
        }


        public async Task<UsageDto> AddAsync(UsageDto dto)
        {
            var entity = dto.ToDbEntity();
             await _dbcontext
                .Usage
                .AddAsync(entity);
            return entity.ToDto();
        }

        public async Task<IEnumerable<UsageDto>> GetAllAsync()
        {
            var entityList = await _dbcontext.Usage.ToListAsync();

            return entityList.Cast<UsageDto>();
        }

        public async Task<UsageDto> GetByIdAsync(int id)
        {
            var entity = await _dbcontext
                        .Usage
                       .SingleOrDefaultAsync(x => x.Id == id);

            return entity==null?null:entity.ToDto();
               
        }

       
        public Task UpdateAsync(UsageDto dto)
        {
            var entity = _dbcontext.Usage.SingleOrDefault(x => x.Id.Equals(dto.Id));

            if (entity != null)
            {
                entity.ClosedAt = dto.ClosedAt ?? entity.ClosedAt;
                entity.FlowVolume = dto.FlowVolume ?? entity.FlowVolume;
                entity.TotalSpent = dto.TotalSpent ?? entity.TotalSpent;
            }
            return Task.CompletedTask;
        }

        public Task DeleteAsync(int id)
        {
            return Task.FromResult(_dbcontext
               .Usage.Remove(new Usage { Id = id }));
        }

        public async Task<UsageDto[]> GetByDispencerIdAsync(Guid dispencerId)
        {
            var entitiesList = await _dbcontext
               .Usage.Where(x => x.DispencerId == dispencerId).ToListAsync();
            return entitiesList
                .Select(x =>
                new UsageDto
                {
                    ClosedAt = x.ClosedAt,
                    Id=x.Id,
                    OpenAt = x.OpenAt,
                    FlowVolume = x.FlowVolume,
                    TotalSpent=x.TotalSpent,
                    DispencerId =x.DispencerId
                }).ToArray() ;
        }
    }
}

