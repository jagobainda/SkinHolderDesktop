using SkinHolderDesktop.Models;
using System.Net.Http;
using System.Text.Json;

namespace SkinHolderDesktop.Services;

public interface IItemsService
{
    Task<List<Item>> GetItemsAsync();
}

public class ItemsService(HttpClient httpClient, JsonSerializerOptions jsonOptions, ILoggerService loggerService) : BaseService(httpClient, jsonOptions), IItemsService
{
    public ILoggerService LoggerService { get; } = loggerService;

    public async Task<List<Item>> GetItemsAsync()
    {
        try
        {
            var response = await HttpClient.GetAsync("/Items");

            response.EnsureSuccessStatusCode();

            var contentStream = await response.Content.ReadAsStreamAsync();
            var items = await JsonSerializer.DeserializeAsync<List<Item>>(contentStream, JsonOptions);
            return items!;
        }
        catch (Exception ex)
        {
            await LoggerService.SendLog($"Error al obtener los items del usuario: {ex.Message}", 3);
            return [];
        }
    }
}
