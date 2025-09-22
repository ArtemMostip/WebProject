using WebProject.Entities;

namespace WebProject.Repositories
{
    public interface IAccountRepository
    {
        Task CreateAccountAsync(Account account);
        Task<List<Account>> GetAllVerifiedAccountsAsync();
        Task<Account> GetAccountByIdAsync(string id);
        Task<Account> GetAccountByNameAsync(string email);
        Task<Account> GetAccountByEmailOrNameAsync(string email, string name);
        Task<Account> GetAccountByTokenAsync(string token);
        Task<Account> GetAccountByEmailAsync(string email);
        Task<bool> UpdateAccountAsync(Account account);
        Task<Account>GetImageByIdAsync(string id);
    }
}
