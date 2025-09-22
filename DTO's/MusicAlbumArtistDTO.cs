using WebProject.Entities;

namespace WebProject.DTO_s
{
    public class MusicAlbumArtistDTO
    {
        public string? Id { get; set; }
        public string Title { get; set; } = null!;
        public Artist? Artist { get; set; }
        public required string AlbCoverUrl { get; set; }
        public int? CountOfMusicInAlbum { get; set; }
    }
}
