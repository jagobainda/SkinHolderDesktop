﻿using Microsoft.Extensions.DependencyInjection;
using SkinHolderDesktop.Core;
using SkinHolderDesktop.Views;
using System.Windows;
using Velopack;

namespace SkinHolderDesktop;

public partial class App : Application
{
    public static IServiceProvider Services { get; private set; } = null!;

    private readonly string feedUrl = "https://cdn.jagoba.dev/downloads/skinholder-desktop-latest";

    protected override void OnStartup(StartupEventArgs e)
    {
        VelopackApp.Build().Run();

        Services = DependencyInjection.Configure();

        _ = Task.Run(CheckForUpdatesAsync);

        var loginWindow = Services.GetRequiredService<LoginWindow>();
        loginWindow.Show();

        base.OnStartup(e);
    }

    private async Task CheckForUpdatesAsync()
    {
        try
        {
            var mgr = new UpdateManager(feedUrl);

            var updateInfo = await mgr.CheckForUpdatesAsync();

            if (updateInfo == null) return;

            await mgr.DownloadUpdatesAsync(updateInfo);

            Dispatcher.Invoke(() =>
            {
                var result = MessageBox.Show($"Nueva versión {updateInfo.TargetFullRelease.Version} disponible. ¿Reiniciar para actualizar?", "Actualización disponible", MessageBoxButton.YesNo, MessageBoxImage.Question);

                if (result == MessageBoxResult.Yes) mgr.ApplyUpdatesAndRestart(updateInfo.TargetFullRelease);
            });
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Error checking for updates: {ex.Message}");
        }
    }

    protected override void OnExit(ExitEventArgs e)
    {
        base.OnExit(e);
    }
}