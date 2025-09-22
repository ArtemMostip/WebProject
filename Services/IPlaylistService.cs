using WebProject.DTO_s;
using WebProject.Entities;

namespace WebProject.Services
{
    public interface IPlaylistService
    {
        Task<bool> CreatePlaylistAsync(PlaylistDTO playlistDTO);
        Task<PlaylistWithMusicsAndAccountsDTO?> GetPlaylistWithMusicsAsync(string id);
        Task<bool> DeletePlaylistAsync(string id);
    }
}
