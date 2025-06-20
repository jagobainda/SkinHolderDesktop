using Microsoft.Extensions.DependencyInjection;
using SkinHolderDesktop.Core;
using SkinHolderDesktop.Views;
using System.Windows;

namespace SkinHolderDesktop;

public partial class App : Application
{
    public static IServiceProvider Services { get; private set; } = null!;

    protected override void OnStartup(StartupEventArgs e)
    {
        Services = DependencyInjection.Configure();

        var loginWindow = Services.GetRequiredService<LoginWindow>();
        loginWindow.Show();

        base.OnStartup(e);
    }
}
