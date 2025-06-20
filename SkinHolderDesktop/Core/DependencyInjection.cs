using Microsoft.Extensions.DependencyInjection;
using SkinHolderDesktop.Services;
using SkinHolderDesktop.ViewModels;
using SkinHolderDesktop.Views;
using System.Text.Json;

namespace SkinHolderDesktop.Core;

public static class DependencyInjection
{
    public static IServiceProvider Configure()
    {
        var services = new ServiceCollection();

        // ViewModels
        services.AddSingleton<MainViewModel>();
        services.AddSingleton<LoginViewModel>();

        // Services
        services.AddSingleton<IWindowService, WindowService>();
        services.AddSingleton<ILoginService, LoginService>();

        // Views
        services.AddSingleton<MainWindow>();
        services.AddSingleton<LoginWindow>();

        // Singletons
        services.AddSingleton(new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        });

        return services.BuildServiceProvider();
    }
}