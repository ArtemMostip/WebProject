using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Linq.Expressions;
using WebProject.DTO_s;
using WebProject.Entities;
using WebProject.Repositories;
using WebProject.Services;

namespace WebProject.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ArtistController : ControllerBase
    {
        private readonly IArtistService _artistService;
        private readonly IArtistRepository _artistRepository;
        private readonly IMusicRepository _musicRepository;
        private readonly IMusicService _musicService;
        public ArtistController(IArtistService artistService, IArtistRepository artistRepository,IMusicRepository musicRepository, IMusicService musicService)
        {
            _artistService = artistService;
            _artistRepository = artistRepository;
            _musicRepository = musicRepository;
            _musicService = musicService;
        }

        [HttpPost("Create")]
        public async Task<IActionResult> CreatePlaylist([FromBody] ArtistDTO artistDTO)
        {
            if (artistDTO == null)
            {
                return BadRequest(new { Message = "Artist data is null" });
            }

            try
            {
                await _artistService.CreateArtistAsync(artistDTO);
                return Ok("Artist created successfully.");
            }
            catch (Exception ex)
            {
                return BadRequest(new { Message = ex.Message });
            }
        }
        [HttpGet("artist/{id}")]
        public async Task<IActionResult> GetAlbumsByArtistId(string id)
        {
            try
            {
                var artist = await _artistService.OutputAllAlbumsByArtistId(id);

                if (artist == null || string.IsNullOrWhiteSpace(artist.Id))
                {
                    return NotFound(new { Message = "Artist or albums not found." });
                }

                return Ok(artist);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Message = "An error occurred while processing your request.", Details = ex.Message });
            }
        }
        [HttpGet("avatar/{id}")]
        public async Task<IActionResult> GetArtistAvatarById(string id)
        {
            try
            {
                var artist = await _artistService.GetArtistByIdAsync(id);
                if (artist == null || string.IsNullOrWhiteSpace(artist.AvatarUrl))
                {
                    return NotFound(new { Message = "Music not found or cover image URL is missing." });
                }


                if (artist.AvatarUrl.StartsWith("https", StringComparison.OrdinalIgnoreCase))
                {
                    using (var httpClient = new HttpClient())
                    {
                        var response = await httpClient.GetAsync(artist.AvatarUrl);

                        if (response.IsSuccessStatusCode)
                        {
                            var imageBytes = await response.Content.ReadAsByteArrayAsync();
                            var contentType = response.Content.Headers.ContentType?.ToString() ?? "image/jpeg/gif";
                            return File(imageBytes, contentType);
                        }
                        else
                        {
                            return BadRequest(new { Message = $"Failed to fetch external image: {response.StatusCode}" });
                        }
                    }
                }


                string imagePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "MUSICAPI","UIA", artist.AvatarUrl);

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

        [HttpGet("banner/{id}")]
        public async Task<IActionResult> GetArtistBannerById(string id)
        {
            try
            {
                var artist = await _artistService.GetArtistByIdAsync(id);
                if (artist == null || string.IsNullOrWhiteSpace(artist.ArtistBannerUrl))
                {
                    return NotFound(new { Message = "Music not found or Banner URL is missing." });
                }

                if (artist.ArtistBannerUrl.StartsWith("https", StringComparison.OrdinalIgnoreCase))
                {
                    using (var httpClient = new HttpClient())
                    {
                        var response = await httpClient.GetAsync(artist.ArtistBannerUrl);

                        if (response.IsSuccessStatusCode)
                        {
                            var imageBytes = await response.Content.ReadAsByteArrayAsync();
                            var contentType = response.Content.Headers.ContentType?.ToString() ?? "image/jpeg"; // Default if not specified
                            return File(imageBytes, contentType);
                        }
                        else
                        {
                            return BadRequest(new { Message = $"Failed to fetch external image: {response.StatusCode}" });
                        }
                    }
                }
                else
                {
                    string imagePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "MUSICAPI", "UIA", artist.ArtistBannerUrl);

                    if (!System.IO.File.Exists(imagePath))
                    {
                        return NotFound(new { Message = "Banner file not found on server." });
                    }

                    var mimeType = "image/jpeg"; // You might need to determine this dynamically
                    return PhysicalFile(imagePath, mimeType);
                }
            }
            catch (Exception ex)
            {
                return BadRequest(new { Message = ex.Message });
            }
        }
        [HttpGet("logo/{id}")]
        public async Task<IActionResult> GetArtistLogoById(string id)
        {
            try
            {
                var artist = await _artistService.GetArtistByIdAsync(id);
                if (artist == null || string.IsNullOrWhiteSpace(artist.ArtistLogo))
                {
                    return NotFound(new { Message = "Logo not found or Logo URL is missing." });
                }

                if (artist.ArtistLogo.StartsWith("https", StringComparison.OrdinalIgnoreCase))
                {
                    using (var httpClient = new HttpClient())
                    {
                        var response = await httpClient.GetAsync(artist.ArtistLogo);

                        if (response.IsSuccessStatusCode)
                        {
                            var imageBytes = await response.Content.ReadAsByteArrayAsync();
                            var contentType = response.Content.Headers.ContentType?.ToString() ?? "image/jpeg"; // Default if not specified
                            return File(imageBytes, contentType);
                        }
                        else
                        {
                            return BadRequest(new { Message = $"Failed to fetch external image: {response.StatusCode}" });
                        }
                    }
                }
                else
                {
                    string imagePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "MUSICAPI", "UIA", artist.ArtistLogo);

                    if (!System.IO.File.Exists(imagePath))
                    {
                        return NotFound(new { Message = "Logo file not found on server." });
                    }

                    var mimeType = "image/jpeg"; // You might need to determine this dynamically
                    return PhysicalFile(imagePath, mimeType);
                }
            }
            catch (Exception ex)
            {
                return BadRequest(new { Message = ex.Message });
            }
        }
        [HttpGet("5randomMusic/ArtistId/{id}")]
        public async Task<IActionResult> GetRandomMusicByArtistId(string id)
        {
            try
            {
                var musicList = await _musicService.Get5RandomMusicByArtistIdWithArtistAsync(id);

                if (musicList == null || !musicList.Any())
                {
                    return NotFound(new { Message = "No music found for this artist." });
                }

                return Ok(musicList);
            }
            catch (Exception ex)
            {
                // За потреби логувати ex
                return StatusCode(500, new { Message = "An error occurred while fetching random music.", Details = ex.Message });
            }

        }
            [HttpGet("5random-artists")]
        public async Task<IActionResult> GetRandomMusic()
        {
            var artists = await _artistRepository.Get5RandomArtistsAsync();

            if (artists == null || !artists.Any())
            {
                Console.WriteLine("Artist_1");
                return NotFound(new { Message = "No artist found." });
            }
            Console.WriteLine("Artist_2");
            return Ok(artists);
        }
        [HttpGet("5random-artistID")]
        public async Task<IActionResult> GetRandomMusicsID()
        {
            var artists = await _artistRepository.Get5RandomArtistsIdAsync();

            if (artists == null || !artists.Any())
            {
                return NotFound(new { Message = "No artist found." });
            }

            return Ok(artists);
        }

        [HttpDelete("by-Artist{id}")]
        public async Task<IActionResult> DeleteArtistAsync(string id)
        {
            var DeletedDocument = await _artistService.DeleteArtistAsync(id);

            if (!DeletedDocument)
            {
                return NotFound(new { Message = $"Artist with ID '{id}' not found." });
            }

            return Ok(new { Message = $"Artist with ID '{id}' was successfully deleted." });
        }

        ////[Authorize(Roles = "admin")]
        //[HttpDelete("by-Playlist/{id}")]
        //public async Task<IActionResult> DeleteMusicByArtistIdFromPlaylistAsync(string id)
        //{
        //    var DeletedRow = await _artistService.DeleteMusicByArtistIdFromPlaylistAsync(id);

        //    if (!DeletedRow)
        //    {
        //        return NotFound(new { Message = $"Artist with ID '{id}' not found." });
        //    }

        //    return Ok(new { Message = $"Artist with ID '{id}' was successfully deleted." });
        //}



    }
}
