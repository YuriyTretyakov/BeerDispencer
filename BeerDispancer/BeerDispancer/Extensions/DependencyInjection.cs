using System;
using BeerDispencer.Infrastructure.Migrations;
using BeerDispencer.Infrastructure.Settings;
using FluentMigrator.Runner;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;


namespace BeerDispencer.WebApi.Extensions
{
	public static class DependencyInjection
    {
        public static void AddSettings(this IServiceCollection collection, ConfigurationManager configuration)
        {
            var dbSettings = configuration.GetSection(nameof(DBSettings)).Get<DBSettings>();
            collection.AddSingleton<DBSettings>(dbSettings);
        }

        
    }
}

