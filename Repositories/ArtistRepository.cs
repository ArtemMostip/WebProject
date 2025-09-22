using MongoDB.Bson;
using MongoDB.Driver;
using WebProject.Datas;
using WebProject.Entities;

namespace WebProject.Repositories
{
    public class ArtistRepository : IArtistRepository
    {
        private readonly IMongoCollection<Artist> _artists;

        public ArtistRepository(MongoDbService mongoDbService)
        {
            _artists = mongoDbService.Database.GetCollection<Artist>("artists");

        }
        public async Task CreateArtistAsync(Artist artist)
        {
            await _artists.InsertOneAsync(artist);
        }

        public async Task<Artist> GetArtistByIdAsync(string id)
        {
            return await _artists.Find(a => a.Id == id).FirstOrDefaultAsync();
        }
        public async Task<List<Artist>> GetAllArtistsByNameAsync(string name)
        {
            var filter = Builders<Artist>.Filter.Regex(a => a.Name, new MongoDB.Bson.BsonRegularExpression(name, "i"));
            return await _artists.Find(filter).ToListAsync();
        }
        public async Task<bool> UpdateArtistAsync(Artist artist)
        {
            var updateResult = await _artists.ReplaceOneAsync(a => a.Id == artist.Id, artist);
            return updateResult.MatchedCount > 0;
        }

        public async Task<bool> DeleteArtistAsync(string id)
        {
            var deleteResult = await _artists.DeleteOneAsync(p => p.Id == id);
            return deleteResult.DeletedCount > 0;
        }
        //Випадковий вивід фіз-об'єктів:
        public async Task<List<Artist>> Get5RandomArtistsAsync()
        {
            var pipeline = new[]
            {
        new BsonDocument("$sample", new BsonDocument("size", 5))
    };

            return await _artists.Aggregate<Artist>(pipeline).ToListAsync();
        }

        //Випадковий вивід тільки ID:
        public async Task<List<string>> Get5RandomArtistsIdAsync()
        {
            var pipeline = new[]
    {
            new BsonDocument("$sample", new BsonDocument("size", 5)),
            new BsonDocument("$project", new BsonDocument("_id", 1))
        };

            var result = await _artists.Aggregate<BsonDocument>(pipeline).ToListAsync();

            // Отримуємо список рядків Id
            return result.Select(doc => doc["_id"].ToString()).ToList();
        }

    }
}
