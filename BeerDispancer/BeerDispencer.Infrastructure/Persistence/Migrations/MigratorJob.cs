﻿using System;
using BeerDispencer.Infrastructure.Settings;
using FluentMigrator.Runner;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Npgsql;

namespace BeerDispencer.Infrastructure.Migrations
{
	public class MigratorJob:BackgroundService
	{
        private readonly IServiceProvider _service;
      
        public MigratorJob(IServiceProvider service)
		{
            _service = service;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
         
            using (var scope = _service.CreateScope())
            {

                IMigrationRunner _migrationRunner = scope.ServiceProvider.GetService<IMigrationRunner>();

                var dbSettings = scope.ServiceProvider.GetRequiredService<DBSettings>();

                

                if (await CheckIfDbExistsAsync(dbSettings.SpecialConnectionString, dbSettings.DbName)!=true)
                {
                    await CreateDbAsync(dbSettings.SpecialConnectionString, dbSettings.DbName);
                }
              

                if (_migrationRunner.HasMigrationsToApplyUp())
                {
                    _migrationRunner.ListMigrations();
                    _migrationRunner.MigrateUp();
                }
            }
        }

        public async Task<bool> CheckIfDbExistsAsync(string connectionStr, string dbname)
        {
            using (NpgsqlConnection conn = new NpgsqlConnection(connectionStr))
            {
                string sql = $"SELECT DATNAME FROM pg_catalog.pg_database WHERE DATNAME = '{dbname}'";
                  
                var createDbSql = $"CREATE DATABASE \"{dbname}\"  WITH OWNER = postgres ENCODING = 'UTF8' CONNECTION LIMIT = -1;";

                object isDbExist;
                using (NpgsqlCommand command = new NpgsqlCommand
                    (sql, conn))
                {
                    try
                    {
                        conn.Open();
                        isDbExist = await command.ExecuteScalarAsync();
                        conn.Close();

                    }
                    catch (Exception e)
                    {
                        Console.WriteLine(e.ToString());
                        return false;
                    }
                }
                return isDbExist!=null;
            }
        }

        public async Task CreateDbAsync(string connectionStr, string dbname)
        {
            using (NpgsqlConnection conn = new NpgsqlConnection(connectionStr))
            {
                var createDbSql = $"CREATE DATABASE \"{dbname}\"  WITH OWNER = postgres ENCODING = 'UTF8' CONNECTION LIMIT = -1;";

                using (NpgsqlCommand command = new NpgsqlCommand
                    (createDbSql, conn))
                {
                    try
                    {
                        conn.Open();
                        await command.ExecuteNonQueryAsync();
                        conn.Close();

                    }
                    catch (Exception e)
                    {
                        Console.WriteLine(e.ToString());
                    }
                }
            }
        }
    }
}
