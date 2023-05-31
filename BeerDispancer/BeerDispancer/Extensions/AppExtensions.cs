using System;
using BeerDispancer.Application.Abstractions;
using BeerDispencer.Infrastructure.Settings;
using FluentMigrator.Runner;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

namespace BeerDispencer.WebApi.Extensions
{
	public static class AppExtensions
	{
        public static void AddSettings(this IServiceCollection collection, ConfigurationManager configuration)
        {
            var dbSettings = configuration.GetSection(nameof(DBSettings)).Get<DBSettings>();
            collection.AddSingleton<DBSettings>(dbSettings);

            var beerFlowSettings = configuration.GetSection(nameof(BeerFlowSettings)).Get<BeerFlowSettings>();
            collection.AddSingleton<IBeerFlowSettings>(beerFlowSettings);
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
        }
    }
}

