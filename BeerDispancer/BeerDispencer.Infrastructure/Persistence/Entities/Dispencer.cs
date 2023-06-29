using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace BeerDispencer.Infrastructure.Persistence.Entities
{
    public class Dispencer
    {
        [BsonId]
        public ObjectId Id { get; set; }
        public double? Volume { get; set; }
        public DispencerStatus? Status { get; set; }
    }
}

