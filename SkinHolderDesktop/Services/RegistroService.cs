using SkinHolderDesktop.Models;
using System.Net.Http;
using System.Text.Json;

namespace SkinHolderDesktop.Services;

public interface IRegistroService
{
    Task<Registro> GetLastRegistroAsync();
    Task<List<Registro>> GetRegistrosAsync();
    Task<long> CreateRegistroAsync(Registro registroDto);
    Task<bool> DeleteRegistroAsync(long registroId);
}

public class RegistroService(HttpClient httpClient, JsonSerializerOptions jsonOptions) : BaseService(httpClient, jsonOptions), IRegistroService
{

    public async Task<Registro> GetLastRegistroAsync()
    {
        try
        {
            var response = await HttpClient.GetAsync("/Registros/GetLastRegistro");

            response.EnsureSuccessStatusCode();

            var contentStream = await response.Content.ReadAsStreamAsync();
            var registro = await JsonSerializer.DeserializeAsync<Registro>(contentStream, JsonOptions);

            return registro!;
        }
        catch
        {
            return new Registro();
        }
    }

    public async Task<List<Registro>> GetRegistrosAsync()
    {
        try
        {
            var response = await HttpClient.GetAsync("/Registros");

            response.EnsureSuccessStatusCode();

            var contentStream = await response.Content.ReadAsStreamAsync();
            var registros = await JsonSerializer.DeserializeAsync<List<Registro>>(contentStream, JsonOptions);

            return registros ?? [];
        }
        catch
        {
            return [];
        }
    }

    public async Task<long> CreateRegistroAsync(Registro registro)
    {
        try
        {
            var content = CreateJsonContent(registro);

            var response = await HttpClient.PostAsync("/Registros", content);

            if (!response.IsSuccessStatusCode) return 0;

            var registroIdString = await response.Content.ReadAsStringAsync();

            return long.TryParse(registroIdString, out var registroId) ? registroId : 0;
        }
        catch
        {
            return 0;
        }
    }

    public async Task<bool> DeleteRegistroAsync(long registroId)
    {
        try
        {
            var response = await HttpClient.DeleteAsync($"/Registros?registroId={registroId}");

            return response.IsSuccessStatusCode;
        }
        catch
        {
            return false;
        }
    }
}