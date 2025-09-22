using WebProject.DTO_s;
using WebProject.Entities;
using WebProject.Repositories;

namespace WebProject.Services
{
    public class PlaylistService : IPlaylistService
    {
        private readonly IPlaylistRepository _playlistRepository;
        private readonly IIMusicService _musicService;
        private readonly IAccountService _accountService;
        public PlaylistService(IPlaylistRepository playlistRepository, IIMusicService musicService, IAccountService accountService)
        {
            _playlistRepository = playlistRepository;
            _musicService = musicService;
            _accountService = accountService;
        }
        public async Task<bool> CreatePlaylistAsync(PlaylistDTO playlistDTO)
        {

            var playlist = new Playlist
            {
                AccountId = playlistDTO.AccountId,
                Name = playlistDTO.Name,
                MusicId = playlistDTO.MusicId

            };
            await _playlistRepository.CreatePlaylistAsync(playlist);
            return true;
        }
        public async Task<Playlist> GetPlaylistByIdAsync(string id)
        {
            return await _playlistRepository.GetPlaylistByIdAsync(id);
        }

        public async Task<PlaylistWithMusicsAndAccountsDTO?> GetPlaylistWithMusicsAsync(string id)
        {
            var playlist = await _playlistRepository.GetPlaylistByIdAsync(id);

            if (playlist == null)
            {
                return null;
            }

            List<Music> musics = new List<Music>();
            if (playlist.MusicId != null && playlist.MusicId.Any())
            {
                foreach (var musicId in playlist.MusicId)
                {
                    var music = await _musicService.GetMusicByIdAsync(musicId.ToString());
                    if (music != null)
                    {
                        musics.Add(music);
                    }
                }
            }

            var account = await _accountService.GetAccount(playlist.AccountId);
           // var music = await _musicService.GetMusicByIdAsync(playlist.Id);
            AccountInfoDTO? accountInfoDTO = null;
            if (account != null)
            {
                accountInfoDTO = new AccountInfoDTO
                {

                    Name = account.Name,
                    Email = account.Email
                };
            }



            var playlistWithMusicsAndAccountsDTO = new PlaylistWithMusicsAndAccountsDTO
            {
                Id = playlist.Id,
                AccountId = playlist.AccountId,
                Account = accountInfoDTO,
                Name = playlist.Name,
                Musics = musics
            };

            return playlistWithMusicsAndAccountsDTO;
        }
        public async Task<bool> DeletePlaylistAsync(string id)
        {
            return await _playlistRepository.DeletePlaylistAsync(id);
        }

    }
}
