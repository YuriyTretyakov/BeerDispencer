using BeerDispancer.Application.DTO;
using BeerDispencer.Application.Abstractions;
using BeerDispencer.Infrastructure.Extensions;
using BeerDispencer.Infrastructure.Persistence.Abstractions;
using BeerDispencer.Infrastructure.Persistence.Entities;
using Microsoft.EntityFrameworkCore;

namespace BeerDispencer.Infrastructure.Persistence
{
    public class DispenserRepository: IDispencerRepository
    {
        private IBeerDispencerDbContext _dbcontext;


        public DispenserRepository(IBeerDispencerDbContext dbcontext)
        {
            _dbcontext = dbcontext;
        }

        public async Task<DispenserDto>AddAsync(DispenserDto dispencerDto)
        {
            var entity = dispencerDto.ToDbEntity();
            await _dbcontext.Dispencers.AddAsync(entity);
            return entity.ToDto();
        }

        public Task DeleteAsync(Guid id)
        { 
             _dbcontext.Dispencers.RemoveRange(new Dispenser { Id =id});
            return Task.CompletedTask;
        }

        public async Task<IEnumerable<DispenserDto>> GetAllAsync()
        {
            var dbResult = await _dbcontext.Dispencers.ToListAsync();

            return dbResult.Select(x => x.ToDto());
        }

        public async Task<DispenserDto> GetByIdAsync(Guid id)
        {
            var entity = await _dbcontext.Dispencers.SingleOrDefaultAsync(x => x.Id == id);
            return entity==null?null:entity.ToDto(); 
        }

        public async Task UpdateAsync(DispenserDto dispenserDto)
        {
            var dispencerEntity = await _dbcontext.Dispencers.SingleOrDefaultAsync(x => x.Id == dispenserDto.Id);

            dispencerEntity.Status = dispenserDto.Status ?? dispencerEntity.Status;
            dispencerEntity.Volume = dispenserDto.Volume ?? dispencerEntity.Volume;
        }

        
    }
}

