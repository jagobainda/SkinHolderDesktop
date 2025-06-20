using Microsoft.Extensions.DependencyInjection;
using SkinHolderDesktop.Views;
using System.Windows;

namespace SkinHolderDesktop.Services;

public interface IWindowService
{
    void ShowMainWindow();
    void CloseLoginWindow();
}

public class WindowService : IWindowService
{
    public void ShowMainWindow()
    {
        var mainWindow = App.Services.GetRequiredService<MainWindow>();
        mainWindow.Show();
    }

    public void CloseLoginWindow()
    {
        foreach (Window w in Application.Current.Windows)
        {
            if (w is LoginWindow) { w.Close(); break; }
        }
    }
}