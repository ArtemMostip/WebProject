using WebProject.Datas;
using WebProject.Repositories;
using WebProject.Services;

namespace WebProject.Extensions
{
    // Extensions/ServiceCollectionExtensions.cs
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddCustomServices(this IServiceCollection services)
        {
            services.AddTransient<IPasswordService, PasswordService>();

            services.AddTransient<IEmailRepository, EmailRepository>();
            services.AddTransient<IEmailService, EmailService>();

            services.AddTransient<IMusicRepository, MusicRepository>();
            services.AddTransient<IMusicService, MusicService>();
            services.AddTransient<IIMusicService, SecondMusicService>();

            services.AddTransient<IPlaylistRepository, PlaylistRepository>();
            services.AddTransient<IPlaylistService, PlaylistService>();

            services.AddTransient<IArtistRepository, ArtistRepository>();
            services.AddTransient<IArtistService, ArtistService>();

            services.AddTransient<IAlbumRepository, AlbumRepository>();
            services.AddTransient<IAlbumService, AlbumService>();

            services.AddTransient<IAccountRepository, AccountRepository>();
            services.AddTransient<IAccountService, AccountService>();

            services.AddSingleton<MongoDbService>();

            return services;
        }
    }

}
