namespace WebProject.DTO_s
{
    public class AlbumWithMusicDTO
    {
        public string Id { get; set; }
        public string Title { get; set; }
        public string? AlbCoverUrl { get; set; }
        public int CountOfMusicInAlbum { get; set; }

        public List<MusicArtistDTO> Music { get; set; } = new();
    }
}
