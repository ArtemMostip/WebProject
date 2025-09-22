    using WebProject.Entities;
    using MongoDB.Driver;
    using MongoDB.Bson;
    using MongoDB.Bson.Serialization.Attributes;
    using WebProject.Datas;
    using Microsoft.AspNetCore.Identity;
    namespace WebProject.Repositories
{
    public class AccountRepository: IAccountRepository
    {
        
            private readonly IMongoCollection<Account> _accounts;

            public AccountRepository(MongoDbService mongoDbService)
            {
                _accounts = mongoDbService.Database.GetCollection<Account>("accounts");
            }

            public async Task CreateAccountAsync(Account account)
            {
                await _accounts.InsertOneAsync(account);
            }
        public async Task<Account> GetAccountByTokenAsync(string token)
        {
            return await _accounts.Find(a => a.Token == token && a.VerifiedAt == null).FirstOrDefaultAsync();
        }
        public async Task<List<Account>> GetAllVerifiedAccountsAsync()
            {
                return await _accounts.Find(a => a.VerifiedAt != null).ToListAsync();
            }

            public async Task<Account> GetAccountByIdAsync(string id)
            {
                return await _accounts.Find(a => a.Id == id).FirstOrDefaultAsync();
            }

            public async Task<Account> GetAccountByNameAsync(string name)
            {
                return await _accounts.Find(a => a.Name == name).FirstOrDefaultAsync();
            }

        public async Task<Account> GetAccountByEmailAsync(string email)
        {
            return await _accounts.Find(a => a.Email == email).FirstOrDefaultAsync();
        }

        public async Task<Account> GetAccountByEmailOrNameAsync(string email, string name)
            {
                return await _accounts.Find(a => a.Email == email || a.Name == name).FirstOrDefaultAsync();
            }
        public async Task<Account> GetImageByIdAsync(string id)
        {
            var filter = Builders<Account>.Filter.Eq(a => a.Id, id);
            return await _accounts.Find(filter).FirstOrDefaultAsync();
        }
            public async Task<bool> UpdateAccountAsync(Account account)
            {
                var updateResult = await _accounts.ReplaceOneAsync(a => a.Id == account.Id, account);
                return updateResult.MatchedCount > 0;
            }
       

    }
}
