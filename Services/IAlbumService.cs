using WebProject.DTO_s;
using WebProject.Entities;

namespace WebProject.Services
{
    public interface IAlbumService
    {
        Task<bool> CreateAlbumAsync(AlbumDTO albumDTO);
        Task<AlbumArtistDTO> GetAlbumAsync(string id);
        Task<Album> GetAlbumAsync1(string id);
        Task<List<AlbumWithTracksDTO>> Get10RandomAlbumsWithArtistAsync();
        Task InsertMusicIntoAlbumAsync(string albumId, string musicId);
    }
}
