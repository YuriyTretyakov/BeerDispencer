using BeerDispencer.Infrastructure.Persistence.Entities;
using BeerDispenser.Infrastructure.Persistence.Abstractions;
using BeerDispenser.Infrastructure.Persistence.Entities;
using BeerDispenser.Infrastructure.Settings;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace BeerDispenser.Infrastructure.Persistence.Models
{
    public class BeerDispencerDbContext : DbContext, IBeerDispencerDbContext
    {
        
        private readonly DBSettings _dbSettings;

        public BeerDispencerDbContext(IServiceProvider service, IOptions<DBSettings> dbSettings)
        {
            _dbSettings = dbSettings.Value;
        }

        public DbSet<PaymentCard> PaymentCards { get ; set; }
        public DbSet<Outbox> Outbox { get; set ; }
        DbSet<Dispenser> IBeerDispencerDbContext.Dispencers { get ; set ; }
       
        DbSet<Usage> IBeerDispencerDbContext.Usage { get; set; }


        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            var connectionStr = _dbSettings.ConnectionString;
            optionsBuilder.UseNpgsql(connectionStr);

        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Dispenser>(x=>x.ToTable("Dispencer").HasKey(x => x.Id));
            modelBuilder.Entity<Usage>(x=>x.ToTable("Usage").HasKey(x => x.Id));
            modelBuilder.Entity<PaymentCard>(x => x.ToTable("PaymentCard").HasKey(x => x.Id));

        }

    }
}

