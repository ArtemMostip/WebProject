using WebProject.DTO_s;
using WebProject.Entities;

namespace WebProject.Services
{
    public interface IIMusicService
    {
        Task<Music?> GetMusicByIdAsync(string id);
        Task<MusicArtistDTO> GetFullMusicArtistByIdAsync(string id);
    }
}
