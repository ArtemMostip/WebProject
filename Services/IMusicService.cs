using System.Threading.Tasks;
using WebProject.DTO_s;
using WebProject.Entities;

namespace WebProject.Services
{
    public interface IMusicService
    {
        Task<bool> CreateMusicAsync(MusicInpDTO musicInpDTO);
        Task<List<MusicArtistDTO>> GetMusicsArtistByTitle(string title);
        Task<List<Music>> GetMusicsByTitle(string title);
        Task<List<Music>> GetMusicsByArtist(string artist);
        // Task<Music?> GetMusicByIdAsync(string id);
        Task<MusicArtistDTO> GetMusicArtistByIdAsync(string id);
        Task<bool> DeleteMusicAsync(string id);
        Task<List<MusicArtistDTO>> Get10RandomMusicWithArtistAsync();
        Task<List<MusicArtistDTO>> Get5RandomMusicByArtistIdWithArtistAsync(string artistId);
    }

}
