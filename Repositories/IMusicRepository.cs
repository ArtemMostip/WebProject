using MongoDB.Bson;
using WebProject.DTO_s;
using WebProject.Entities;

namespace WebProject.Repositories
{
    public interface IMusicRepository
    {
        Task CreateMusicAsync(Music music);
        Task<bool> UpdateMusicAsync(Music music);
        Task<List<Music>> GetAllMusicByTitleAsync(string title);
        Task<Music?> GetMusicByIdAsync(string id);
        Task<List<Music>> GetAllMusicByArtistAsync(string artist);
        Task<bool> DeleteMusicAsync(string id);
        Task DeleteMusicByArtistIdAsync(string id);
        Task<List<Music>> GetMusicByArtistIdAsync(string artistId);
        Task<List<Music>> Get10RandomMusicsAsync();
        Task<List<Music>> GetTopListenedMusicAsync(int count);
        Task<List<string>> Get10RandomMusicsIdAsync();
        Task<bool> IncrementListeningCountAsync(string musicId);
        Task<List<Music>> Get5RandomMusicByArtistIdAsync(string artistId);
    }
}
