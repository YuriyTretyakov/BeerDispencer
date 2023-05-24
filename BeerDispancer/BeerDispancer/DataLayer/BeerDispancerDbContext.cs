using System;
using BeerDispancer.DataLayer.Models;
using BeerDispancer.DomainLayer;
using BeerDispencer.Infrastructure.Settings;
using Microsoft.EntityFrameworkCore;

namespace BeerDispancer.DataLayer
{
	public class BeerDispancerDbContext:DbContext
	{
        private readonly IServiceProvider _service;

        public BeerDispancerDbContext(IServiceProvider service)
		{
            _service = service;
        }

		public DbSet<Dispencer> Dispencers { get; set; }
        public DbSet<Usage> Usage { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            DbSettings settings;
            using (var scope = _service.CreateScope())
            {
                settings = scope.ServiceProvider.GetRequiredService<DbSettings>();
            }


            var connectionStr = settings.ConnectionString;
            optionsBuilder.UseNpgsql(connectionStr);

            
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            //modelBuilder.Entity<Dispencer>()
            //    .Property(b => b.Id)
            //    .HasDefaultValueSql("NEWID()");
        }
    }
}

