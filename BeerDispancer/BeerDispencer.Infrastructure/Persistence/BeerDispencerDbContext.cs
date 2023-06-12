using System;
using BeerDispencer.Infrastructure.Persistence.Abstractions;
using BeerDispencer.Infrastructure.Persistence.Entities;
using BeerDispencer.Infrastructure.Settings;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;

namespace BeerDispencer.Infrastructure.Persistence.Models
{
    public class BeerDispencerDbContext : DbContext, IBeerDispancerDbContext
    {
        private readonly IServiceProvider _service;
        private readonly DBSettings _dbSettings;

        public BeerDispencerDbContext(IServiceProvider service, DBSettings dbSettings)
        {
            _service = service;
            _dbSettings = dbSettings;
        }

        
        DbSet<Dispencer> IBeerDispancerDbContext.Dispencers { get ; set ; }
       
        DbSet<Usage> IBeerDispancerDbContext.Usage { get; set; }


        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            var connectionStr = _dbSettings.ConnectionString;
            optionsBuilder.UseNpgsql(connectionStr);

        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Dispencer>(x=>x.ToTable("Dispencer").HasKey(x => x.Id));
            modelBuilder.Entity<Usage>(x=>x.ToTable("Usage").HasKey(x => x.Id));
            
        }

    }
}

