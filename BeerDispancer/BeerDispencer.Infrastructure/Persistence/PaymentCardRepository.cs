using BeerDispenser.Application.Abstractions;
using BeerDispenser.Infrastructure.Extensions;
using BeerDispenser.Infrastructure.Persistence.Abstractions;
using Microsoft.EntityFrameworkCore;
using System.Collections.ObjectModel;
using BeerDispencer.Infrastructure.Extensions;
using BeerDispenser.Application.DTO;

namespace BeerDispenser.Infrastructure.Persistence
{
    public class PaymentCardRepository : IPaymentCardRepository
    {
        private IBeerDispencerDbContext _dbcontext;


        public PaymentCardRepository(IBeerDispencerDbContext dbcontext)
        {
            _dbcontext = dbcontext;
        }

        public async Task DeleteAsync(Guid id)
        {
            var entity = await _dbcontext.PaymentCards.SingleOrDefaultAsync(x => x.Id == id);
            _dbcontext.PaymentCards.Remove(entity);
        }

        public async Task<IEnumerable<PaymentCardDto>> GetAllAsync()
        {
            var dbResult = await _dbcontext.PaymentCards.ToListAsync();

            return dbResult.Select(x => x.ToDto());
        }

        public async Task<PaymentCardDto> GetByIdAsync(Guid id)
        {
            var entity = await _dbcontext.PaymentCards.SingleOrDefaultAsync(x => x.Id == id);
            return entity==null?null:entity.ToDto(); 
        }

        public Task<ReadOnlyCollection<PaymentCardDto>> GetUserCards(Guid userId)
        {
            var dbRersult = _dbcontext.PaymentCards.Where(x => x.UserId == userId);
            var dtos = dbRersult.Select(x => x.ToDto());

            return Task.FromResult(new ReadOnlyCollection<PaymentCardDto>(dtos.ToList()));

        }

        public async Task UpdateAsync(PaymentCardDto dto)
        {
            var card = await _dbcontext.PaymentCards.SingleOrDefaultAsync(x => x.Id == dto.Id);
            card.IsDefault = dto.IsDefault;
        }

        public async Task<PaymentCardDto> AddAsync(PaymentCardDto dto)
        {
            var entity = dto.ToDbEntity();
            await _dbcontext.PaymentCards.AddAsync(entity);
            return entity.ToDto();
        }

        public async Task<PaymentCardDto> GetDefaultCard(Guid userId)
        {
            var dbRersult = await _dbcontext.PaymentCards.SingleOrDefaultAsync(x => x.UserId == userId && x.IsDefault);
            return dbRersult.ToDto();
        }
    }
}

