using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using SkinHolderDesktop.Services;
using System.IO;
using System.Text.Json;
using System.Windows;

namespace SkinHolderDesktop.ViewModels;

public partial class LoginViewModel : ObservableObject
{
    private readonly ILoginService _loginService;
    private readonly IWindowService _windowService;
    private readonly GlobalViewModel _globalViewModel;
    private readonly ILoggerService _loggerService;

    [ObservableProperty]
    private string userName = "";

    [ObservableProperty]
    private string password = "";

    [ObservableProperty]
    private string? errorText;

    [ObservableProperty]
    private bool guardarPassword;

    public LoginViewModel(ILoginService loginService, IWindowService windowService, GlobalViewModel globalViewModel, ILoggerService loggerService)
    {
        _loginService = loginService;
        _windowService = windowService;
        _globalViewModel = globalViewModel;
        _loggerService = loggerService;

        CargarUltimoUsername();
    }

    [RelayCommand]
    private void ToggleGuardarPassword()
    {
        GuardarPassword = !GuardarPassword;
    }


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

            _globalViewModel.CurrentUsername = _loginService.CurrentUsername;
            _globalViewModel.Token = _loginService.Token;
            _globalViewModel.UserId = _loginService.UserId;
            _globalViewModel.IsAuthenticated = true;
            
            _windowService.ShowMainWindow();
            _windowService.CloseLoginWindow();

            await _loggerService.SendLog($"Login", 1);

            return;
        }

        ErrorText = error ?? "Usuario o contraseña incorrectos";
    }

    [RelayCommand]
    private static void Registrar()
    {
        MessageBox.Show("De momento el registro de nuevos usuarios es privado. El correo para solicitar un usuario es skinholder@jagoba.dev -> copiado al portapapeles.");

        Clipboard.SetText("skinholder@jagoba.dev");
    }

    private void CargarUltimoUsername()
    {
        var path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "last.json");
        if (!File.Exists(path)) return;

        var jsonString = File.ReadAllText(path);
        var data = JsonSerializer.Deserialize<JsonElement>(jsonString);

        UserName = data.GetProperty("last_username").GetString()!;

        if (data.TryGetProperty("last_password", out var passProp))
        {
            Password = passProp.GetString()!;
            GuardarPassword = true;
        }
    }

    private void GuardarUsername()
    {
        var path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "last.json");

        var data = new
        {
            last_username = UserName,
            last_password = GuardarPassword ? Password : ""
        };

        var jsonString = JsonSerializer.Serialize(data);
        File.WriteAllText(path, jsonString);
    }
}