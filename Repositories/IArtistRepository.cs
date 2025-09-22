using WebProject.Entities;

namespace WebProject.Repositories
{
    public interface IArtistRepository
    {
        Task CreateArtistAsync(Artist artist);
        Task<Artist> GetArtistByIdAsync(string id);
        Task<List<Artist>> GetAllArtistsByNameAsync(string name);
        Task<bool> UpdateArtistAsync(Artist artist);
        Task<bool> DeleteArtistAsync(string id);
        Task<List<string>> Get5RandomArtistsIdAsync();
        Task<List<Artist>> Get5RandomArtistsAsync();
    }
}
