using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;

namespace WebProject.DTO_s
{
    public class PlaylistDTO
    {
        public required string AccountId { get; set; }
        public required string Name { get; set; } = string.Empty;

        //[BsonElement("MusicId")]
        //[BsonRepresentation(BsonType.ObjectId)]
        public List<string>? MusicId { get; set; }
    }
}
