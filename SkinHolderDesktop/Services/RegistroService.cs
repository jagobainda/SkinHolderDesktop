using SkinHolderDesktop.Models;
using SkinHolderDesktop.ViewModels;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;

namespace SkinHolderDesktop.Services;

public interface IRegistroService
{
    Task<Registro> GetLastRegistroAsync();
    Task<List<Registro>> GetRegistrosAsync();
    Task<long> CreateRegistroAsync(Registro registroDto);
    Task<bool> DeleteRegistroAsync(long registroId);
}

public class RegistroService(HttpClient httpClient, JsonSerializerOptions jsonOptions, GlobalViewModel globalViewModel) : BaseService(httpClient, jsonOptions), IRegistroService
{
    public GlobalViewModel GlobalViewModel { get; } = globalViewModel;

    public async Task<Registro> GetLastRegistroAsync()
    {
        try
        {
            var token = GlobalViewModel.Token;

            HttpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

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
            var token = GlobalViewModel.Token;

            HttpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

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
            var token = GlobalViewModel.Token;

            HttpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            var jsonContent = JsonSerializer.Serialize(registro, JsonOptions);
            var content = new StringContent(jsonContent, Encoding.UTF8, "application/json");

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
            var token = GlobalViewModel.Token;

            HttpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            var response = await HttpClient.DeleteAsync($"/Registros?registroId={registroId}");

            return response.IsSuccessStatusCode;
        }
        catch
        {
            return false;
        }
    }
}