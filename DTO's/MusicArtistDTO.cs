using WebProject.Entities;

namespace WebProject.DTO_s
{
    public class MusicArtistDTO
    {
        public string? Id { get; set; }
        public string Title { get; set; } = null!;
        public Artist? Artist { get; set; } 
        public MusicAlbumArtistDTO? Album { get; set; }
        public int Year { get; set; }
        public string[] Genre { get; set; } = null!;
        public string CoverUrl { get; set; } = null!;
        public string FileUrl { get; set; } = null!;
        public DateTime DateCreation { get; set; }

    }
}
