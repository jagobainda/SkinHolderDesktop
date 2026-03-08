using SkinHolderDesktop.Enums;
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
    private readonly ILoggerService _loggerService = loggerService;

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
            await _loggerService.SendLog($"Error al obtener los items del usuario: {ex.Message}", ELogType.Error);
            return [];
        }
    }
}
