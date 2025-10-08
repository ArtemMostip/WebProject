using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using MongoDB.Driver;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http.Json;
using System.Text;
using System.Threading.Tasks;
using WebProject.Datas;
using WebProject.Entities;

namespace HttpTester
{
    public class AlbumControllerTests : IClassFixture<WebApplicationFactory<Program>>
    {
        private readonly HttpClient _client;
        private readonly IMongoCollection<Album> _album;

        public AlbumControllerTests(WebApplicationFactory<Program> factory)
        {
            _client = factory.CreateClient();

            var scopeFactory = factory.Services.GetService<IServiceScopeFactory>();
            using var scope = scopeFactory.CreateScope();

            var configuration = scope.ServiceProvider.GetRequiredService<IConfiguration>();
            var mongoDbService = new MongoDbService(configuration);
            _album = mongoDbService.Database.GetCollection<Album>("albums");
        }
        public async Task<Album> GetAlbumByIdAsync(string id)
        {
            return await _album.Find(a => a.Id == id).FirstOrDefaultAsync();
        }
        
        [Fact]
        public async Task AlbumHttpPostCreateTestAsync()
        {
            var musicInArray = new string[]
            {
                "681fa467bb454a53d0152476", 
                "681fa49dbb454a53d0152479"
            };
            var newAlbum = new
            {
                Title = "Test Album",
                ArtistId = "681fa22ebb454a53d015246e",
                MusicInAlbum = musicInArray,
                AlbCoverUrl = "Bring_Me_The_Horizon/Code_Mistake_Album/Code_Mistake_album_cover.jpg",
                CountOfMusicInAlbum = musicInArray.Length
            };

            var json = JsonConvert.SerializeObject(newAlbum);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await _client.PostAsync("/api/Album/Create", content);

            Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        }

        [Fact]
        public async Task Invalid_AlbumHttpPostCreateTestAsync()
        {
            var musicInArray = new string[]
            {
                "681fa467bb454a53d0152476",
                "681fa49dbb454a53d0152479"
            };
            var newAlbum = new
            {
                Title = "Test Album",
                ArtistId = "681fa22ebb454a53d015246e",
                MusicInAlbum = musicInArray,
                AlbCoverUrl = "Bring_Me_The_Horizon/Code_Mistake_Album/Code_Mistake_album_cover.jpg",
                CountOfMusicInAlbum = musicInArray
            };

            var json = JsonConvert.SerializeObject(newAlbum);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await _client.PostAsync("/api/Album/Create", content);
            Assert.False(response.StatusCode == HttpStatusCode.OK);
           

        }

        [Fact]
        public async Task AlbumHttpGetIdTestAsync()
        {
            string existingAlbumId = "681f893b403ff5e181a15efe";

            var response = await _client.GetAsync($"/api/Album/{existingAlbumId}");

            response.EnsureSuccessStatusCode();
            var content = await response.Content.ReadAsStringAsync();
            Assert.Contains(existingAlbumId, content);
        }

        [Fact]
        public async Task Invalid_AlbumHttpGetIdTestAsync()
        {
            string NotexistingAccountId = "681fa27fbb454a53d0152471";

            var response = await _client.GetAsync($"/api/Album/{NotexistingAccountId}");

            var content = await response.Content.ReadAsStringAsync();
            Assert.True(response.StatusCode == HttpStatusCode.NoContent, $"Помилка: {response.StatusCode}");
        }

        [Fact]
        public async Task AlbumHttpGetIdAlbumCoverTestAsync()
        {
            string albumId = "681f893b403ff5e181a15efe";
            var album = await GetAlbumByIdAsync(albumId);

            string coverFileName = album.AlbCoverUrl;

            string filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "MUSICAPI", "Covers", coverFileName);

            var directory = Path.GetDirectoryName(filePath);
            if (!Directory.Exists(directory))
            {
                Directory.CreateDirectory(directory);
            }

            if (!File.Exists(filePath))
            {
               
                await File.WriteAllBytesAsync(filePath, new byte[] { 255, 216, 255 }); 
            }

            var response = await _client.GetAsync($"/api/Album/AlbumCover/{albumId}");

            Assert.Equal(HttpStatusCode.OK, response.StatusCode);

            var contentType = response.Content.Headers.ContentType.MediaType;
            Assert.Contains(contentType, new[] { "image/jpeg", "image/png" });

            File.Delete(filePath);
        }

        [Fact]
        public async Task Invalid_AlbumHttpGetIdAlbumCoverTestAsync()
        {
            string albumId = "681f893b403ff5e181a15efe";
            var album = await GetAlbumByIdAsync(albumId);

            string coverFileName = album.AlbCoverUrl;

            string filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "MUSICAPI", "Covers", coverFileName);

            var directory = Path.GetDirectoryName(filePath);
            if (!Directory.Exists(directory))
            {
                Directory.CreateDirectory(directory);
            }

            if (File.Exists(filePath))
            {
                File.Delete(filePath);
            }

            var response = await _client.GetAsync($"/api/Album/AlbumCover/{albumId}");

            Assert.False(response.StatusCode == HttpStatusCode.OK, $"Очікувалась помилка, але прийшов статус: {response.StatusCode}");
        }

        [Fact]
        public async Task AlbumHttpGetAllAlbumsTestAsync()
        {
            var response = await _client.GetAsync("/api/Album/10random-album");
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            var responseString = await response.Content.ReadAsStringAsync();
            var albums = JsonConvert.DeserializeObject<List<Album>>(responseString);
            Assert.NotNull(albums);
            Assert.Equal(10, albums.Count);
        }

        //[Fact]
        //public async Task Invalid_AlbumHttpGetAllAlbumsTestAsync()
        //{
        //    var response = await _client.GetAsync("/api/Album/10random-album");
        //    Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        //    var responseString = await response.Content.ReadAsStringAsync();
        //    var albums = JsonConvert.DeserializeObject<List<Album>>(responseString);
        //    Assert.NotNull(albums);
        //    Assert.True(albums.Count > 0);
        //}

        [Fact]
        public async Task AlbumHttpGet10RandomAlbumsWithArtistTestAsync()
        {
            var response = await _client.GetAsync("/api/Album/10random-albumiD");
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            var responseString = await response.Content.ReadAsStringAsync();
            var albums = JsonConvert.DeserializeObject<List<string>>(responseString);
            Assert.NotNull(albums);
            Assert.Equal(10, albums.Count);
        }
        
        //[Fact]
        //public async Task Invalid_AlbumHttpGet10RandomAlbumsWithArtistTestAsync()
        //{
        //    var response = await _client.GetAsync("/api/Album/10random-albumiD");
        //    Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        //    var responseString = await response.Content.ReadAsStringAsync();
        //    var albums = JsonConvert.DeserializeObject<List<string>>(responseString);
        //    Assert.NotNull(albums);
        //    Assert.Equal(10, albums.Count);
        //}
    }
}
