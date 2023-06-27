using BeerDispencer.Infrastructure.Persistence.Abstractions;
using BeerDispencer.Infrastructure.Persistence.Entities;
using BeerDispencer.Infrastructure.Settings;
using Microsoft.Extensions.Options;
using MongoDB.Bson;
using MongoDB.Driver;

namespace BeerDispencer.Infrastructure.Persistence.Models
{
    public class BeerDispencerDbContext : IBeerDispencerDbContext
    {
        
        private readonly DBSettings _dbSettings;

        public BeerDispencerDbContext(IServiceProvider service, IOptions<DBSettings> dbSettings)
        {
            _dbSettings = dbSettings.Value;
            BsonDefaults.GuidRepresentation = GuidRepresentation.Standard;
            Client = new MongoClient(_dbSettings.ConnectionString);
        }

        public MongoClient Client { get; private set; }

        IMongoCollection<Dispencer> IBeerDispencerDbContext.Dispencers =>
            Client
            .GetDatabase(_dbSettings.DbName)
            .GetCollection<Dispencer>(nameof(Dispencer));

        IMongoCollection<Usage> IBeerDispencerDbContext.Usage=>
            Client
            .GetDatabase(_dbSettings.DbName)
            .GetCollection<Usage>(nameof(Usage));


    }
}

