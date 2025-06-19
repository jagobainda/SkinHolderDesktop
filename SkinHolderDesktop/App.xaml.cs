using Microsoft.Extensions.DependencyInjection;
using SkinHolderDesktop.Core;
using System.Windows;

namespace SkinHolderDesktop;

public partial class App : Application
{
    public static IServiceProvider Services { get; private set; } = null!;

    protected override void OnStartup(StartupEventArgs e)
    {
        Services = DependencyInjection.Configure();

        var mainWindow = Services.GetRequiredService<MainWindow>();
        mainWindow.Show();

        base.OnStartup(e);
    }
}
