﻿using BeerDispencer.Infrastructure.Persistence.Abstractions;
using BeerDispencer.Infrastructure.Persistence.Entities;
using BeerDispencer.Infrastructure.Settings;
using Microsoft.EntityFrameworkCore;
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

        
        DbSet<Dispencer> IBeerDispencerDbContext.Dispencers { get ; set ; }
        DbSet<Usage> IBeerDispencerDbContext.Usage { get; set; }
        DbSet<Outbox> IBeerDispencerDbContext.Outbox { get; set; }
        DbSet<Payments> IBeerDispencerDbContext.Payments { get; set; }


        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            var connectionStr = _dbSettings.ConnectionString;
            optionsBuilder.UseNpgsql(connectionStr);

        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Dispencer>(x=>x.ToTable("Dispencer").HasKey(x => x.Id));
            modelBuilder.Entity<Usage>(x=>x.ToTable("Usage").HasKey(x => x.Id));
            modelBuilder.Entity<Outbox>(x => x.ToTable("Outbox").HasKey(x => x.Id));
            modelBuilder.Entity<Payments>(x => x.ToTable("Payments").HasKey(x => x.Id));

        }

    }
}

