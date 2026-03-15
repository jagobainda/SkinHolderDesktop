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
        services.AddSingleton<IAuthSession>(provider => provider.GetRequiredService<GlobalViewModel>());
        services.AddSingleton<ITokenProvider>(provider => provider.GetRequiredService<IAuthSession>());
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

        // View factories
        services.AddSingleton<Func<Bienvenida>>(p => () => p.GetRequiredService<Bienvenida>());
        services.AddSingleton<Func<Registros>>(p => () => p.GetRequiredService<Registros>());
        services.AddSingleton<Func<UserItems>>(p => () => p.GetRequiredService<UserItems>());
        services.AddSingleton<Func<UserSettings>>(p => () => p.GetRequiredService<UserSettings>());
        services.AddSingleton<Func<RegistroDetailsViewModel>>(p => () => p.GetRequiredService<RegistroDetailsViewModel>());
        services.AddSingleton<Func<RegistroDetails>>(p => () => p.GetRequiredService<RegistroDetails>());
        services.AddSingleton<Func<RegistroListViewModel>>(p => () => p.GetRequiredService<RegistroListViewModel>());
        services.AddSingleton<Func<RegistroList>>(p => () => p.GetRequiredService<RegistroList>());

        // Singletons
        services.AddSingleton<IMessenger, WeakReferenceMessenger>();

        services.AddSingleton(new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        });

        // HttpClient with session handling
        services.AddSingleton<AuthenticationHandler>();
        services.AddTransient<UnauthorizedHandler>();

        services.AddSingleton(provider =>
        {
            var authHandler = provider.GetRequiredService<AuthenticationHandler>();
            var unauthorizedHandler = provider.GetRequiredService<UnauthorizedHandler>();

            unauthorizedHandler.InnerHandler = new HttpClientHandler();
            authHandler.InnerHandler = unauthorizedHandler;

            return new HttpClient(authHandler) { BaseAddress = new Uri("https://shapi.jagoba.dev") };
        });

        services.AddKeyedSingleton("steam", (_, _) => new HttpClient());
        services.AddKeyedSingleton("extsites", (_, _) => new HttpClient());

        return services.BuildServiceProvider();
    }
}