using WebProject.DTO_s;
using WebProject.Entities;

namespace WebProject.Services
{
    public interface IAccountService
    {
        Task<Account> GetAccount(string id);
        Task<List<string>> GetAllEmailsAsync();
        Task<bool> CreateAccountAsync(AccountRegDTO accountDTO);
        Task<bool> ValidateAccountAsync(string email, string password);
        Task<Account> LoginAsync(AccountLoginDTO loginDto);
        Task<bool> VerifyEmailAsync(string token);
    }
}
