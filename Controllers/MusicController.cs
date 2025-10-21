using LazyCache;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using WebProject.Caching;
using WebProject.DTO_s;
using WebProject.Entities;
using WebProject.Repositories;
using WebProject.Services;

namespace WebProject.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class MusicController : ControllerBase
    {

        //musicalService - основний 
        //musicSerivce - допоміжний для розриву циклу залежностей між AlbumService і MusicService
        IIMusicService _musicService;
        IMusicService _musicalService;
        IMusicRepository _musicRepository;
        ICacheProvider _cacheProvider;
        public MusicController(IIMusicService musicService, IMusicRepository musicRepository, IMusicService musicalService, ICacheProvider cacheProvider)
        {
            _musicService = musicService;
            _musicRepository = musicRepository;
            _musicalService = musicalService;
            _cacheProvider = cacheProvider;
        }

 

        [HttpGet("by-id/{id}")]
        public async Task<IActionResult> GetMusicsById(string id)
        {
            try
            {
                var music = await _musicalService.GetMusicArtistByIdAsync(id);
                await _musicRepository.IncrementListeningCountAsync(id);
                return Ok(music);
            }
            catch (Exception ex)
            {
                return BadRequest(new { Message = ex.Message });
            }

        }

        [HttpGet("by-title/{title}")]
        public async Task<IActionResult> GetMusictsByTitle(string title)
        {
            try
            {
                List<MusicArtistDTO> musicList = await _musicalService.GetMusicsArtistByTitle(title);
               // await _musicRepository.IncrementListeningCountAsync(title);
                return Ok(musicList);
            }
            catch (Exception ex)
            {
                return BadRequest(new { Message = ex.Message });
            }

        }
        // Код для зміни зображення динамічно:
        /*public async Task<IActionResult> GetMusicCoverById(string id, [FromQuery] long? version = null)
{
    try
    {
        var music = await _musicService.GetMusicByIdAsync(id);
        if (music == null || string.IsNullOrWhiteSpace(music.CoverUrl))
        {
            return NotFound(new { Message = "Music not found or cover image URL is missing." });
        }

        if (music.CoverUrl.StartsWith("http", StringComparison.OrdinalIgnoreCase))
        {
            return Redirect($"{music.CoverUrl}?v={version ?? DateTime.UtcNow.Ticks}");
        }

        var imagePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "MUSICAPI", "Covers", music.CoverUrl);

        if (!System.IO.File.Exists(imagePath))
        {
            return NotFound(new { Message = "Cover image file not found on server." });
        }

        // Забороняємо кешування
        Response.Headers["Cache-Control"] = "no-cache, no-store, must-revalidate";
        Response.Headers["Pragma"] = "no-cache";
        Response.Headers["Expires"] = "0";

        var extension = Path.GetExtension(imagePath).ToLower();
        var mimeType = extension switch
        {
            ".png" => "image/png",
            ".jpg" or ".jpeg" => "image/jpeg",
            _ => "application/octet-stream"
        };

        return PhysicalFile(imagePath, mimeType);
    }
    catch (Exception ex)
    {
        return BadRequest(new { Message = ex.Message });
    }
}*/
        
        [HttpGet("cover/{id}")]
        public async Task<IActionResult> GetMusicCoverById(string id)
        {
            try
            {
                var music = await _musicService.GetMusicByIdAsync(id);
                if (music == null || string.IsNullOrWhiteSpace(music.CoverUrl))
                {
                    return NotFound(new { Message = "Music not found or cover image URL is missing." });
                }


                if (music.CoverUrl.StartsWith("https", StringComparison.OrdinalIgnoreCase))
                {
                    using (var httpClient = new HttpClient())
                    {
                        var response = await httpClient.GetAsync(music.CoverUrl);

                        if (response.IsSuccessStatusCode)
                        {
                            var imageBytes = await response.Content.ReadAsByteArrayAsync();
                            var contentType = response.Content.Headers.ContentType?.ToString() ?? "image/jpeg";
                            return File(imageBytes, contentType);
                        }
                        else
                        {
                            return BadRequest(new { Message = $"Failed to fetch external image: {response.StatusCode}" });
                        }
                    }
                }


                string imagePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "MUSICAPI", "Covers", music.CoverUrl);

                if (!System.IO.File.Exists(imagePath))
                {
                    return NotFound(new { Message = "Cover image file not found on server." });
                }

                var mimeType = "image/jpeg"; 
                return PhysicalFile(imagePath, mimeType);
            }
            catch (Exception ex)
            {
                return BadRequest(new { Message = ex.Message });
            }
        }

        [HttpGet("file/{id}")]
        public async Task<IActionResult> GetMusicFileById(string id)
        {
            try
            {
                var music = await _musicService.GetMusicByIdAsync(id);
                if (music == null || string.IsNullOrWhiteSpace(music.FileUrl))
                {
                    return NotFound(new { Message = "Music not found or file URL is missing." });
                }
              

                var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "MUSICAPI","Musics", music.FileUrl);

                if (!System.IO.File.Exists(filePath))
                {
                    return NotFound(new { Message = "Music file not found on server." });
                }

                var mimeType = "application/octet-stream"; 
                return PhysicalFile(filePath, mimeType, enableRangeProcessing: true);
            }
            catch (Exception ex)
            {
                return BadRequest(new { Message = ex.Message });
            }
        }

        [HttpGet("Sorted_music")] 
        public async Task<IActionResult> SortMusicByListenings()
        {
            try
            {
                // Отримати топ 10 пісень за кількістю прослуховувань
                var topMusic = await _musicRepository.GetTopListenedMusicAsync(10);

                return Ok(topMusic);
            }
            catch (Exception ex)
            {
                // Логування помилки (опціонально)
                return StatusCode(500, $"Помилка при отриманні музики: {ex.Message}");
            }
        }

     



        [HttpGet("10random-music")]
        public async Task<IActionResult> GetRandomMusics()
        {
           
            if(!_cacheProvider.TryGetValue(CacheKeys.RandomMusicKey,out List<MusicArtistDTO> musics ))
            {
                musics = await _musicalService.Get10RandomMusicWithArtistAsync();
                var cacheEntryOption = new MemoryCacheEntryOptions 
                { 
                AbsoluteExpiration = DateTime.Now.AddSeconds(30),
                SlidingExpiration = TimeSpan.FromSeconds(30),
           
                };
                _cacheProvider.Set(CacheKeys.RandomMusicKey, musics, cacheEntryOption);
            }
            return Ok(musics);
            //var musics = await _musicalService.Get10RandomMusicWithArtistAsync();

            //if (musics == null || !musics.Any())
            //{
            //    return NotFound(new { Message = "No music found." });
            //}

            //return Ok(musics);
        }


        [HttpGet("10random-musicID")]
        public async Task<IActionResult> GetRandomMusicsID()
        {
            var musics = await _musicRepository.Get10RandomMusicsIdAsync();

            if (musics == null || !musics.Any())
            {
                return NotFound(new { Message = "No music found." });
            }

            return Ok(musics);
        }


        [HttpGet("by-artist/{artist}")]
        public async Task<IActionResult> GetMusicsByArtist(string artist)
        {
            try
            {
                List<Music> musicList = await _musicalService.GetMusicsByArtist(artist);
                return Ok(musicList);
            }
            catch (Exception ex)
            {
                return BadRequest(new { Message = ex.Message });
            }
        }

        [HttpPost("Create")]
        public async Task<IActionResult> CreateMusic([FromBody] MusicInpDTO musicDTO)
        {
            if (musicDTO == null)
            {
                return BadRequest(new { Message = "Music data is null" });
            }

            try
            {
                await _musicalService.CreateMusicAsync(musicDTO);

              

                return Ok("Music created successfully.");
            }
            catch (Exception ex)
            {
                return BadRequest(new { Message = ex.Message });
            }
        }
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteMusicAsync(string id)
        {
            var DeletedDocument = await _musicalService.DeleteMusicAsync(id);

            if (!DeletedDocument)
            {
                return NotFound(new { Message = $"Music with ID '{id}' not found." });
            }

            return Ok(new { Message = $"Music with ID '{id}' was successfully deleted." });
        }
    }
}
