using System.Threading;
using BeerDispencer.Infrastructure.Persistence.Entities;
using BeerDispenser.Infrastructure.Persistence.Entities;
using Microsoft.EntityFrameworkCore;

namespace BeerDispenser.Infrastructure.Persistence.Abstractions
{
    public interface IBeerDispencerDbContext
    {
        DbSet<Dispenser> Dispencers { get; set; }
        DbSet<Usage> Usage { get; set; }
        DbSet<PaymentCard> PaymentCards { get; set; }
        void Dispose();
        Task<int> SaveChangesAsync(CancellationToken cancellationToken);
    }
}