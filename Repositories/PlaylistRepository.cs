//public async Task<Playlist?> GetPlaylistWithMusicsAsync(string id)
//{
//    var playlist = await _playlist.Find(p => p.Id == id).FirstOrDefaultAsync();
//    if (playlist == null || playlist.MusicId == null || !playlist.MusicId.Any())
//    {
//        return playlist;
//    }

//    var musicFilter = Builders<Music>.Filter.In(m => m.Id, playlist.MusicId.Select(x => x.ToString()));
//    var musics = await _musics.Find(musicFilter).ToListAsync();
//    return playlist;
//}


using MongoDB.Bson;
using MongoDB.Driver;
using WebProject.Datas;
using WebProject.Entities;

namespace WebProject.Repositories
{
    public class PlaylistRepository : IPlaylistRepository
    {
        private readonly IMongoCollection<Playlist> _playlist;
        private readonly IMusicRepository _musicRepository;
        public PlaylistRepository(MongoDbService mongoDbService, IMusicRepository musicRepository)
        {
            _playlist = mongoDbService.Database.GetCollection<Playlist>("playlists");
            _musicRepository = musicRepository;
        }
        public async Task CreatePlaylistAsync(Playlist playlist)
        {
            await _playlist.InsertOneAsync(playlist);
        }
        public async Task<bool> UpdatePlaylistAsync(Playlist playlist)
        {
            var updateResult = await _playlist.ReplaceOneAsync(a => a.Id == playlist.Id, playlist);
            return updateResult.MatchedCount > 0;
        }

        public async Task<Playlist> GetPlaylistByIdAsync(string id)
        {
            return await _playlist.Find(a => a.Id == id).FirstOrDefaultAsync();
        }
        public async Task<bool> DeletePlaylistAsync(string id)
        {
            var deleteResult = await _playlist.DeleteOneAsync(p => p.Id == id);
            return deleteResult.DeletedCount > 0;
        }
        public async Task DeleteMusicByArtistIdFromPlaylistAsync(string artistId)
        {
            var musics = await _musicRepository.GetMusicByArtistIdAsync(artistId);
            var musicIds = musics.Select(m => m.Id).ToList(); 

            if (!musicIds.Any()) return;

            var filter = Builders<Playlist>.Filter.AnyIn("MusicId", musicIds);
            var update = Builders<Playlist>.Update.PullAll("MusicId", musicIds);

            await _playlist.UpdateManyAsync(filter, update);
        }
        public async Task DeleteMusicFromPlaylistsAsync(string musicId)
        {
            var filter = Builders<Playlist>.Filter.AnyEq(p => p.MusicId, musicId);
            var update = Builders<Playlist>.Update.Pull(p => p.MusicId, musicId);

            await _playlist.UpdateManyAsync(filter, update);
        }
    }
}
