using BeerDispancer.Application.DTO;
using BeerDispencer.Application.Abstractions;
using BeerDispencer.Infrastructure.Extensions;
using BeerDispencer.Infrastructure.Persistence.Abstractions;
using BeerDispencer.Infrastructure.Persistence.Entities;
using MongoDB.Bson;
using MongoDB.Driver;

namespace BeerDispencer.Infrastructure.Persistence
{
    public class UsageRepository : IUsageRepository
    {
        private readonly IBeerDispencerDbContext _dbcontext;

        public UsageRepository(IBeerDispencerDbContext dbcontext)
        {
            _dbcontext = dbcontext;
        }


        public async Task<UsageDto> AddAsync(UsageDto dto)
        {
            var entity = dto.ToDbEntity();
             await _dbcontext
                .Usage
                .InsertOneAsync(entity);
            return entity.ToDto();
        }

        public async Task<IEnumerable<UsageDto>> GetAllAsync()
        {
            var entityList = await (await _dbcontext.Usage.FindAsync(_=>true)).ToListAsync();
            return entityList.Cast<UsageDto>();
        }

        public async Task<UsageDto> GetByIdAsync(string id)
        {

            var entity =await (await _dbcontext
                        .Usage
                       .FindAsync(x => x.Id == new ObjectId(id))).SingleOrDefaultAsync();

            return entity==null?null:entity.ToDto();
               
        }

       
        public async Task UpdateAsync(UsageDto dto)
        {
            var updateBuilder = Builders<Usage>.Update;
            var updateDefinitions = new List<UpdateDefinition<Usage>>();

            if (dto.ClosedAt is not null)
            {
                updateDefinitions.Add(updateBuilder.Set(x => x.ClosedAt, dto.ClosedAt));
            }

            if (dto.FlowVolume is not null)
            {
                updateDefinitions.Add(updateBuilder.Set(x => x.FlowVolume, dto.FlowVolume));
            }

            if (dto.TotalSpent is not null)
            {
                updateDefinitions.Add(updateBuilder.Set(x => x.TotalSpent, dto.TotalSpent));
            }

            if (dto.DispencerId is not null)
            {
                updateDefinitions.Add(updateBuilder.Set(x => x.DispencerId, ObjectId.Parse(dto.DispencerId)));
            }

            if (dto.OpenAt is not null)
            {
                updateDefinitions.Add(updateBuilder.Set(x => x.OpenAt, dto.OpenAt));
            }

            await _dbcontext.Usage.UpdateOneAsync(x => x.Id == ObjectId.Parse(dto.Id), updateBuilder.Combine(updateDefinitions));
        
        }

        public async Task DeleteAsync(string id)
        {
           await _dbcontext
               .Usage.DeleteOneAsync(x=>x.Id==new ObjectId(id));
        }

        public async Task<UsageDto[]> GetByDispencerIdAsync(string dispencerId)
        {
            
            var entitiesList =  await (await _dbcontext
               .Usage.FindAsync(Builders<Usage>.Filter.Eq(x=> x.DispencerId, new ObjectId(dispencerId)))).ToListAsync();


            return entitiesList
                .Select(x =>
                new UsageDto
                {
                    ClosedAt = x.ClosedAt,
                    Id=x.Id.ToString(),
                    OpenAt = x.OpenAt,
                    FlowVolume = x.FlowVolume,
                    TotalSpent=x.TotalSpent,
                    DispencerId =x.DispencerId.ToString()
                }).ToArray() ;
        }
    }
}

