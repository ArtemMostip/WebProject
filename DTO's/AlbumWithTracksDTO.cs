using WebProject.Entities;

namespace WebProject.DTO_s
{
    public class AlbumWithTracksDTO
    {
        public string? Id { get; set; }
        public required string Title { get; set; }
        public string AlbCoverUrl { get; set; }
        public int? CountOfMusicInAlbum { get; set; }
        public List<string>? MusicInAlbum { get; set; }  // ← ID пісень
        public Artist? Artist { get; set; }
    }
}
