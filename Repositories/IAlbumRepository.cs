using MongoDB.Driver;
using WebProject.DTO_s;
using WebProject.Entities;

namespace WebProject.Repositories
{
    public interface IAlbumRepository
    {
        Task CreateAlbumAsync(Album album);
        Task DeleteAlbumsByArtistIdAsync(string id);
        Task<Album> GetAlbumByIdAsync(string id);
        Task<List<Album>> GetAllAlbumsByTitleAsync(string title);
        Task<bool> UpdateAlbumAsync(Album album);
        Task<List<string>> Get10RandomAlbumIdAsync();
        Task<List<Album>> Get10RandomAlbumsAsync();
        Task AddMusicToAlbumAsync(string albumId, string musicId);
        Task<List<Album>> GetAllAlbumsByIdAsync(string id);
        Task<List<AlbumWithMusicDTO>> GetAlbumsByArtistIdAsync(string artistId);

    }
}
