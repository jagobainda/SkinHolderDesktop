using SkinHolderDesktop.Enums;
using SkinHolderDesktop.Models;
using System.Net.Http;
using System.Text.Json;

namespace SkinHolderDesktop.Services;

public interface IUserSettingsService
{
    Task<UserInfoResponse?> GetUserInfoAsync();
    Task<(bool Success, string? ErrorMessage)> ChangePasswordAsync(string currentPassword, string newPassword);
    Task<(bool Success, string? ErrorMessage)> DeleteAccountAsync(string currentPassword);
}

public class UserSettingsService(HttpClient httpClient, JsonSerializerOptions jsonOptions, ILoggerService loggerService) : BaseService(httpClient, jsonOptions), IUserSettingsService
{
    private readonly ILoggerService _loggerService = loggerService;

    public async Task<UserInfoResponse?> GetUserInfoAsync()
    {
        try
        {
            var response = await HttpClient.GetAsync("UserSettings");

            if (!response.IsSuccessStatusCode)
            {
                await _loggerService.SendLog($"Error al obtener info de usuario. Código: {(int)response.StatusCode}", ELogType.Warning);
                return null;
            }

            var json = await response.Content.ReadAsStringAsync();
            return JsonSerializer.Deserialize<UserInfoResponse>(json, JsonOptions);
        }
        catch (HttpRequestException ex)
        {
            await _loggerService.SendLog($"Error de conexión al obtener info de usuario: {ex.Message}", ELogType.Error);
            return null;
        }
        catch (JsonException ex)
        {
            await _loggerService.SendLog($"Error de deserialización al obtener info de usuario: {ex.Message}", ELogType.Error);
            return null;
        }
        catch (TaskCanceledException ex)
        {
            await _loggerService.SendLog($"Timeout al obtener info de usuario: {ex.Message}", ELogType.Warning);
            return null;
        }
        catch (Exception ex)
        {
            await _loggerService.SendLog($"Error inesperado al obtener info de usuario: {ex.Message}", ELogType.Error);
            return null;
        }
    }

    public async Task<(bool Success, string? ErrorMessage)> ChangePasswordAsync(string currentPassword, string newPassword)
    {
        var payload = new { currentPassword, newPassword };
        var content = CreateJsonContent(payload);

        try
        {
            var response = await HttpClient.PutAsync("UserSettings/password", content);

            if (!response.IsSuccessStatusCode)
            {
                var errorBody = await response.Content.ReadAsStringAsync();
                await _loggerService.SendLog($"Error al cambiar contraseña. Código: {(int)response.StatusCode}, Respuesta: {errorBody}", ELogType.Warning);
                return (false, errorBody);
            }

            return (true, null);
        }
        catch (HttpRequestException ex)
        {
            await _loggerService.SendLog($"Error de conexión al cambiar contraseña: {ex.Message}", ELogType.Error);
            return (false, "Error de conexión con el servidor");
        }
        catch (TaskCanceledException ex)
        {
            await _loggerService.SendLog($"Timeout al cambiar contraseña: {ex.Message}", ELogType.Warning);
            return (false, "Tiempo de espera agotado");
        }
        catch (Exception ex)
        {
            await _loggerService.SendLog($"Error inesperado al cambiar contraseña: {ex.Message}", ELogType.Error);
            return (false, "Error inesperado");
        }
    }

    public async Task<(bool Success, string? ErrorMessage)> DeleteAccountAsync(string currentPassword)
    {
        try
        {
            var request = new HttpRequestMessage(HttpMethod.Delete, "UserSettings/account")
            {
                Content = CreateJsonContent(new { currentPassword })
            };

            var response = await HttpClient.SendAsync(request);

            if (!response.IsSuccessStatusCode)
            {
                var errorBody = await response.Content.ReadAsStringAsync();
                await _loggerService.SendLog($"Error al eliminar cuenta. Código: {(int)response.StatusCode}, Respuesta: {errorBody}", ELogType.Warning);
                return (false, errorBody);
            }

            return (true, null);
        }
        catch (HttpRequestException ex)
        {
            await _loggerService.SendLog($"Error de conexión al eliminar cuenta: {ex.Message}", ELogType.Error);
            return (false, "Error de conexión con el servidor");
        }
        catch (TaskCanceledException ex)
        {
            await _loggerService.SendLog($"Timeout al eliminar cuenta: {ex.Message}", ELogType.Warning);
            return (false, "Tiempo de espera agotado");
        }
        catch (Exception ex)
        {
            await _loggerService.SendLog($"Error inesperado al eliminar cuenta: {ex.Message}", ELogType.Error);
            return (false, "Error inesperado");
        }
    }
}
