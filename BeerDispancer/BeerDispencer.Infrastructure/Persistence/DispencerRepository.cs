using BeerDispancer.Application.DTO;
using BeerDispencer.Application.Abstractions;
using BeerDispencer.Infrastructure.Extensions;
using BeerDispencer.Infrastructure.Persistence.Abstractions;
using BeerDispencer.Infrastructure.Persistence.Entities;
using MongoDB.Bson;
using MongoDB.Driver;

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
            await _dbcontext.Dispencers.InsertOneAsync(entity);
            return entity.ToDto();
        }

        public async Task DeleteAsync(string id)
        {
             await _dbcontext.Dispencers.DeleteOneAsync( x=>x.Id==ObjectId.Parse(id));
        }

        public async Task<IEnumerable<DispencerDto>> GetAllAsync()
        {
            var dbResult = await _dbcontext.Dispencers.FindAsync(_ => true);
            return (await dbResult.ToListAsync()).Cast<DispencerDto>();
        }

        public async Task<DispencerDto> GetByIdAsync(string id)
        {
     
            var entity = await _dbcontext
                .Dispencers
                .Find(Builders<Dispencer>.Filter.Eq("_id", ObjectId.Parse(id)))
                .FirstOrDefaultAsync();

            return entity==null?null: entity.ToDto(); 
        }

        public async Task UpdateAsync(DispencerDto dispencerDto)
        {

            var updateBuilder = Builders<Dispencer>.Update;
            var updateDefinitions = new List<UpdateDefinition<Dispencer>>();

            if (dispencerDto.Status is not null)
            {
                updateDefinitions.Add(updateBuilder.Set(x => x.Status, DispencerExtensions.ToDbEntity(dispencerDto.Status)));  
            }

            if (dispencerDto.Volume is not null)
            {
                updateDefinitions.Add(updateBuilder.Set(x => x.Volume, dispencerDto.Volume));
            }


            await _dbcontext
                .Dispencers
                .UpdateOneAsync(x => x.Id == ObjectId.Parse(dispencerDto.Id),
                updateBuilder.Combine(updateDefinitions));

        }        
    }
}

