using System.IO;
using System.Text.Json;
using System.Windows;

namespace SkinHolderDesktop.Utils;

public static class ProcessUtils
{
    public static void RestartApplication()
    {
        var exePath = System.Diagnostics.Process.GetCurrentProcess().MainModule?.FileName;

        if (exePath is not null) System.Diagnostics.Process.Start(exePath);

        Application.Current.Shutdown();
    }

    public static void SaveErrorMessageToFile(string errorMessage)
    {
        var path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "close_error.json");

        var data = new { error_message = errorMessage };

        var jsonString = JsonSerializer.Serialize(data);

        File.WriteAllText(path, jsonString);
    }
}
