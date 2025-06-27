using Microsoft.Extensions.DependencyInjection;
using SkinHolderDesktop.Services;
using SkinHolderDesktop.ViewModels;
using SkinHolderDesktop.Views;
using SkinHolderDesktop.Views.Partials;
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
        services.AddSingleton<RegistrosViewModel>();

        // Services
        services.AddSingleton<IWindowService, WindowService>();
        services.AddSingleton<ILoginService, LoginService>();
        services.AddSingleton<IRegistroService, RegistroService>();

        // Views
        services.AddSingleton<LoginWindow>();
        services.AddSingleton<MainWindow>();
        services.AddSingleton<Registros>();

        // Partial Views
        services.AddTransient<Bienvenida>();

        // Singletons
        services.AddSingleton(new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        });

        services.AddSingleton(provider =>
        {
            return new HttpClient
            {
                BaseAddress = new Uri("https://shapi.jagoba.dev")
            };
        });

        return services.BuildServiceProvider();
    }
}