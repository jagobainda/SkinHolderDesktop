using SkinHolderDesktop.Models.Auth;
using System.Net.Http;
using System.Text.Json;
using System.Text;

namespace SkinHolderDesktop.Services;

public interface ILoginService
{
    Task<(bool Success, string? ErrorMessage)> LoginAsync(string username, string password);
    string? Token { get; }
    string? CurrentUsername { get; }
}

public class LoginService(JsonSerializerOptions jsonOptions) : ILoginService
{
    private readonly HttpClient _httpClient = new();
    private readonly JsonSerializerOptions _jsonOptions = jsonOptions;

    public string? Token { get; private set; }
    public string? CurrentUsername { get; private set; }

    private const string BaseUrl = "https://shapi.jagoba.dev";

    public async Task<(bool Success, string? ErrorMessage)> LoginAsync(string username, string password)
    {
        var payload = new { username, password };
        var content = new StringContent(JsonSerializer.Serialize(payload), Encoding.UTF8, "application/json");

        try
        {
            var response = await _httpClient.PostAsync($"{BaseUrl}/Auth/login", content);

            if (!response.IsSuccessStatusCode) return (false, "Usuario o contraseña incorrectos");

            var json = await response.Content.ReadAsStringAsync();

            var data = JsonSerializer.Deserialize<LoginResponse>(json, _jsonOptions);

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