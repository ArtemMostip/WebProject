using MongoDB.Bson;
using MongoDB.Driver;
using WebProject.Datas;
using WebProject.DTO_s;
using WebProject.Entities;
using WebProject.Services;

namespace WebProject.Repositories
{
    public class AlbumRepository : IAlbumRepository
    {
        private readonly IMongoCollection<Album> _album;
        private readonly Lazy<IIMusicService> _musicService;

        public AlbumRepository(MongoDbService mongoDbService, Lazy<IIMusicService> musicService )
        {
            _album = mongoDbService.Database.GetCollection<Album>("albums");
            _musicService = musicService;
        }
        public async Task AddMusicToAlbumAsync(string albumId, string musicId)
{
    var filter = Builders<Album>.Filter.Eq(a => a.Id, albumId);
    var update = Builders<Album>.Update.AddToSet("MusicInAlbum", musicId);
    await _album.UpdateOneAsync(filter, update);
}
        public async Task CreateAlbumAsync(Album album)
        {
            await _album.InsertOneAsync(album);
        }
        public async Task<Album> GetAlbumByIdAsync(string id)
        {
            return await _album.Find(a => a.Id == id).FirstOrDefaultAsync();
        }
        public async Task<List<Album>> GetAllAlbumsByTitleAsync(string title)
        {
            var filter = Builders<Album>.Filter.Regex(a => a.Title, new MongoDB.Bson.BsonRegularExpression(title, "i"));
            return await _album.Find(filter).ToListAsync();
        }
        public async Task<bool> UpdateAlbumAsync(Album album)
        {
            var updateResult = await _album.ReplaceOneAsync(a => a.Id == album.Id, album);
            return updateResult.MatchedCount > 0;
        }
        public async Task DeleteAlbumsByArtistIdAsync(string artistId)
        {
            var filter = Builders<Album>.Filter.Eq(a => a.ArtistId, artistId);
            await _album.DeleteManyAsync(filter);
        }

        public async Task<List<Album>> Get10RandomAlbumsAsync()
        {
            var pipeline = new[]
            {
        new BsonDocument("$sample", new BsonDocument("size", 10))
    };

            return await _album.Aggregate<Album>(pipeline).ToListAsync();
        }

        //Випадковий вивід тільки ID:
        public async Task<List<string>> Get10RandomAlbumIdAsync()
        {
            var pipeline = new[]
    {
            new BsonDocument("$sample", new BsonDocument("size", 10)),
            new BsonDocument("$project", new BsonDocument("_id", 1))
        };

            var result = await _album.Aggregate<BsonDocument>(pipeline).ToListAsync();

            // Отримуємо список рядків Id
            return result.Select(doc => doc["_id"].ToString()).ToList();
        }

        public async Task<List<Album>> GetAllAlbumsByIdAsync(string id)
        {
            var filter = Builders<Album>.Filter.Regex(a => a.Id, new MongoDB.Bson.BsonRegularExpression(id, "i"));
            return await _album.Find(filter).ToListAsync();
        }
        public async Task<List<AlbumWithMusicDTO>> GetAlbumsByArtistIdAsync(string artistId)
        {
            var filter = Builders<Album>.Filter.Eq(a => a.ArtistId, artistId);
            var albums = await _album.Find(filter).ToListAsync();

            var result = new List<AlbumWithMusicDTO>();

            foreach (var album in albums)
            {
                List<MusicArtistDTO> musics = new List<MusicArtistDTO>();

                if (album.MusicInAlbum != null && album.MusicInAlbum.Any())
                {
                    foreach (var musicId in album.MusicInAlbum)
                    {
                        var music = await _musicService.Value.GetFullMusicArtistByIdAsync(musicId);
                        if (music != null)
                        {
                            musics.Add(music);
                        }
                    }
                }

                result.Add(new AlbumWithMusicDTO
                {
                    Id = album.Id,
                    Title = album.Title,
                    AlbCoverUrl = album.AlbCoverUrl,
                    CountOfMusicInAlbum = album.CountOfMusicInAlbum ?? 0,
                    Music = musics // додаємо повні пісні тут
                });
            }

            return result;
        }

    }
}
