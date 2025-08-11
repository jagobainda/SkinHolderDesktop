using SkinHolderDesktop.Models;
using SkinHolderDesktop.ViewModels;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text.Json;

namespace SkinHolderDesktop.Services;

public interface IRegistroService
{
    Task<Registro> GetLastRegistroAsync();
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

}
