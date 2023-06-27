using BeerDispancer.Application.DTO;
using BeerDispencer.Application.Abstractions;
using Microsoft.Extensions.Caching.Memory;

namespace BeerDispencer.Infrastructure.Persistence
{
    public class CachedUsageRepository : IUsageRepository
    {
        private readonly IMemoryCache _memoryCache;
        IUsageRepository _decorated;

        public CachedUsageRepository(UsageRepository usageRepository, IMemoryCache memoryCache)
        {
            _decorated = usageRepository;
            _memoryCache = memoryCache;
        }
        public Task<UsageDto> AddAsync(UsageDto dto)
        {
            return _decorated.AddAsync(dto);
        }

        public Task DeleteAsync(string id)
        {
            return _decorated.DeleteAsync(id);
        }

        public Task<IEnumerable<UsageDto>> GetAllAsync()
        {
            return _memoryCache.GetOrCreateAsync(
                nameof(GetAllAsync),
                x =>
                {
                    x.SetAbsoluteExpiration(TimeSpan.FromSeconds(30));
                    return _decorated.GetAllAsync();
                }
              );
        }

        public Task<UsageDto[]> GetByDispencerIdAsync(string dispencerId)
        {
            return _memoryCache.GetOrCreateAsync(
                nameof(GetByDispencerIdAsync),
                x =>
                {
                    x.SetAbsoluteExpiration(TimeSpan.FromSeconds(30));
                    return _decorated.GetByDispencerIdAsync(dispencerId);
                }
              );
        }

        public Task<UsageDto> GetByIdAsync(string id)
        {
            return _memoryCache.GetOrCreateAsync(
                nameof(GetByIdAsync),
                x =>
                {
                    x.SetAbsoluteExpiration(TimeSpan.FromSeconds(30));
                
                    return _decorated.GetByIdAsync(id);
                }
              );
        }

        public Task UpdateAsync(UsageDto dto)
        {
            return _decorated.UpdateAsync(dto);
        }
    }
}

