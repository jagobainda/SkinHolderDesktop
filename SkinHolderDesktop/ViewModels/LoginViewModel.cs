using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using SkinHolderDesktop.Services;
using System.IO;
using System.Text.Json;
using System.Windows;

namespace SkinHolderDesktop.ViewModels;

public partial class LoginViewModel(ILoginService loginService, IWindowService windowService, MainViewModel mainViewModel) : ObservableObject
{
    private readonly ILoginService _loginService = loginService;
    private readonly IWindowService _windowService = windowService;
    private readonly MainViewModel _mainViewModel = mainViewModel;

    [ObservableProperty]
    private string userName = "";

    [ObservableProperty]
    private string password = "";

    [ObservableProperty]
    private string? errorText;

    [RelayCommand]
    private async Task IniciarSesionAsync()
    {
        ErrorText = "";

        if (string.IsNullOrWhiteSpace(UserName) || string.IsNullOrWhiteSpace(Password))
        {
            ErrorText = "Debes introducir usuario y contraseña.";
            return;
        }

        var (success, error) = await _loginService.LoginAsync(UserName.Trim(), Password.Trim());

        if (success)
        {
            GuardarUsername();

            _mainViewModel.CurrentUsername = _loginService.CurrentUsername;
            _mainViewModel.Token = _loginService.Token;
            _mainViewModel.IsAuthenticated = true;

            _windowService.ShowMainWindow();
            _windowService.CloseLoginWindow();
        }
        else
        {
            ErrorText = error ?? "Usuario o contraseña incorrectos";
        }
    }

    [RelayCommand]
    private static void Registrar()
    {
        MessageBox.Show("La aplicación sigue en desarrollo. Para acceder a la Alpha puedes contactarme a Discord: jagobainda#5551");
    }

    private void CargarUltimoUsername()
    {
        var path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "last_username.json");
        if (!File.Exists(path)) return;

        var jsonString = File.ReadAllText(path);
        var data = JsonSerializer.Deserialize<JsonElement>(jsonString);
        UserName = data.GetProperty("last_username").GetString()!;
    }

    private void GuardarUsername()
    {
        var path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "last_username.json");
        var data = new { last_username = UserName };
        var jsonString = JsonSerializer.Serialize(data);
        File.WriteAllText(path, jsonString);
    }
}