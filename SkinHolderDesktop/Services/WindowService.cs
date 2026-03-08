using Microsoft.Extensions.DependencyInjection;
using SkinHolderDesktop.Enums;
using SkinHolderDesktop.Views;
using System.Windows;

namespace SkinHolderDesktop.Services;

public interface IWindowService
{
    void ShowMainWindow();
    void CloseLoginWindow();
}

public class WindowService(ILoggerService loggerService) : IWindowService
{
    private readonly ILoggerService _loggerService = loggerService;

    public void ShowMainWindow()
    {
        try
        {
            var mainWindow = App.Services.GetRequiredService<MainWindow>();
            mainWindow.Show();
        }
        catch (Exception ex)
        {
            _ = _loggerService.SendLog($"Error mostrando MainWindow: {ex.Message}", ELogType.Error);
        }
    }

    public void CloseLoginWindow()
    {
        try
        {
            if (Application.Current is null) return;

            foreach (Window w in Application.Current.Windows)
            {
                if (w is LoginWindow) { w.Close(); break; }
            }
        }
        catch (Exception ex)
        {
            _ = _loggerService.SendLog($"Error cerrando LoginWindow: {ex.Message}", ELogType.Error);
        }
    }
}