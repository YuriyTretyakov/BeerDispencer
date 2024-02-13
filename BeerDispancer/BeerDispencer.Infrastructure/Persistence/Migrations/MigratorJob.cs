using BeerDispenser.Infrastructure.Settings;
using FluentMigrator.Runner;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace BeerDispenser.Infrastructure.Migrations
{
    public class MigratorJob
	{
        private readonly IServiceProvider _service;
        private readonly DBSettings _dbSettings;

        public MigratorJob(IServiceProvider service, IOptions<DBSettings> dbSettings)
		{
            _service = service;
            _dbSettings = dbSettings.Value;
        }

        public async Task ExecuteAsync()
        {
         
            using (var scope = _service.CreateScope())
            {

                var _migrationRunner = scope.ServiceProvider.GetService<IMigrationRunner>();               

                if (await CheckIfDbExistsAsync(_dbSettings.SpecialConnectionString, _dbSettings.DbName)!=true)
                {
                    await CreateDbAsync(_dbSettings.SpecialConnectionString, _dbSettings.DbName);
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
            using (SqlConnection conn = new SqlConnection(connectionStr))
            {
                string sql = $"SELECT DATNAME FROM pg_catalog.pg_database WHERE DATNAME = '{dbname}'";
                  
                var createDbSql = $"CREATE DATABASE \"{dbname}\"  WITH OWNER = postgres ENCODING = 'UTF8' CONNECTION LIMIT = -1;";

                object isDbExist;
                using (SqlCommand command = new SqlCommand
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
            using (SqlConnection conn = new SqlConnection(connectionStr))
            {
                var createDbSql = $"CREATE DATABASE \"{dbname}\"  WITH OWNER = postgres ENCODING = 'UTF8' CONNECTION LIMIT = -1;";

                using (SqlCommand command = new SqlCommand
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

