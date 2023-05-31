using System;
using Beerdispancer.Domain.Entities;
using BeerDispancer.Application.Abstractions;
using BeerDispencer.Infrastructure.Settings;
using Microsoft.EntityFrameworkCore;

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

        
        DbSet<DispencerDto> IBeerDispancerDbContext.Dispencers { get ; set ; }
       
        DbSet<UsageDto> IBeerDispancerDbContext.Usage { get; set; }
        

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            var connectionStr = _dbSettings.ConnectionString;
            optionsBuilder.UseNpgsql(connectionStr);

        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<DispencerDto>(x=>x.ToTable("Dispencer").HasKey(x => x.Id));
            modelBuilder.Entity<UsageDto>(x=>x.ToTable("Usage").HasKey(x => x.Id));
            
        }
    }
}

