using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace BeerDispencer.Infrastructure.Persistence.Entities
{
    public class Usage
    {
        [BsonId]
        public ObjectId Id { get; set; }
        public ObjectId DispencerId { get; set; }
        public DateTime? OpenAt { get; set; }
        public DateTime? ClosedAt { get; set; }
        public double? FlowVolume { get; set; }
        public double? TotalSpent { get; set; }
    }
}

