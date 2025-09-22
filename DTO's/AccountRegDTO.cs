namespace WebProject.DTO_s
{
    public class AccountRegDTO
    {
        public required string Name { get; set; }  
        public required string Email { get; set; }  
        public required string Password { get; set; }
        public IFormFile? AvatarFile { get; set; }
    }
}
