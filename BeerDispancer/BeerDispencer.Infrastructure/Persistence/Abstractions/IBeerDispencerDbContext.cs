using System.Threading;
using BeerDispencer.Infrastructure.Persistence.Entities;
using Microsoft.EntityFrameworkCore;

namespace BeerDispencer.Infrastructure.Persistence.Abstractions
{
    public interface IBeerDispencerDbContext:IDisposable
    {
        DbSet<Dispencer> Dispencers { get; set; }
        DbSet<Usage> Usage { get; set; }

        DbSet<Outbox> Outbox { get; set; }
        DbSet<Payments> Payments { get; set; }
        void Dispose();
        Task<int> SaveChangesAsync(CancellationToken cancellationToken);
    }
}