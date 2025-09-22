using WebProject.DTO_s;
using WebProject.Entities;
using WebProject.Repositories;
using Microsoft.Extensions.DependencyInjection;
namespace WebProject.Services
{
    public class SecondMusicService : IIMusicService

    {
        private readonly IMusicRepository _musicRepository;
        private readonly IArtistService _artistService;
        private readonly Lazy<IAlbumService> _albumService;

        public SecondMusicService(IMusicRepository musicRepository, IArtistService artistService, Lazy<IAlbumService> albumService)
        {
            _musicRepository = musicRepository;
            _artistService = artistService;
            _albumService = albumService;
        }

        public async Task<Music?> GetMusicByIdAsync(string id)
        {
            return await _musicRepository.GetMusicByIdAsync(id);
        }
        public async Task<MusicArtistDTO> GetFullMusicArtistByIdAsync(string id)
        {
            var music = await _musicRepository.GetMusicByIdAsync(id);

            if (music == null)
                return null;

            Artist? artist = null;
            MusicAlbumArtistDTO? albumDTO = null;

            if (!string.IsNullOrEmpty(music.ArtistId))
            {
                artist = await _artistService.GetArtist(music.ArtistId);
            }

            if (!string.IsNullOrEmpty(music.AlbumId))
            {
                var album = await _albumService.Value.GetAlbumAsync1(music.AlbumId);
                Artist? albumArtist = null;

                if (!string.IsNullOrEmpty(album.ArtistId))
                {
                    albumArtist = await _artistService.GetArtist(album.ArtistId);
                }

                albumDTO = new MusicAlbumArtistDTO
                {
                    Id = album.Id,
                    Title = album.Title,

                    Artist = albumArtist,
                    AlbCoverUrl = album.AlbCoverUrl,
                    CountOfMusicInAlbum = album.CountOfMusicInAlbum,
                };
            }

            return new MusicArtistDTO
            {
                Id = music.Id,
                Title = music.Title,
                Artist = artist,
                Album = albumDTO,
                Year = music.Year,
                Genre = music.Genre,
                CoverUrl = music.CoverUrl,
                FileUrl = music.FileUrl,
                DateCreation = music.DateCreation
            };


        }
    }
}
