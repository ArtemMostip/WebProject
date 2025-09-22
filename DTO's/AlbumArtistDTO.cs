using WebProject.Entities;

namespace WebProject.DTO_s
{
    public class AlbumArtistDTO
    {
        public required string Title { get; set; }
        public Artist? Artist { get; set; }
        public int? CountOfMusicInAlbum { get; set; }
        public required string AlbCoverUrl { get; set; }
        public required List<MusicArtistDTO>? MusicInAlbum { get; set; }
    }
}
