using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using WebProject.DTO_s;
using WebProject.Entities;
using WebProject.Repositories;
using WebProject.Services;

namespace WebProject.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AlbumController : ControllerBase
    {
        private readonly IAlbumService _albumService;
        private readonly IAlbumRepository _albumRepository;
        public AlbumController(IAlbumService albumService, IAlbumRepository albumRepository)
        {
            _albumService = albumService;
            _albumRepository = albumRepository;
        }

        [HttpPost("Create")]
        public async Task<IActionResult> CreateAlbum([FromBody] AlbumDTO albumDTO)
        {
            if (albumDTO == null)
            {
                return BadRequest(new { Message = "Album data is null" });
            }

            try
            {
                await _albumService.CreateAlbumAsync(albumDTO);
                return Ok("Album created successfully.");
            }
            catch (Exception ex)
            {
                return BadRequest(new { Message = ex.Message });
            }
        }
        [HttpGet("{id}")]
        public async Task<IActionResult> GetPlaylist(string id)
        {
            try
            {
                AlbumArtistDTO album = await _albumService.GetAlbumAsync(id);
                return Ok(album);
            }
            catch (Exception ex)
            {
                return BadRequest(new { Message = ex.Message });
            }
        }
        [HttpGet("AlbumCover/{id}")]
        public async Task<IActionResult> GetAlbumCoverById(string id)
        {
            try
            {
                var album = await _albumService.GetAlbumAsync1(id);
                if (album == null || string.IsNullOrWhiteSpace(album.AlbCoverUrl))
                {
                    return NotFound(new { Message = "Album not found or cover image URL is missing." });
                }


                if (album.AlbCoverUrl.StartsWith("https", StringComparison.OrdinalIgnoreCase))
                {
                    using (var httpClient = new HttpClient())
                    {
                        var response = await httpClient.GetAsync(album.AlbCoverUrl);

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

                string imagePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "MUSICAPI", "Covers", album.AlbCoverUrl);

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


        [HttpGet("10random-album")]
        public async Task<IActionResult> GetRandomMusic()
        {
            var albums = await _albumService.Get10RandomAlbumsWithArtistAsync();

            if (albums == null || !albums.Any())
            {
                Console.WriteLine("Album_1");
                return NotFound(new { Message = "No albums found." });
            }
            Console.WriteLine("Album_2");
            return Ok(albums);
        }
        [HttpGet("10random-albumiD")]
        public async Task<IActionResult> GetRandomMusicsID()
        {
            var albums = await _albumRepository.Get10RandomAlbumIdAsync();
          
            if (albums == null || !albums.Any())
            {
                return NotFound(new { Message = "No albums found." });
            }

            return Ok(albums);
        }
    }
}
