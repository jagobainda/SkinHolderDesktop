using Microsoft.Extensions.DependencyInjection;

namespace SkinHolderDesktop.Core;

public static class DependencyInjection
{
    public static IServiceProvider Configure()
    {
        var services = new ServiceCollection();

        // ViewModels

        // Services

        // Views
        services.AddSingleton<MainWindow>();

        return services.BuildServiceProvider();
    }
}