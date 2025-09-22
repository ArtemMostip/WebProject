using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;
using WebProject.DTO_s;

namespace WebProject.Entities
{
    public class Playlist
    {
        [BsonId, BsonElement("_id"), BsonRepresentation(BsonType.ObjectId)]
        public string? Id { get; set; }
        public required string AccountId { get; set; }
        public required string Name { get; set; } = string.Empty;

        //[BsonRepresentation(BsonType.ObjectId)]
        public List<string>? MusicId { get; set; }

    }
}
