using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace EmptyCourseFinderAPI.Models
{
    public class League
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string? Id { get; set; }
        public string? OwnerId { get; set; }
        public string LeagueName { get; set; }
        public string LeagueTime { get; set; }
        public string Location { get; set; }
        public string Description { get; set; }
        public List<string>? Players { get; set; }
        public bool Open { get; set; }
    }
}
