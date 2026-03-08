using SkinHolderDesktop.Core;
using SkinHolderDesktop.Enums;
using SkinHolderDesktop.Models.Auth;
using System.Net.Http;
using System.Text.Json;

namespace SkinHolderDesktop.Services;

public interface ILoginService
{
    Task<(bool Success, string? ErrorMessage)> LoginAsync(string username, string password);
    Task<bool> ValidateToken(string token);
}

public class LoginService(HttpClient httpClient, JsonSerializerOptions jsonOptions, IAuthSession authSession, ILoggerService loggerService) : BaseService(httpClient, jsonOptions), ILoginService
{
    private readonly ILoggerService _loggerService = loggerService;

    public async Task<(bool Success, string? ErrorMessage)> LoginAsync(string username, string password)
    {
        var payload = new { username, password };
        var content = CreateJsonContent(payload);

        try
        {
            var response = await HttpClient.PostAsync("Auth/login", content);

            if (!response.IsSuccessStatusCode)
            {
                await _loggerService.SendLog($"Login inválido para el usuario '{username}'. Código: {(int)response.StatusCode}", ELogType.Warning);
                return (false, "Usuario o contraseña incorrectos");
            }

            var json = await response.Content.ReadAsStringAsync();
            var data = JsonSerializer.Deserialize<LoginResponse>(json, JsonOptions);

            if (data is null || string.IsNullOrWhiteSpace(data.Token)) return (false, "Respuesta inválida del servidor");

            authSession.Token = data.Token;
            authSession.CurrentUsername = data.Username;
            authSession.UserId = data.UserId;
            authSession.IsAuthenticated = true;

            return (true, null);
        }
        catch (HttpRequestException ex)
        {
            await _loggerService.SendLog($"Error de conexión durante login de '{username}': {ex.Message}", ELogType.Error);
            return (false, "Error de conexión con el servidor");
        }
        catch (JsonException ex)
        {
            await _loggerService.SendLog($"Error de deserialización en login de '{username}': {ex.Message}", ELogType.Error);
            return (false, "Error interpretando la respuesta");
        }
        catch (TaskCanceledException ex)
        {
            await _loggerService.SendLog($"Timeout durante login de '{username}': {ex.Message}", ELogType.Warning);
            return (false, "Tiempo de espera agotado al iniciar sesión");
        }
        catch (Exception ex)
        {
            await _loggerService.SendLog($"Error inesperado durante login de '{username}': {ex.Message}", ELogType.Error);
            return (false, "Error inesperado durante el login");
        }
    }

    public async Task<bool> ValidateToken(string token)
    {
        var request = new HttpRequestMessage(HttpMethod.Get, "Auth/validate");

        request.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);

        try
        {
            var response = await HttpClient.SendAsync(request);

            return response.IsSuccessStatusCode;
        }
        catch (HttpRequestException ex)
        {
            await _loggerService.SendLog($"Error validando token: {ex.Message}", ELogType.Warning);
            return false;
        }
        catch (TaskCanceledException ex)
        {
            await _loggerService.SendLog($"Timeout validando token: {ex.Message}", ELogType.Warning);
            return false;
        }
        catch (Exception ex)
        {
            await _loggerService.SendLog($"Error inesperado validando token: {ex.Message}", ELogType.Error);
            return false;
        }
    }
}
