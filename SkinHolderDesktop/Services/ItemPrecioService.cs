using SkinHolderDesktop.Models;
using System.Net.Http;
using System.Text;
using System.Text.Json;

namespace SkinHolderDesktop.Services;

public interface IItemPrecioService
{
    Task<List<ItemPrecio>> GetItemPreciosAsync(long registroId);
    Task<bool> CreateItemPreciosAsync(List<ItemPrecio> itemPrecios);
    Task<bool> DeleteItemPreciosAsync(long registroId);
}

public class ItemPrecioService(HttpClient httpClient, JsonSerializerOptions jsonOptions) : BaseService(httpClient, jsonOptions), IItemPrecioService
{

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
        catch
        {
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

            return response.IsSuccessStatusCode;
        }
        catch
        {
            return false;
        }
    }

    public async Task<bool> DeleteItemPreciosAsync(long registroId)
    {
        try
        {
            var response = await HttpClient.DeleteAsync($"/ItemPrecio/{registroId}");

            return response.IsSuccessStatusCode;
        }
        catch
        {
            return false;
        }
    }
}