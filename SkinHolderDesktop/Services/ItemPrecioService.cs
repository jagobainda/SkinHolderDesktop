using SkinHolderDesktop.Models;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using SkinHolderDesktop.Enums;

namespace SkinHolderDesktop.Services;

public interface IItemPrecioService
{
    Task<List<ItemPrecio>> GetItemPreciosAsync(long registroId);
    Task<bool> CreateItemPreciosAsync(List<ItemPrecio> itemPrecios);
    Task<bool> DeleteItemPreciosAsync(long registroId);
}

public class ItemPrecioService(HttpClient httpClient, JsonSerializerOptions jsonOptions, ILoggerService loggerService) : BaseService(httpClient, jsonOptions), IItemPrecioService
{
    private readonly ILoggerService _loggerService = loggerService;

    public async Task<List<ItemPrecio>> GetItemPreciosAsync(long registroId)
    {
        try
        {
            var response = await HttpClient.GetAsync($"/ItemPrecio/{registroId}");

            response.EnsureSuccessStatusCode();

            var contentStream = await response.Content.ReadAsStreamAsync();
            var itemPrecios = await JsonSerializer.DeserializeAsync<List<ItemPrecio>>(contentStream, JsonOptions);

            return itemPrecios ?? [];
        }
        catch (HttpRequestException ex)
        {
            await _loggerService.SendLog($"Error de conexión obteniendo precios del registro {registroId}: {ex.Message}", ELogType.Error);
            return [];
        }
        catch (JsonException ex)
        {
            await _loggerService.SendLog($"Error parseando precios del registro {registroId}: {ex.Message}", ELogType.Error);
            return [];
        }
        catch (Exception ex)
        {
            await _loggerService.SendLog($"Error inesperado obteniendo precios del registro {registroId}: {ex.Message}", ELogType.Error);
            return [];
        }
    }

    public async Task<bool> CreateItemPreciosAsync(List<ItemPrecio> itemPrecios)
    {
        try
        {
            var jsonContent = JsonSerializer.Serialize(itemPrecios, JsonOptions);
            var content = new StringContent(jsonContent, Encoding.UTF8, "application/json");

            var response = await HttpClient.PostAsync("/ItemPrecio", content);

            if (!response.IsSuccessStatusCode)
            {
                await _loggerService.SendLog($"No se pudieron crear precios del item. Código: {(int)response.StatusCode}", ELogType.Warning);
            }

            return response.IsSuccessStatusCode;
        }
        catch (HttpRequestException ex)
        {
            await _loggerService.SendLog($"Error de conexión creando precios de item: {ex.Message}", ELogType.Error);
            return false;
        }
        catch (Exception ex)
        {
            await _loggerService.SendLog($"Error inesperado creando precios de item: {ex.Message}", ELogType.Error);
            return false;
        }
    }

    public async Task<bool> DeleteItemPreciosAsync(long registroId)
    {
        try
        {
            var response = await HttpClient.DeleteAsync($"/ItemPrecio/{registroId}");

            if (!response.IsSuccessStatusCode)
            {
                await _loggerService.SendLog($"No se pudieron borrar precios del registro {registroId}. Código: {(int)response.StatusCode}", ELogType.Warning);
            }

            return response.IsSuccessStatusCode;
        }
        catch (HttpRequestException ex)
        {
            await _loggerService.SendLog($"Error de conexión borrando precios del registro {registroId}: {ex.Message}", ELogType.Error);
            return false;
        }
        catch (Exception ex)
        {
            await _loggerService.SendLog($"Error inesperado borrando precios del registro {registroId}: {ex.Message}", ELogType.Error);
            return false;
        }
    }
}