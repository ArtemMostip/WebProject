using MongoDB.Bson.Serialization;
using WebProject.DTO_s;
using WebProject.Entities;
using WebProject.Repositories;

namespace WebProject.Services
{
    public class MusicService : IMusicService
    {
        private readonly IMusicRepository _musicRepository;
        private readonly IArtistService _artistService;
        private readonly IAlbumService _albumService;
        private readonly IAlbumRepository _albumRepository;
        private readonly IPlaylistRepository _playlistRepository;
        public MusicService(IMusicRepository musicRepository, IArtistService artistService, IAlbumService albumService, IAlbumRepository albumRepository, IPlaylistRepository playlistRepository)
        {
            _musicRepository = musicRepository;
            _artistService = artistService;
           _albumService = albumService;
            _albumRepository = albumRepository;
            _playlistRepository = playlistRepository;
        }



        //var album = string.IsNullOrWhiteSpace(musicInpDTO.AlbumId) || musicInpDTO.AlbumId == "string" ? null : musicInpDTO.AlbumId;
        //if (musicInpDTO.Year <= 0 || musicInpDTO.Year > DateTime.Now.Year)
        //{
        //    throw new Exception("Music year is invalid.");
        //}
        //var music = new Music
        //{
        //    Title = musicInpDTO.Title,
        //    ArtistId = musicInpDTO.ArtistId,
        //    AlbumId = album,
        //    DateCreation = DateTime.Now,
        //    Year = musicInpDTO.Year,
        //    Genre = musicInpDTO.Genre,
        //    CoverUrl = musicInpDTO.CoverUrl,
        //    FileUrl = musicInpDTO.FileUrl

        //};

        ////GetAlbumByIdAsync
        //await _musicRepository.CreateMusicAsync(music);

        //if (!string.IsNullOrWhiteSpace(albumId))
        //{
        //    var album = await _albumRepository.GetAlbumByIdAsync(albumId);
        //    if (album != null)
        //    {
        //        album.CountOfMusicInAlbum = (album.CountOfMusicInAlbum ?? 0) + 1;
        //        await _albumRepository.UpdateAlbumAsync(album);
        //    }
        //}
        //return true;


        public async Task<List<MusicArtistDTO>> Get10RandomMusicWithArtistAsync()
        {
            var musics = await _musicRepository.Get10RandomMusicsAsync();
            var result = new List<MusicArtistDTO>();

            foreach (var music in musics)
            {
                Artist? artist = null;
                MusicAlbumArtistDTO? albumDTO = null;

                if (!string.IsNullOrEmpty(music.ArtistId))
                {
                    artist = await _artistService.GetArtist(music.ArtistId);
                }

                if (!string.IsNullOrEmpty(music.AlbumId))
                {
                    var album = await _albumService.GetAlbumAsync1(music.AlbumId);
                    Artist? albumArtist = null;

                    if (!string.IsNullOrEmpty(album.ArtistId))
                    {
                        albumArtist = await _artistService.GetArtist(album.ArtistId);
                    }

                    albumDTO = new MusicAlbumArtistDTO
                    {
                        Id = album.Id,
                        Title = album.Title,
                        CountOfMusicInAlbum = album.CountOfMusicInAlbum,
                        AlbCoverUrl = album.AlbCoverUrl,
                        Artist = albumArtist
                    };
                }

                result.Add(new MusicArtistDTO
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
                }); 
            }

            return result;
        }
        public async Task<List<MusicArtistDTO>> Get5RandomMusicByArtistIdWithArtistAsync(string artistId)
        {
            var musics = await _musicRepository.Get5RandomMusicByArtistIdAsync(artistId);
            var result = new List<MusicArtistDTO>();

            foreach (var music in musics)
            {
                Artist? artist = null;
                MusicAlbumArtistDTO? albumDTO = null;

                // Отримуємо об'єкт артиста
                if (!string.IsNullOrEmpty(music.ArtistId))
                {
                    artist = await _artistService.GetArtist(music.ArtistId);
                }

                // Отримуємо об'єкт альбому і артиста для альбому
                if (!string.IsNullOrEmpty(music.AlbumId))
                {
                    var album = await _albumService.GetAlbumAsync1(music.AlbumId);
                    Artist? albumArtist = null;

                    if (!string.IsNullOrEmpty(album.ArtistId))
                    {
                        albumArtist = await _artistService.GetArtist(album.ArtistId);
                    }

                    albumDTO = new MusicAlbumArtistDTO
                    {
                        Id = album.Id,
                        Title = album.Title,
                        CountOfMusicInAlbum = album.CountOfMusicInAlbum,
                        AlbCoverUrl = album.AlbCoverUrl,
                        Artist = albumArtist
                    };
                }

                // Додаємо DTO в результат
                result.Add(new MusicArtistDTO
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
                });
            }

            return result;
        }

        //Старий код: 
        /*public async Task<List<MusicArtistDTO>> GetMusicsArtistByTitle(string title)
 {
     var musics = await _musicRepository.GetAllMusicByTitleAsync(title);
     var musicOutList = new List<MusicArtistDTO>();

     foreach (var music in musics)
     {
         Artist? artist = null;
         Album? album = null;

         if (!string.IsNullOrEmpty(music.ArtistId))
         {
             artist = await _artistService.GetArtist(music.ArtistId);
         }

         if (!string.IsNullOrEmpty(music.AlbumId))
         {
             album = await _albumService.GetAlbumAsync1(music.AlbumId);
         }

         musicOutList.Add(new MusicArtistDTO
         {
             Id = music.Id,
             Title = music.Title,
             Artist = artist,
             Album = album,
             Year = music.Year,
             Genre = music.Genre,
             CoverUrl = music.CoverUrl,
             FileUrl = music.FileUrl,
             DateCreation = music.DateCreation
         });
     }

     return musicOutList;
 }*/
        public async Task<MusicArtistDTO> GetMusicArtistByIdAsync(string id)
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
                var album = await _albumService.GetAlbumAsync1(music.AlbumId);
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


        public async Task<List<MusicArtistDTO>> GetMusicsArtistByTitle(string title)
        {
            var musics = await _musicRepository.GetAllMusicByTitleAsync(title);
            var musicOutList = new List<MusicArtistDTO>();

            foreach (var music in musics)
            {
                Artist? artist = null;
                MusicAlbumArtistDTO? albumDTO = null;

                if (!string.IsNullOrEmpty(music.ArtistId))
                {
                    artist = await _artistService.GetArtist(music.ArtistId);
                }

                if (!string.IsNullOrEmpty(music.AlbumId))
                {
                    var album = await _albumService.GetAlbumAsync1(music.AlbumId);
                    //Artist? albumArtist = null;

                    //if (!string.IsNullOrEmpty(album.ArtistId))
                    //{
                    //    albumArtist = await _artistService.GetArtist(album.ArtistId);
                    //}

                    albumDTO = new MusicAlbumArtistDTO
                    {
                        Id = album.Id,
                        Title = album.Title,
                        Artist = artist,
                        AlbCoverUrl = album.AlbCoverUrl,
                        CountOfMusicInAlbum = album.CountOfMusicInAlbum
                      

                    };
                }

                musicOutList.Add(new MusicArtistDTO
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
                });
            }

            return musicOutList;
        }
        public async Task<bool> CreateMusicAsync(MusicInpDTO musicInpDTO)
        {
            var albumId = string.IsNullOrWhiteSpace(musicInpDTO.AlbumId) || musicInpDTO.AlbumId == "string" ? null : musicInpDTO.AlbumId;

            if (musicInpDTO.Year <= 0 || musicInpDTO.Year > DateTime.Now.Year)
            {
                throw new Exception("Music year is invalid.");
            }

            var music = new Music
            {
                Title = musicInpDTO.Title,
                ArtistId = musicInpDTO.ArtistId,
                AlbumId = albumId,
                DateCreation = DateTime.Now,
                Year = musicInpDTO.Year,
                Genre = musicInpDTO.Genre,
                CoverUrl = musicInpDTO.CoverUrl,
                FileUrl = musicInpDTO.FileUrl
            };

            await _musicRepository.CreateMusicAsync(music);
            await _albumService.InsertMusicIntoAlbumAsync(albumId, music.Id);
            if (!string.IsNullOrWhiteSpace(albumId))
            {
                var album = await _albumRepository.GetAlbumByIdAsync(albumId);
                if (album != null)
                {
                    album.CountOfMusicInAlbum = (album.CountOfMusicInAlbum ?? 0) + 1;
                    await _albumRepository.UpdateAlbumAsync(album);
                }
            }

            return true;
        }
        public async Task<List<Music>> GetMusicsByArtist(string artist)
        {
            var musics = await _musicRepository.GetAllMusicByArtistAsync(artist);
            return musics;
        }
       
        public async Task<List<Music>> GetMusicsByTitle(string title)
        {
            return await _musicRepository.GetAllMusicByTitleAsync(title);
        }
        public async Task<bool> DeleteMusicAsync(string id)
        {
            await _playlistRepository.DeleteMusicFromPlaylistsAsync(id);
            //var DeleteMusicFromAlbum = await _musicRepository.GetMusicByIdAsync(id);
            var music = await _musicRepository.GetMusicByIdAsync(id);
            if (music == null)
                return false;
           

            var album = await _albumRepository.GetAlbumByIdAsync(music.AlbumId);
            if (album != null)
            {
                album.CountOfMusicInAlbum = (album.CountOfMusicInAlbum ?? 0) - 1;
                await _albumRepository.UpdateAlbumAsync(album);
            }

            var result = await _musicRepository.DeleteMusicAsync(id);
          
            return result;


        }

    }
}
