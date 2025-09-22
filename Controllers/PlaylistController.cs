using Microsoft.AspNetCore.Mvc;
using WebProject.DTO_s;
using WebProject.Services;

namespace WebProject.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PlaylistController : ControllerBase
    {
        IPlaylistService _playlistService;
        public PlaylistController(IPlaylistService playlistService)
        {
            _playlistService = playlistService;
        }


        [HttpPost("Create")]
        public async Task<IActionResult> CreatePlaylist([FromBody] PlaylistDTO playlistDTO)
        {
            if (playlistDTO == null)
            {
                return BadRequest(new { Message = "Playlist data is null" });
            }

            try
            {
                await _playlistService.CreatePlaylistAsync(playlistDTO);
                return Ok("Playlist created successfully.");
            }
            catch (Exception ex)
            {
                return BadRequest(new { Message = ex.Message });
            }
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetPlaylistWithMusics(string id)
        {
            var playlistWithMusics = await _playlistService.GetPlaylistWithMusicsAsync(id);

            if (playlistWithMusics == null)
            {
                return NotFound(new { Message = $"Playlist with ID '{id}' not found." });
            }

            return Ok(playlistWithMusics);
        }
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeletePlaylistAsync(string id)
        {
            var DeletedDocument = await _playlistService.DeletePlaylistAsync(id);

            if (!DeletedDocument)
            {
                return NotFound(new { Message = $"Playlist with ID '{id}' not found." });
            }

            return Ok(new { Message = $"Playlist with ID '{id}' was successfully deleted." });
        }
    }
}
