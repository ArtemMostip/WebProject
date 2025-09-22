namespace WebProject.DTO_s
{
    public class ArtistWithAlbumsDTO

    {
     public string Id { get; set; }
    public string Name { get; set; }
    public string AvatarUrl { get; set; }
    public string ArtistBannerUrl { get; set; }
    public string? ArtistLogo { get; set; }
    public List<AlbumWithMusicDTO>? Albums { get; set; }
    }
}
