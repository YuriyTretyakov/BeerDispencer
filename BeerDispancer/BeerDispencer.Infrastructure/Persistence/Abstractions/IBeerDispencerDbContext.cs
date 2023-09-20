using System.Threading;
using BeerDispencer.Infrastructure.Persistence.Entities;
using Microsoft.EntityFrameworkCore;

namespace BeerDispencer.Infrastructure.Persistence.Abstractions
{
    public interface IBeerDispencerDbContext
    {
        DbSet<Dispenser> Dispencers { get; set; }
        DbSet<Usage> Usage { get; set; }
        void Dispose();
        Task<int> SaveChangesAsync(CancellationToken cancellationToken);
    }
}