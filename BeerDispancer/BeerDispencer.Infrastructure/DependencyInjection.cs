using System;
using System.Runtime;
using Beerdispancer.Domain.Abstractions;
using Beerdispancer.Domain.Implementations;
using BeerDispancer.Application.Abstractions;
using BeerDispencer.Infrastructure.Migrations;
using BeerDispencer.Infrastructure.Persistence;
using BeerDispencer.Infrastructure.Persistence.Abstractions;
using BeerDispencer.Infrastructure.Persistence.Models;
using BeerDispencer.Infrastructure.Settings;
using FluentMigrator.Runner;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace BeerDispancer.Infrastructure
{
	public static class DependencyInjection
    {
        public static void AddInfrastructure(this IServiceCollection collection, ConfigurationManager configuration)
        {

            collection.AddDbContext<BeerDispencerDbContext>();
            collection.AddTransient<IBeerDispancerDbContext>(c => c.GetRequiredService<BeerDispencerDbContext>());

           // collection.AddTransient<IBeerDispancerDbContext, BeerDispencerDbContext>();
            collection.AddTransient<IDispencerUof, BeerDispancerUof>();
            collection.AddMigrations(configuration);

        }

        public static void AddMigrations(this IServiceCollection collection, ConfigurationManager configuration)
        {
            var dbSettings = configuration.GetSection(nameof(DBSettings)).Get<DBSettings>();

            collection
           .AddFluentMigratorCore()
           .ConfigureRunner(x => x.AddPostgres().WithGlobalConnectionString(dbSettings.ConnectionString)
           .ScanIn(typeof(BeerDispencer.Infrastructure.Migrations.M0001_CreateInitial).Assembly)
           .For
           .Migrations())
           .AddLogging(l => l.AddFluentMigratorConsole());

            collection.AddHostedService<MigratorJob>();
        }
    }
}

