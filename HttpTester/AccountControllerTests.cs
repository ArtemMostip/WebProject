using Microsoft.AspNetCore.Mvc.Testing;
using WebProject.Entities;
using MongoDB.Driver;
using System.Net;
using System.Net.Http.Headers;
using System.Text;
using Newtonsoft.Json;
using System.Net.Http.Json;


namespace HttpTester
{
    public class AccountControllerTests : IClassFixture<WebApplicationFactory<Program>>
    {
        private readonly HttpClient _client;

        public AccountControllerTests(WebApplicationFactory<Program> factory)
        {
           _client = factory.CreateClient();
        }

        private async Task<string> LoginAndGetJwtTokenAsync()
        {
            var loginData = new
            {
                Email = "artemmostipaka07@gmail.com",
                Password = "string"
            };

            var response = await _client.PostAsJsonAsync("/api/Account/Login", loginData);

            response.EnsureSuccessStatusCode();

            var json = await response.Content.ReadFromJsonAsync<Dictionary<string, string>>();

            return json["token"]; 
        }

        private async Task UpdateAccountAsync(string name, string password)
        {
            var putData = new
            {
                Name = name,
                Password = password
            };

            var response = await _client.PutAsJsonAsync("/api/Account/", putData);
            response.EnsureSuccessStatusCode();
        }

        [Fact]
        public async Task AccountHttpPutTestAsync()
        { 
            var token = await LoginAndGetJwtTokenAsync();
            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            
            await UpdateAccountAsync("Achilles", "string");
            await UpdateAccountAsync("AchillesImba", "string");
         
        }

        [Fact]
        public async Task AccountHttpGetInfoTestAsync()
        {
           
            string existingAccountId = "68109774de4a08faed56d86b"; 

            var response = await _client.GetAsync($"/api/Account/{existingAccountId}");

            response.EnsureSuccessStatusCode(); 
            var content = await response.Content.ReadAsStringAsync();
            Assert.Contains("68109774de4a08faed56d86b", content);
        }
        [Fact]
        public async Task Invalid_AccountHttpGetInfoTestAsync()
        {

            string invalidAccountId = "1234"; 

            var response = await _client.GetAsync($"/api/Account/{invalidAccountId}");

            Assert.Equal(System.Net.HttpStatusCode.BadRequest, response.StatusCode);

          
        }

        [Fact]
        public async Task AccountHttpGetAvatarTestAsync()
        {
            string existingAccountId = "68d403afe069b7df20ec2165";
            var response = await _client.GetAsync($"/api/Account/avatar/{existingAccountId}");
            response.EnsureSuccessStatusCode();
            Assert.Equal("image/png", response.Content.Headers.ContentType.ToString());
        }

        [Fact] 
        public async Task Invalid_AccountHttpGetAvatarTestAsync()
        {
            string invalidAccountId = "124";
            var response = await _client.GetAsync($"/api/Account/avatar/{invalidAccountId}");
            Assert.True(
     response.StatusCode == System.Net.HttpStatusCode.NotFound ||
     response.StatusCode == System.Net.HttpStatusCode.InternalServerError, $"Ти ловиш помилку: {response.StatusCode}");
        }

        [Fact]
        public async Task AccountHttpPostCreateTestAsync()
        {
        var formData = new MultipartFormDataContent
        {
            { new StringContent("TESTNAME"), "Name" },
            { new StringContent("TESTMAIL@gmail.com"), "Email" },
            { new StringContent("hashedPassword123"), "Password" }
        };

            var imageContent = new ByteArrayContent([1, 2, 3]);
        imageContent.Headers.ContentType = MediaTypeHeaderValue.Parse("application/octet-stream");
        formData.Add(imageContent, "AvatarFile", "avatar.jpg"); 

        var response = await _client.PostAsync("/api/Account/Create", formData);

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var mongoClient = new MongoClient("mongodb+srv://artemmostipaka07:2wxBqUNxINdeHg4O@cluster0.pihclvh.mongodb.net/");
        var database = mongoClient.GetDatabase("musicplatform");
        var accounts = database.GetCollection<Account>("accounts");

        var deleteResult = await accounts.DeleteOneAsync(acc => acc.Email == "TESTMAIL@gmail.com");
        Assert.Equal(1, deleteResult.DeletedCount);

        }

        [Fact]
        public async Task Invalid_AccountHttpPostCreateTestAsync()
        {
            var formData = new MultipartFormDataContent
            {
                { new StringContent("TESTNAME"), "Name" },
                { new StringContent(DateTime.UtcNow.ToString("o")), "Email" },
                { new StringContent("hashedPassword123"), "Password" }
            };
            var response = await _client.PostAsync("/api/Account/Create", formData);
            Assert.True(
            response.StatusCode == System.Net.HttpStatusCode.BadRequest ||
            response.StatusCode == System.Net.HttpStatusCode.InternalServerError || 
            response.StatusCode == System.Net.HttpStatusCode.UnsupportedMediaType, 
            $"Ця помилка не очікується {response.StatusCode}");
        }


        [Fact]
        public async Task AccountHttpPostLoginTestAsync()
        {
            var account = new
            {
                Email = "akaptar2007@gmail.com",
                Password = "string",
        };

            var json = JsonConvert.SerializeObject(account);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await _client.PostAsync("/api/Account/Login", content);

            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        }
        [Fact]
        public async Task Invalid_AccountHttpPostLoginTestAsync()
        {
            var account = new
            {
                Email = "akaptar2006@gmail.com",
                Password = "string"
            };

            var json = JsonConvert.SerializeObject(account);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await _client.PostAsync("/api/Account/Login", content);

            Assert.True(
            response.StatusCode == System.Net.HttpStatusCode.Unauthorized);
        }

        //[Fact]
        //public async Task AccountHttpPutAsync()
        //{
        //    var account = new
        //    {
        //        Email = "akaptar2006@gmail.com",
        //        Password = "string"
        //    };

        //    var json = JsonConvert.SerializeObject(account);
        //    var content = new StringContent(json, Encoding.UTF8, "application/json");

        //    var response = await _client.PostAsync("/api/Account/Login", content);

        //}


    }
}
