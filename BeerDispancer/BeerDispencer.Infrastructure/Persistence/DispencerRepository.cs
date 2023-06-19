using System;
using System.Net.NetworkInformation;
using BeerDispancer.Application.DTO;
using BeerDispencer.Application.Abstractions;
using BeerDispencer.Infrastructure.Extensions;
using BeerDispencer.Infrastructure.Persistence.Abstractions;
using BeerDispencer.Infrastructure.Persistence.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;

namespace BeerDispencer.Infrastructure.Persistence
{
	public class DispencerRepository: IDispencerRepository
    {
        private IBeerDispencerDbContext _dbcontext;


        public DispencerRepository(IBeerDispencerDbContext dbcontext)
        {
            _dbcontext = dbcontext;
        }

        public async Task<DispencerDto>AddAsync(DispencerDto dispencerDto)
        {
            var entity = dispencerDto.ToDbEntity();
            await _dbcontext.Dispencers.AddAsync(entity);
            return entity.ToDto();
        }

        public Task DeleteAsync(Guid id)
        { 
             _dbcontext.Dispencers.RemoveRange(new Dispencer { Id =id});
            return Task.CompletedTask;
        }

        public async Task<IEnumerable<DispencerDto>> GetAllAsync()
        {
            var dbResult = await _dbcontext.Dispencers.ToListAsync();

            return dbResult.Cast<DispencerDto>();
        }

        public async Task<DispencerDto> GetByIdAsync(Guid id)
        {
            var entity = await _dbcontext.Dispencers.SingleOrDefaultAsync(x => x.Id == id);
            return entity==null?null:entity.ToDto(); 
        }

        public async Task UpdateAsync(DispencerDto dispencerDto)
        {
            var dispencerEntity = await _dbcontext.Dispencers.SingleOrDefaultAsync(x => x.Id == dispencerDto.Id);

            dispencerEntity.Status = DispencerExtensions.ToDbEntity(dispencerDto.Status) ?? dispencerEntity.Status;
            dispencerEntity.Volume = dispencerDto.Volume ?? dispencerEntity.Volume;
        }

        
    }
}

