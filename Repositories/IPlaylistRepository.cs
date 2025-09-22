//Task<Playlist?> GetPlaylistWithMusicsAsync(string id);

using WebProject.Entities;

namespace WebProject.Repositories
{
    public interface IPlaylistRepository
    {
        Task CreatePlaylistAsync(Playlist playlist);
        Task<bool> UpdatePlaylistAsync(Playlist playlist);
        Task DeleteMusicByArtistIdFromPlaylistAsync(string musicId);
        Task<Playlist> GetPlaylistByIdAsync(string id);
        Task<bool> DeletePlaylistAsync(string id);
        Task DeleteMusicFromPlaylistsAsync(string musicId);
    }
}
