using SkinHolderDesktop.Models.Auth;
using System.Net.Http;
using System.Text.Json;

namespace SkinHolderDesktop.Services;

public interface ILoginService
{
    Task<(bool Success, string? ErrorMessage)> LoginAsync(string username, string password);
    string? Token { get; }
    string? CurrentUsername { get; }
}

public class LoginService(HttpClient httpClient, JsonSerializerOptions jsonOptions) : BaseService(httpClient, jsonOptions), ILoginService
{
    public string? Token { get; private set; }
    public string? CurrentUsername { get; private set; }

    public async Task<(bool Success, string? ErrorMessage)> LoginAsync(string username, string password)
    {
        var payload = new { username, password };
        var content = CreateJsonContent(payload);

        try
        {
            var response = await HttpClient.PostAsync("Auth/login", content);

            if (!response.IsSuccessStatusCode) return (false, "Usuario o contraseña incorrectos");

            var json = await response.Content.ReadAsStringAsync();
            var data = JsonSerializer.Deserialize<LoginResponse>(json, JsonOptions);

            if (data is null || string.IsNullOrWhiteSpace(data.Token)) return (false, "Respuesta inválida del servidor");

            Token = data.Token;
            CurrentUsername = data.Username;

            return (true, null);
        }
        catch (HttpRequestException)
        {
            return (false, "Error de conexión con el servidor");
        }
        catch (JsonException)
        {
            return (false, "Error interpretando la respuesta");
        }
    }
}
