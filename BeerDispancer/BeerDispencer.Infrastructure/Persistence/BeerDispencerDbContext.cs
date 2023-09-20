using System;
using BeerDispencer.Infrastructure.Persistence.Abstractions;
using BeerDispencer.Infrastructure.Persistence.Entities;
using BeerDispencer.Infrastructure.Settings;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.Extensions.Options;

namespace BeerDispencer.Infrastructure.Persistence.Models
{
    public class BeerDispencerDbContext : DbContext, IBeerDispencerDbContext
    {
        
        private readonly DBSettings _dbSettings;

        public BeerDispencerDbContext(IServiceProvider service, IOptions<DBSettings> dbSettings)
        {
            _dbSettings = dbSettings.Value;
        }

        
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
            
        }

    }
}

