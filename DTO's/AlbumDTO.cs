using WebProject.Entities;

namespace WebProject.DTO_s
{
    public class AlbumDTO
    {
        public required string Title { get; set; }
        public string? ArtistId { get; set; }
        public required List<string>? MusicInAlbum { get; set; }
        public required string AlbCoverUrl { get; set; }
        public int? CountOfMusicInAlbum { get; set; }
       
    }
}
