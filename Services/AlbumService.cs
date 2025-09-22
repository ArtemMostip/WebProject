using MongoDB.Driver;
using WebProject.DTO_s;
using WebProject.Entities;
using WebProject.Repositories;

namespace WebProject.Services
{
    public class AlbumService : IAlbumService
    {
        private readonly IAlbumRepository _albumRepository;
        private readonly IArtistService _artistService;
        private readonly IIMusicService _musicService;
        public AlbumService(IAlbumRepository albumRepository, IArtistService artistService, IIMusicService musicService)
        {
            _albumRepository = albumRepository;
            _artistService = artistService;
            _musicService = musicService;

        }



        public async Task<List<AlbumWithTracksDTO>> Get10RandomAlbumsWithArtistAsync()
        {
            var albums = await _albumRepository.Get10RandomAlbumsAsync();
            var result = new List<AlbumWithTracksDTO>();

            foreach (var album in albums)
            {
                Artist? artist = null;

                if (!string.IsNullOrEmpty(album.ArtistId))
                {
                    artist = await _artistService.GetArtist(album.ArtistId);
                }

                var albumDTO = new AlbumWithTracksDTO
                {
                    Id = album.Id,
                    Title = album.Title,
                    AlbCoverUrl = album.AlbCoverUrl,
                    CountOfMusicInAlbum = album.CountOfMusicInAlbum,
                    MusicInAlbum = album.MusicInAlbum,
                    Artist = artist
                };

                result.Add(albumDTO);
            }

            return result;
        }


        public async Task<bool> CreateAlbumAsync(AlbumDTO albumDTO)
        {

            var album = new Album
            {
                Title = albumDTO.Title,
                ArtistId = albumDTO.ArtistId,
                MusicInAlbum = albumDTO.MusicInAlbum,
                AlbCoverUrl = albumDTO.AlbCoverUrl,
                CountOfMusicInAlbum = albumDTO.CountOfMusicInAlbum
            };
            await _albumRepository.CreateAlbumAsync(album);
            return true;
        }



        public async Task<AlbumArtistDTO> GetAlbumAsync(string id)
        {

            var album = await _albumRepository.GetAlbumByIdAsync(id);
            if (album == null)
            {
                return null;
            }

            List<MusicArtistDTO> musics = new List<MusicArtistDTO>();
            if (album.MusicInAlbum != null && album.MusicInAlbum.Any())
            {
                foreach (var musicId in album.MusicInAlbum)
                {
                    var music = await _musicService.GetFullMusicArtistByIdAsync(musicId);
                    if (music != null)
                    {
                        musics.Add(music);
                    }
                }
            }



            Artist? artist = null;
            if (!string.IsNullOrEmpty(album.ArtistId))
            {
                artist = await _artistService.GetArtist(album.ArtistId);
            }

            var albumArtistDTO = new AlbumArtistDTO
            {
                Title = album.Title,
                Artist = artist,
                CountOfMusicInAlbum = album.CountOfMusicInAlbum,
                AlbCoverUrl = album.AlbCoverUrl,
                MusicInAlbum = musics
            };

            return albumArtistDTO;
        }
        public async Task<Album> GetAlbumAsync1(string id)
        {
            return await _albumRepository.GetAlbumByIdAsync(id);
        }
        public async Task InsertMusicIntoAlbumAsync(string albumId, string musicId)
        {
            await _albumRepository.AddMusicToAlbumAsync(albumId, musicId);
        }

    }
}
