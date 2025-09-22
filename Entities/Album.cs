using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;

namespace WebProject.Entities
{
    public class Album
    {
        [BsonId, BsonElement("_id"), BsonRepresentation(BsonType.ObjectId)]
        public string? Id { get; set; }
        public required string Title { get; set; }
        [BsonRepresentation(BsonType.ObjectId)]
        public string? ArtistId { get; set; }
         public required List<string>? MusicInAlbum { get; set; }
        public required string AlbCoverUrl { get; set; }
        public int? CountOfMusicInAlbum { get; set; }
    }
}
