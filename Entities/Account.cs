using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace WebProject.Entities

{
    public class Account
    {
        [BsonId, BsonElement("_id"), BsonRepresentation(BsonType.ObjectId)]
        public string? Id { get; set; }
        public required string Name { get; set; }

        public required string Email { get; set; }

        public required string PasswordHash { get; set; }
        public DateTime? VerifiedAt { get; set; }
        public required string Token { get; set; }
        public DateTime DateCreation { get; set; }
        public string Role { get; set; } = "user";

        [BsonElement("avatar")]
        public byte[]? AvatarImage { get; set; }
    }
}
