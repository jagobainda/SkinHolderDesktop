using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using SkinHolderDesktop.Core;
using SkinHolderDesktop.Services;
using SkinHolderDesktop.Views.Dialogs;
using System.Windows;

namespace SkinHolderDesktop.ViewModels;

public partial class UserSettingsViewModel(IUserSettingsService userSettingsService, IAuthSession authSession, ILoggerService loggerService) : ObservableObject, IDisposable
{
    private readonly IUserSettingsService _userSettingsService = userSettingsService;
    private readonly IAuthSession _authSession = authSession;
    private readonly ILoggerService _loggerService = loggerService;

    [ObservableProperty] private string username = string.Empty;
    [ObservableProperty] private string createdAtText = string.Empty;
    [ObservableProperty] private bool isLoading;

    // Change password
    [ObservableProperty] private string currentPassword = string.Empty;
    [ObservableProperty] private string newPassword = string.Empty;
    [ObservableProperty] private string confirmNewPassword = string.Empty;
    [ObservableProperty] private string passwordStatusMessage = string.Empty;
    [ObservableProperty] private bool isPasswordSuccess;

    // Delete account
    [ObservableProperty] private string deletePassword = string.Empty;
    [ObservableProperty] private string deleteStatusMessage = string.Empty;
    [ObservableProperty] private bool isDeleteSuccess;

    [RelayCommand]
    private async Task InitializeAsync()
    {
        IsLoading = true;

        try
        {
            var userInfo = await _userSettingsService.GetUserInfoAsync();

            if (userInfo != null)
            {
                Username = userInfo.Username;
                CreatedAtText = userInfo.CreatedAt.ToString("dd/MM/yyyy");
            }
            else
            {
                Username = _authSession.CurrentUsername ?? "Desconocido";
                CreatedAtText = "No disponible";
            }
        }
        finally
        {
            IsLoading = false;
        }
    }

    [RelayCommand]
    private async Task ChangePasswordAsync()
    {
        PasswordStatusMessage = string.Empty;

        if (string.IsNullOrWhiteSpace(CurrentPassword))
        {
            PasswordStatusMessage = "Introduce tu contraseña actual.";
            IsPasswordSuccess = false;
            return;
        }

        if (string.IsNullOrWhiteSpace(NewPassword))
        {
            PasswordStatusMessage = "Introduce la nueva contraseña.";
            IsPasswordSuccess = false;
            return;
        }

        if (NewPassword.Length < 6)
        {
            PasswordStatusMessage = "La nueva contraseña debe tener al menos 6 caracteres.";
            IsPasswordSuccess = false;
            return;
        }

        if (NewPassword != ConfirmNewPassword)
        {
            PasswordStatusMessage = "Las contraseñas nuevas no coinciden.";
            IsPasswordSuccess = false;
            return;
        }

        if (CurrentPassword == NewPassword)
        {
            PasswordStatusMessage = "La nueva contraseña no puede ser igual a la actual.";
            IsPasswordSuccess = false;
            return;
        }

        IsLoading = true;

        try
        {
            var (success, errorMessage) = await _userSettingsService.ChangePasswordAsync(CurrentPassword, NewPassword);

            if (success)
            {
                PasswordStatusMessage = "Contraseña actualizada correctamente.";
                IsPasswordSuccess = true;
                CurrentPassword = string.Empty;
                NewPassword = string.Empty;
                ConfirmNewPassword = string.Empty;
            }
            else
            {
                PasswordStatusMessage = errorMessage ?? "Error al cambiar la contraseña.";
                IsPasswordSuccess = false;
            }
        }
        finally
        {
            IsLoading = false;
        }
    }

    [RelayCommand]
    private async Task DeleteAccountAsync()
    {
        DeleteStatusMessage = string.Empty;

        if (string.IsNullOrWhiteSpace(DeletePassword))
        {
            DeleteStatusMessage = "Introduce tu contraseña para confirmar.";
            IsDeleteSuccess = false;
            return;
        }

        var confirm = CustomMessageBox.Show(
            "Eliminar cuenta",
            "¿Estás seguro de que quieres eliminar tu cuenta? Esta acción no se puede deshacer.",
            CustomMessageBoxButton.YesNo,
            CustomMessageBoxIcon.Error);

        if (confirm != true) return;

        IsLoading = true;

        try
        {
            var (success, errorMessage) = await _userSettingsService.DeleteAccountAsync(DeletePassword);

            if (success)
            {
                DeleteStatusMessage = "Cuenta eliminada. La aplicación se cerrará.";
                IsDeleteSuccess = true;

                await _loggerService.SendLog("Cuenta eliminada por el usuario.", Enums.ELogType.Info);

                // Restart the app to force re-login (account is deactivated)
                var exePath = Environment.ProcessPath;
                if (exePath != null) System.Diagnostics.Process.Start(exePath);
                Application.Current.Shutdown();
            }
            else
            {
                DeleteStatusMessage = errorMessage ?? "Error al eliminar la cuenta.";
                IsDeleteSuccess = false;
            }
        }
        finally
        {
            IsLoading = false;
        }
    }

    public void Dispose()
    {
        GC.SuppressFinalize(this);
    }
}
