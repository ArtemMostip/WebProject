using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;

namespace WebProject.Entities
{
    public class Music
    {
        [BsonId, BsonElement("_id"), BsonRepresentation(BsonType.ObjectId)]
        public string? Id { get; set; }

        public required string Title { get; set; }
        [BsonRepresentation(BsonType.ObjectId)]
        public string? ArtistId { get; set; }


        [BsonRepresentation(BsonType.ObjectId)]
        public string? AlbumId { get; set; }
        public int Year { get; set; }
        public required string[] Genre { get; set; }

        public required string CoverUrl { get; set; }

        public required string FileUrl { get; set; }
        public DateTime DateCreation { get; set; }

        public int CountOfListening { get; set; } = 0;
    }
}
