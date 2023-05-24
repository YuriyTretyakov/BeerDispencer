using System;
using System.Text;
using BeerDispencer.Infrastructure.Settings;
using FluentMigrator;
using FluentMigrator.Runner;
using Microsoft.Extensions.DependencyInjection;

namespace BeerDispancer.Extensions
{
    public static class AppServiceExtensions
    {
        public static void AddMigrations(this IServiceCollection collection, string connstring)
        {
            collection
           .AddFluentMigratorCore()
           .ConfigureRunner(x => x.AddPostgres().WithGlobalConnectionString(connstring)
           .ScanIn(typeof(Migrations.M0001_CreateInitial).Assembly)
           .For
           .Migrations())
           .AddLogging(l => l.AddFluentMigratorConsole());
        }

    }
}

