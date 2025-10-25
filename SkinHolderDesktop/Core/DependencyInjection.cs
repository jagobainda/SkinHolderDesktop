using CommunityToolkit.Mvvm.Messaging;
using Microsoft.Extensions.DependencyInjection;
using SkinHolderDesktop.Services;
using SkinHolderDesktop.ViewModels;
using SkinHolderDesktop.ViewModels.Shared;
using SkinHolderDesktop.Views;
using SkinHolderDesktop.Views.Partials;
using SkinHolderDesktop.Views.Shared;
using System.Net.Http;
using System.Text.Json;

namespace SkinHolderDesktop.Core;

public static class DependencyInjection
{
    public static IServiceProvider Configure()
    {
        var services = new ServiceCollection();

        // ViewModels
        services.AddSingleton<GlobalViewModel>();
        services.AddSingleton<LoginViewModel>();
        services.AddSingleton<MainViewModel>();
        services.AddTransient<RegistrosViewModel>();
        services.AddTransient<UserItemsViewModel>();

        // Shared ViewModels
        services.AddTransient<RegistroDetailsViewModel>();
        services.AddTransient<RegistroListViewModel>();

        // Partial ViewModels
        services.AddTransient<UserItemViewModel>();

        // Services
        services.AddSingleton<IWindowService, WindowService>();
        services.AddSingleton<ILoginService, LoginService>();
        services.AddSingleton<ILoggerService, LoggerService>();
        services.AddSingleton<ISteamRequestService, SteamRequestService>();
        services.AddSingleton<IExtSitesRequestService, ExtSitesRequestService>();
        services.AddSingleton<IRegistroService, RegistroService>();
        services.AddSingleton<IItemPrecioService, ItemPrecioService>();
        services.AddSingleton<IItemsService, ItemsService>();
        services.AddSingleton<IUserItemService, UserItemService>();

        // Views
        services.AddSingleton<LoginWindow>();
        services.AddSingleton<MainWindow>();
        services.AddTransient<RegistroDetails>();
        services.AddTransient<RegistroList>();

        // Partial Views
        services.AddTransient<Bienvenida>();
        services.AddTransient<Registros>();
        services.AddTransient<UserItems>();
        services.AddTransient<UserSettings>();

        // Singletons
        services.AddSingleton<IMessenger, WeakReferenceMessenger>();

        services.AddSingleton(new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        });

        // HttpClient with session handling
        services.AddTransient<UnauthorizedHandler>();

        services.AddSingleton(provider =>
        {
            var handler = provider.GetRequiredService<UnauthorizedHandler>();
            handler.InnerHandler = new HttpClientHandler();
            
            return new HttpClient(handler)
            {
                BaseAddress = new Uri("https://shapi.jagoba.dev")
            };
        });

        return services.BuildServiceProvider();
    }
}