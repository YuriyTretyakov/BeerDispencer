using BeerDispencer.Infrastructure.Persistence.Entities;
using MongoDB.Driver;

namespace BeerDispencer.Infrastructure.Persistence.Abstractions
{
    public interface IBeerDispencerDbContext
    {
        IMongoCollection<Dispencer> Dispencers { get; }
        IMongoCollection<Usage> Usage { get; }
        MongoClient Client { get;   }
    }
}