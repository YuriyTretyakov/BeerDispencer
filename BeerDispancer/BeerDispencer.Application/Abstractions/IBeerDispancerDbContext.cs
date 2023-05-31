using System.Threading;
using Beerdispancer.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace BeerDispancer.Application.Abstractions
{
    public interface IBeerDispancerDbContext
    {
        DbSet<DispencerDto> Dispencers { get; set; }
        DbSet<UsageDto> Usage { get; set; }
        void Dispose();
        Task<int> SaveChangesAsync(CancellationToken cancellationToken);
    }
}