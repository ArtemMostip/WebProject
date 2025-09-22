using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;

namespace WebProject.Entities
{
    public class Artist
    {
        [BsonId, BsonElement("_id"), BsonRepresentation(BsonType.ObjectId)]
        public string? Id { get; set; }
        public required string Name { get; set; }
        public required string AvatarUrl { get; set; }
        public required string ArtistBannerUrl { get; set; }
        public string? ArtistLogo { get; set; }
    }
}
