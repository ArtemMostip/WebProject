namespace WebProject.DTO_s
{
    public class MusicInpDTO
    {
        public required string Title { get; set; }
        public string? ArtistId { get; set; }
        public string? AlbumId { get; set; }
        public required int Year { get; set; }
        public required string[] Genre { get; set; }
        public required string CoverUrl { get; set; }
        public required string FileUrl { get; set; }
        public required int CountOfListening { get; set; } = 0;
    }
}
