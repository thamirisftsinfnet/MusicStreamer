using Microsoft.Extensions.DependencyInjection;
using MusicStreamer.Application.AppServices;
using MusicStreamer.Application.Interfaces.AppServices;
using MusicStreamer.Data.Context;
using MusicStreamer.Data.Repositories;
using MusicStreamer.Data.UoW;
using MusicStreamer.Domain.Interfaces.Repositories;
using MusicStreamer.Domain.Interfaces.Services;
using MusicStreamer.Domain.Interfaces.UnitOfWork;
using MusicStreamer.Domain.Services;

namespace MusicStreamer.IoC
{
    public class NativeInjectorBootstrapper
    {
        public static void RegisterServices(IServiceCollection services)
        {
            // Repository Registration
            services.AddScoped<IUserRepository, UserRepository>();
            services.AddScoped<IMusicRepository, MusicRepository>();
            services.AddScoped<IBandRepository, BandRepository>();
            services.AddScoped<IPlaylistRepository, PlaylistRepository>();
            services.AddScoped<ISubscriptionRepository, SubscriptionRepository>();
            services.AddScoped<IUnitOfWork, UnitOfWork>();

            // Service Registration
            services.AddScoped<IUserService, UserService>();
            services.AddScoped<IMusicService, MusicService>();
            services.AddScoped<IBandService, BandService>();
            services.AddScoped<ISubscriptionService, SubscriptionService>();
            services.AddScoped<IAuthService, AuthService>();
            services.AddScoped<INotificationService, NotificationService>();
        }
 
}
}
