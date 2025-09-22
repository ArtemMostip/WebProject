using WebProject.DTO_s;
using WebProject.Entities;
using WebProject.Repositories;

namespace WebProject.Services
{
    public class ArtistService : IArtistService
    {
        private readonly IArtistRepository _artistRepository;
        private readonly IAlbumRepository _albumRepository;
        private readonly IMusicRepository _musicRepository;
        private readonly IPlaylistRepository _playlistRepository;
        private readonly Lazy<IAlbumService> _albumService;
        public ArtistService(IArtistRepository artistRepository, IAlbumRepository albumRepository, IMusicRepository musicRepository, IPlaylistRepository playlistRepository, Lazy<IAlbumService> albumService)
        {
            _artistRepository = artistRepository;
            _albumRepository = albumRepository;
            _musicRepository = musicRepository;
            _playlistRepository = playlistRepository;
            _albumService = albumService;
        }
        public async Task<bool> CreateArtistAsync(ArtistDTO artistDTO)
        {

            var artist = new Artist
            {
                Name = artistDTO.Name,
                AvatarUrl = artistDTO.AvatarUrl,
                  ArtistBannerUrl = artistDTO.ArtistBannerUrl,
                  ArtistLogo = artistDTO.ArtistLogo
            };
            await _artistRepository.CreateArtistAsync(artist);
            return true;
        }


        public async Task<Artist> GetArtistByIdAsync(string id)
        {
            return await _artistRepository.GetArtistByIdAsync(id);
        }
        public async Task<Artist> GetArtist(string id)
        {
            var artists = await _artistRepository.GetArtistByIdAsync(id);
            return artists;
        }
        public async Task<bool> DeleteMusicByArtistIdFromPlaylistAsync(string id)
        {
            await _playlistRepository.DeleteMusicByArtistIdFromPlaylistAsync(id);

            var deleteResult = await _artistRepository.DeleteArtistAsync(id);

            return deleteResult;
        }


        public async Task<bool> DeleteArtistAsync(string id)
        {
            await _albumRepository.DeleteAlbumsByArtistIdAsync(id);

            await _playlistRepository.DeleteMusicByArtistIdFromPlaylistAsync(id);

            await _musicRepository.DeleteMusicByArtistIdAsync(id);

          
            var deleteResult = await _artistRepository.DeleteArtistAsync(id);

            return deleteResult;
        }

        public async Task<ArtistWithAlbumsDTO?> OutputAllAlbumsByArtistId(string id)
        {
            var artist = await GetArtistByIdAsync(id);
            if (artist == null)
            {
                return null;
            }

            var artistAlbums = await _albumRepository.GetAlbumsByArtistIdAsync(id);

            var albums = artistAlbums.Where(a => a != null).ToList();

            return new ArtistWithAlbumsDTO
            {
                Id = artist.Id!,
                Name = artist.Name,
                AvatarUrl = artist.AvatarUrl,
                ArtistBannerUrl = artist.ArtistBannerUrl,
                ArtistLogo = artist.ArtistLogo,
                Albums = albums
            };
        }


    }
}
