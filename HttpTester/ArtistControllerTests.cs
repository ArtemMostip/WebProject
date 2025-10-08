using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using MongoDB.Driver;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using WebProject.Datas;
using WebProject.DTO_s;
using WebProject.Entities;

namespace HttpTester
{
    public class ArtistControllerTests : IClassFixture<WebApplicationFactory<Program>>
    {
        private readonly HttpClient _client;
        private readonly IMongoCollection<Artist> _artists;
        private readonly IMongoCollection<Music> _musics;
        private readonly IMongoCollection<Album> _albums;
        private readonly IMongoCollection<Playlist> _playlists;

        public ArtistControllerTests(WebApplicationFactory<Program> factory)
        {
            _client = factory.CreateClient();

            var scopeFactory = factory.Services.GetService<IServiceScopeFactory>();
            using var scope = scopeFactory.CreateScope();

            var configuration = scope.ServiceProvider.GetRequiredService<IConfiguration>();
            var mongoDbService = new MongoDbService(configuration);
         
            _artists = mongoDbService.Database.GetCollection<Artist>("artists");
            _musics = mongoDbService.Database.GetCollection<Music>("musics");
            _albums = mongoDbService.Database.GetCollection<Album>("albums");
            _playlists = mongoDbService.Database.GetCollection<Playlist>("playlists");
        }

        public async Task<Artist> GetArtistByIdAsync(string id)
        {
            return await _artists.Find(a => a.Id == id).FirstOrDefaultAsync();
        }

        [Fact]
        public async Task ArtistHttpPostCreateTestAsync()
        {
            var add = new
            {
                Name = "Test Artist Name1",
                AvatarUrl = "Deftones/deftones_avatar.jpg",
                ArtistBannerUrl = "Deftones/Deftones_gif_from_site.gif",
                ArtistLogo = "Deftones/deftones_logo1.png"

            };
            var json = JsonConvert.SerializeObject(add);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await _client.PostAsync("/api/Artist/Create", content);

            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            var mongoClient = new MongoClient("mongodb+srv://artemmostipaka07:2wxBqUNxINdeHg4O@cluster0.pihclvh.mongodb.net/");
            var database = mongoClient.GetDatabase("musicplatform");
            var artist = database.GetCollection<Artist>("artists");

            var deleteResult = await artist.DeleteOneAsync(art => art.Name == "Test Artist Name1");
            Assert.Equal(1, deleteResult.DeletedCount);

        }
        [Fact]
        public async Task Invalid_ArtistHttpPostCreateTestAsync()
        {
            var add = new Dictionary<string, object?>
            {
                ["Name"] = null,
                ["AvatarUrl"] = "Muse/muse_avatar.jpg",
                ["ArtistBannerUrl"] = "Muse/muse_banner.gif",
                ["ArtistLogo"] = "Muse/MuseLogo.png"
            };

            var json = JsonConvert.SerializeObject(add);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await _client.PostAsync("/api/Artist/Create", content);

            Assert.False(response.IsSuccessStatusCode);

            
            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);

        }

        [Fact]
        public async Task ArtistHttpGetByIdTestAsync()
        {

            string existingArtistId = "68d957d0651c63c5dadb793f";

            var response = await _client.GetAsync($"/api/Artist/artist/{existingArtistId}");

            response.EnsureSuccessStatusCode();
            var content = await response.Content.ReadAsStringAsync();
            Assert.Contains(existingArtistId, content);
        }

        [Fact]
        public async Task Invalid_ArtistHttpGetByIdTestAsync()
        {
            string NotexistingArtistId = "681fa22ebb454a53d015246F";
            var response = await _client.GetAsync($"/api/Artist/artist/{NotexistingArtistId}");
            var content = await response.Content.ReadAsStringAsync();
            Assert.False(response.StatusCode == HttpStatusCode.OK);
        }
        [Fact]
        public async Task ArtistHttpGetByIdAvatarTestAsync()
        {
            string artistId = "68d957d0651c63c5dadb793f";
            var artist = await GetArtistByIdAsync(artistId);

            string coverFileName = artist.AvatarUrl;

            string filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "MUSICAPI", "UIA", coverFileName);

            var directory = Path.GetDirectoryName(filePath);
            if (!Directory.Exists(directory))
            {
                Directory.CreateDirectory(directory);
            }

            if (!File.Exists(filePath))
            {

                await File.WriteAllBytesAsync(filePath, new byte[] { 255, 216, 255 });
            }

            var response = await _client.GetAsync($"/api/Artist/avatar/{artistId}");

            Assert.Equal(HttpStatusCode.OK, response.StatusCode);

            var contentType = response.Content.Headers.ContentType.MediaType;
            Assert.Contains(contentType, new[] { "image/jpeg", "image/png" });

            File.Delete(filePath);
        }

        [Fact]
        public async Task Invalid_ArtistHttpGetByIdAvatarTestAsync()
        {
            string artistId = "68d957d0651c63c5dadb793f";
            var artist = await GetArtistByIdAsync(artistId);

            string coverFileName = artist.AvatarUrl;

            string filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "MUSICAPI", "UIA", coverFileName);

            var directory = Path.GetDirectoryName(filePath);
            if (!Directory.Exists(directory))
            {
                Directory.CreateDirectory(directory);
            }

            if (File.Exists(filePath))
            {

                File.Delete(filePath);
            }

            var response = await _client.GetAsync($"/api/Artist/avatar/{artistId}");

             Assert.False(response.StatusCode == HttpStatusCode.OK, $"Очікувалась помилка, але прийшов статус: {response.StatusCode}");
        }

        [Fact]
        public async Task ArtistHttpGetByIdBannerTestAsync()
        {
            string artistId = "68d957d0651c63c5dadb793f";
            var artist = await GetArtistByIdAsync(artistId);

            string coverFileName = artist.ArtistBannerUrl;

            string filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "MUSICAPI", "UIA", coverFileName);

            var directory = Path.GetDirectoryName(filePath);
            if (!Directory.Exists(directory))
            {
                Directory.CreateDirectory(directory);
            }

            if (!File.Exists(filePath))
            {

                await File.WriteAllBytesAsync(filePath, new byte[] { 255, 216, 255 });
            }

            var response = await _client.GetAsync($"/api/Artist/banner/{artistId}");

            Assert.Equal(HttpStatusCode.OK, response.StatusCode);

            var contentType = response.Content.Headers.ContentType.MediaType;
            Assert.Contains(contentType, new[] { "image/jpeg", "image/png" });

            File.Delete(filePath);
        }

        [Fact]
        public async Task Invalid_ArtistHttpGetByIdBannerTestAsync()
        {
            string artistId = "68d957d0651c63c5dadb793f";
            var artist = await GetArtistByIdAsync(artistId);
            string coverFileName = artist.ArtistBannerUrl;
            string filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "MUSICAPI", "UIA", coverFileName);
            var directory = Path.GetDirectoryName(filePath);
            if (!Directory.Exists(directory))
            {
                Directory.CreateDirectory(directory);
            }
            if (File.Exists(filePath))
            {
                File.Delete(filePath);
            }
            var response = await _client.GetAsync($"/api/Artist/banner/{artistId}");
            Assert.False(response.StatusCode == HttpStatusCode.OK, $"Очікувалась помилка, але прийшов статус: {response.StatusCode}");
        }

        [Fact]
        public async Task ArtistHttpGetByIdlogoTestAsync()
        {
            string artistId = "68d957d0651c63c5dadb793f";
            var artist = await GetArtistByIdAsync(artistId);

            string coverFileName = artist.ArtistLogo;

            string filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "MUSICAPI", "UIA", coverFileName);

            var directory = Path.GetDirectoryName(filePath);

            if (!Directory.Exists(directory))
            {
                Directory.CreateDirectory(directory);
            }

            if (!File.Exists(filePath))
            {

                await File.WriteAllBytesAsync(filePath, new byte[] { 255, 216, 255 });
            }

            var response = await _client.GetAsync($"/api/Artist/logo/{artistId}");

            Assert.Equal(HttpStatusCode.OK, response.StatusCode);

            var contentType = response.Content.Headers.ContentType.MediaType;
            Assert.Contains(contentType, new[] { "image/jpeg", "image/png" });

            File.Delete(filePath);
        }
        [Fact]
        public async Task Invalid_ArtistHttpGetByIdlogoTestAsync()
        {
            string artistId = "68d957d0651c63c5dadb793f";
            var artist = await GetArtistByIdAsync(artistId);

            string coverFileName = artist.ArtistLogo;

            string filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "MUSICAPI", "UIA", coverFileName);

            var directory = Path.GetDirectoryName(filePath);

            if (!Directory.Exists(directory))
            {
                Directory.CreateDirectory(directory);
            }

            if (File.Exists(filePath))
            {

               File.Delete(filePath);
            }

            var response = await _client.GetAsync($"/api/Artist/logo/{artistId}");

            Assert.False(response.StatusCode == HttpStatusCode.OK, $"Очікувалась помилка, але прийшов статус: {response.StatusCode}");
        }

        [Fact]
        public async Task ArtistHttpGet5RandomArtistsTestAsync()
        {
            var response = await _client.GetAsync($"/api/Artist/5random-artists");
            response.EnsureSuccessStatusCode();
            var content = await response.Content.ReadAsStringAsync();
            var artists = JsonConvert.DeserializeObject<List<Artist>>(content);
            Assert.Equal(3, artists.Count);
        }

        [Fact]
        public async Task ArtistHttpGet5RandomArtistsIdTestAsync()
        {
            var response = await _client.GetAsync($"/api/Artist/5random-artistID");
            response.EnsureSuccessStatusCode();
            var content = await response.Content.ReadAsStringAsync();
            var artists = JsonConvert.DeserializeObject<List<string>>(content);
            Assert.Equal(3, artists.Count);
        }
        [Fact]
        public async Task ArtistMusicArtistsHttpGet5RandomTestAsync()
        {
            var existingArtistId = "68d957d0651c63c5dadb793f";

            var response = await _client.GetAsync($"/api/Artist/5randomMusic/ArtistId/{existingArtistId}");
           
            response.EnsureSuccessStatusCode();

            var content = await response.Content.ReadAsStringAsync();
            var tracks = JsonConvert.DeserializeObject<List<MusicArtistDTO>>(content);

            Assert.NotNull(tracks);
            Assert.Equal(5, tracks.Count); 

            foreach (var track in tracks)
            {
                Assert.NotNull(track.Artist);
                Assert.Equal(existingArtistId, track.Artist.Id);
            }
        }
        [Fact]
        public async Task DeleteArtistAllByIdTestAsync()
        {

            var newArtist = new
            {
                Name = "Temp Artist for Cascade Delete",
                AvatarUrl = "avatars/temp_avatar.jpg",
                ArtistBannerUrl = "banners/temp_banner.jpg",
                ArtistLogo = "logos/temp_logo.png"
            };

            var json = JsonConvert.SerializeObject(newArtist);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            // Створюємо артиста
            var createResponse = await _client.PostAsync("/api/Artist/Create", content);
            createResponse.EnsureSuccessStatusCode();

            var createdArtistName = newArtist.Name;

            // Витягуємо артиста з бази по імені (оскільки відповідь не повертає об'єкт)
            var createdArtist = await _artists.Find(a => a.Name == createdArtistName).FirstOrDefaultAsync();

            Assert.NotNull(createdArtist);
            var artistId = createdArtist.Id;
            Assert.False(string.IsNullOrEmpty(artistId));

            // === Act ===

            var deleteResponse = await _client.DeleteAsync($"/api/Artist/by-Artist{artistId}");

            // === Assert ===

            Assert.Equal(HttpStatusCode.OK, deleteResponse.StatusCode);
            var deleteContent = await deleteResponse.Content.ReadAsStringAsync();
            Assert.Contains("was successfully deleted", deleteContent, StringComparison.OrdinalIgnoreCase);

            // 1. Артист має бути видалений
            var deletedArtist = await _artists.Find(a => a.Id == artistId).FirstOrDefaultAsync();
            Assert.Null(deletedArtist);

            // 2. Перевірити, що всі пісні артиста також видалено
            var artistMusics = await _musics.Find(m => m.ArtistId == artistId).ToListAsync();
            Assert.Empty(artistMusics);

            // 3. Перевірити, що ці пісні також видалені з плейлистів
            var allPlaylists = await _playlists.Find(_ => true).ToListAsync();

            var deletedMusicIds = artistMusics.Select(m => m.Id).ToHashSet();

            foreach (var playlist in allPlaylists)
            {
                if (playlist.MusicId == null) continue;

                foreach (var musicId in playlist.MusicId)
                {
                    Assert.DoesNotContain(musicId, deletedMusicIds);
                }
            }
        }

        [Fact]
        public async Task Invalid_DeleteArtistAllByIdTestAsync()
        {
           
            var nonExistentArtistId = "68d94f7e826eb0692755ffff"; 

            var response = await _client.DeleteAsync($"/api/Artist/by-Artist{nonExistentArtistId}");

            Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);

            var content = await response.Content.ReadAsStringAsync();
            Assert.Contains("not found", content, StringComparison.OrdinalIgnoreCase);
        }
        //[Fact]
        //public async Task DeleteAllMusicFromPlaylistTestAsync()
        //{
        //    var existingArtistId = "681e416f1b44faf58059d731";

        //    var response = await _client.DeleteAsync($"/api/Artist/by-Playlist{existingArtistId}");

        //    Assert.Equal(HttpStatusCode.OK, response.StatusCode);

            //var existingArtistId = "68d94f7e826eb0692755d934";

            //var artistMusicIds = (await _musics.Find(m => m.ArtistId == existingArtistId).ToListAsync())
            //    .Select(m => m.Id)
            //    .ToHashSet();

            //var playlists = await _playlists.Find(_ => true).ToListAsync();

            //foreach (var playlist in playlists)
            //{
            //    if (playlist.MusicId == null) continue;
            //    foreach (var id in playlist.MusicId)
            //    {
            //        Assert.DoesNotContain(id, artistMusicIds);
            //    }
            //}
        //}

    }
}
