using WebProject.DTO_s;
using WebProject.Entities;

namespace WebProject.Services
{
    public interface IArtistService 
    {
        Task<bool> CreateArtistAsync(ArtistDTO artistDTO);
        Task<Artist> GetArtist(string id);
        Task<bool> DeleteArtistAsync(string id);
        Task<bool> DeleteMusicByArtistIdFromPlaylistAsync(string id);
        Task<Artist> GetArtistByIdAsync(string id);
        Task<ArtistWithAlbumsDTO?> OutputAllAlbumsByArtistId(string id);
    }
}
