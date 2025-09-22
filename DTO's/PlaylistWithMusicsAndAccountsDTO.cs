using WebProject.Entities;

namespace WebProject.DTO_s
{
    public class PlaylistWithMusicsAndAccountsDTO
    {
        public string? Id { get; set; }
        public required string AccountId { get; set; }
        public AccountInfoDTO? Account { get; set; }
        public required string Name { get; set; } = string.Empty;
        public List<Music>? Musics { get; set; }
    }
}
