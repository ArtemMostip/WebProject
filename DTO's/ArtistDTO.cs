namespace WebProject.DTO_s
{
    public class ArtistDTO
    {

        public required string Name { get; set; }
        public required string AvatarUrl { get; set; }
        public required string ArtistBannerUrl { get; set; }
        public string? ArtistLogo { get; set; }
    }
}
