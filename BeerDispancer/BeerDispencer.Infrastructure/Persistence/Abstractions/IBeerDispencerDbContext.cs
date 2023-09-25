using System.Threading;
using BeerDispenser.Infrastructure.Persistence.Entities;
using Microsoft.EntityFrameworkCore;

namespace BeerDispenser.Infrastructure.Persistence.Abstractions
{
    public interface IBeerDispencerDbContext
    {
        DbSet<Dispenser> Dispencers { get; set; }
        DbSet<Usage> Usage { get; set; }
        void Dispose();
        Task<int> SaveChangesAsync(CancellationToken cancellationToken);
    }
}