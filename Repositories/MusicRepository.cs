using MongoDB.Bson;
using MongoDB.Driver;
using WebProject.Datas;
using WebProject.Entities;

namespace WebProject.Repositories
{
    public class MusicRepository : IMusicRepository
    {
        private readonly IMongoCollection<Music> _musics;

        public MusicRepository(MongoDbService mongoDbService)
        {
            _musics = mongoDbService.Database.GetCollection<Music>("musics");
        }
        public async Task CreateMusicAsync(Music music)
        {
            await _musics.InsertOneAsync(music);
        }
        public async Task<bool> UpdateMusicAsync(Music music)
        {
            var updateResult = await _musics.ReplaceOneAsync(a => a.Id == music.Id, music);
            return updateResult.MatchedCount > 0;
        }
        public async Task<List<Music>> GetAllMusicByTitleAsync(string title)
        {
            var filter = Builders<Music>.Filter.Regex(a => a.Title, new MongoDB.Bson.BsonRegularExpression(title, "i"));
            return await _musics.Find(filter).ToListAsync();
        }
        //Випадковий вивід фіз-об'єктів:
        public async Task<List<Music>> Get10RandomMusicsAsync()
        {
            var pipeline = new[]
            {
        new BsonDocument("$sample", new BsonDocument("size", 10))
    };

            return await _musics.Aggregate<Music>(pipeline).ToListAsync();
        }

        public async Task<List<Music>> Get5RandomMusicByArtistIdAsync(string artistId)
{
    if (!ObjectId.TryParse(artistId, out var objectId))
        return new List<Music>();

    var pipeline = new[]
    {
        new BsonDocument("$match", new BsonDocument("ArtistId", objectId)),
        new BsonDocument("$sample", new BsonDocument("size", 5))
    };

    return await _musics.Aggregate<Music>(pipeline).ToListAsync();
}


        //Випадковий вивід тільки ID:
        public async Task<List<string>> Get10RandomMusicsIdAsync()
        {
            var pipeline = new[]
    {
            new BsonDocument("$sample", new BsonDocument("size", 10)),
            new BsonDocument("$project", new BsonDocument("_id", 1))
        };

            var result = await _musics.Aggregate<BsonDocument>(pipeline).ToListAsync();

            // Отримуємо список рядків Id
            return result.Select(doc => doc["_id"].ToString()).ToList();
        }
        public async Task<bool> IncrementListeningCountAsync(string MusicId)
        {
            var filter = Builders<Music>.Filter.Eq(m => m.Id, MusicId);
            var update = Builders<Music>.Update.Inc(m => m.CountOfListening, 1);

            var result = await _musics.UpdateOneAsync(filter, update);
            return result.ModifiedCount > 0;
        }
        public async Task<List<Music>> GetTopListenedMusicAsync(int count)
        {
            var sort = Builders<Music>.Sort.Descending(m => m.CountOfListening);
            return await _musics.Find(_ => true).Sort(sort).Limit(count).ToListAsync();
        }
        public async Task<List<Music>> GetAllMusicByArtistAsync(string artist)
        {
            var filter = Builders<Music>.Filter.Regex(a => a.ArtistId, new MongoDB.Bson.BsonRegularExpression(artist, "i"));
            return await _musics.Find(filter).ToListAsync();
        }
        public async Task<Music?> GetMusicByIdAsync(string id)
        {
            var filter = Builders<Music>.Filter.Eq(a => a.Id, id);
            return await _musics.Find(filter).FirstOrDefaultAsync();
        }
        public async Task<bool> DeleteMusicAsync(string id)
        {
            var deleteResult = await _musics.DeleteOneAsync(p => p.Id == id);
            return deleteResult.DeletedCount > 0;
        }
        public async Task DeleteMusicByArtistIdAsync(string artistId)
        {
            var filter = Builders<Music>.Filter.Eq(m => m.ArtistId, artistId);
            await _musics.DeleteManyAsync(filter);
        }
        public async Task<List<Music>> GetMusicByArtistIdAsync(string artistId)
        {
            var filter = Builders<Music>.Filter.Eq(m => m.ArtistId, artistId);
            return await _musics.Find(filter).ToListAsync();
        }


    }
}
