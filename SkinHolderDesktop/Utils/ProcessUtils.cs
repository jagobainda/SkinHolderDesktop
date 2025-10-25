namespace SkinHolderDesktop.Utils;

public static class ProcessUtils
{
    public static void RestartApplication()
    {
        var exePath = System.Diagnostics.Process.GetCurrentProcess().MainModule?.FileName;

        if (exePath is not null)System.Diagnostics.Process.Start(exePath);

        System.Diagnostics.Process.GetCurrentProcess().Kill();
    }

    public static void SaveErrorMessageToFile(string errorMessage)
    {
        var path = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "close_error.json");

        var data = new { error_message = errorMessage };

        var jsonString = System.Text.Json.JsonSerializer.Serialize(data);
        System.IO.File.WriteAllText(path, jsonString);
    }
}
