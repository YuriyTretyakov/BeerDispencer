using BeerDispenser.Application.DTO;
using BeerDispenser.Application.Abstractions;
using BeerDispenser.Infrastructure.Extensions;
using BeerDispenser.Infrastructure.Persistence.Abstractions;
using Microsoft.EntityFrameworkCore;
using BeerDispencer.Infrastructure.Persistence.Entities;
using BeerDispencer.Infrastructure.Extensions;

namespace BeerDispenser.Infrastructure.Persistence
{
    public class OutBoxRepository: IOutboxRepository
    {
        private IBeerDispencerDbContext _dbcontext;


        public OutBoxRepository(IBeerDispencerDbContext dbcontext)
        {
            _dbcontext = dbcontext;
        }

        public async Task<OutboxDto>AddAsync(OutboxDto outbox)
        {
            var entity = outbox.ToDb();
            await _dbcontext.Outbox.AddAsync(entity);
            return entity.ToDto();
        }

        public Task DeleteAsync(Guid id)
        { 
             _dbcontext.Outbox.RemoveRange(new Outbox { Id =id});
            return Task.CompletedTask;
        }

        public async Task<IEnumerable<OutboxDto>> GetAllAsync()
        {
            var dbResult = await _dbcontext.Outbox.ToListAsync();

            return dbResult.Select(x => x.ToDto());
        }

        public async Task<OutboxDto> GetByIdAsync(Guid id)
        {
            var entity = await _dbcontext.Outbox.SingleOrDefaultAsync(x => x.Id == id);
            return entity==null?null:entity.ToDto(); 
        }

        public async Task<IEnumerable<OutboxDto>> GetNotProccessedEvents()
        {
            var notCompleted = await _dbcontext
                 .Outbox
                 .Where(x => x.EventState != EventStateDto.Completed)
                 .ToListAsync();

            return notCompleted.Select(x => x.ToDto());
        }

        public async Task UpdateAsync(OutboxDto outbox)
        {
            var dispencerEntity = await _dbcontext.Outbox.SingleOrDefaultAsync(x => x.Id == outbox.Id);

            dispencerEntity.EventState = outbox.EventState;
            dispencerEntity.UpdatedAt = outbox.UpdatedAt ?? outbox.UpdatedAt;
        }
    }
}

