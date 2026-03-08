using SkinHolderDesktop.Enums;
using SkinHolderDesktop.Models;
using System.Net.Http;
using System.Text.Json;

namespace SkinHolderDesktop.Services;

public interface IUserItemService
{
    Task<List<UserItem>> GetUserItemsAsync();
    Task<bool> AddUserItemAsync(UserItem userItem);
    Task<bool> UpdateUserItemAsync(UserItem userItem, int cantidad);
}

public class UserItemService(HttpClient httpClient, JsonSerializerOptions jsonOptions, ILoggerService loggerService) : BaseService(httpClient, jsonOptions), IUserItemService
{
    private readonly ILoggerService _loggerService = loggerService;

    public async Task<List<UserItem>> GetUserItemsAsync()
    {
        try
        {
            var response = await HttpClient.GetAsync("/UserItems");

            response.EnsureSuccessStatusCode();
            var contentStream = await response.Content.ReadAsStreamAsync();
            var userItems = await JsonSerializer.DeserializeAsync<List<UserItem>>(contentStream, JsonOptions);

            return userItems!;
        }
        catch (Exception ex)
        {
            await _loggerService.SendLog($"Error al obtener los items del usuario: {ex.Message}", ELogType.Error);
            return [];
        }
    }

    public async Task<bool> AddUserItemAsync(UserItem userItem)
    {
        try
        {
            var content = CreateJsonContent(userItem);

            var response = await HttpClient.PostAsync("/UserItems", content);

            return response.IsSuccessStatusCode;
        }
        catch (Exception ex)
        {
            await _loggerService.SendLog($"Error al agregar el item del usuario: {ex.Message}", ELogType.Error);
            return false;
        }
    }

    public async Task<bool> UpdateUserItemAsync(UserItem userItem, int cantidad)
    {
        try
        {
            var updatedUserItem = new UserItem
            {
                Useritemid = userItem.Useritemid,
                Itemid = userItem.Itemid,
                Userid = userItem.Userid,
                Cantidad = cantidad,
                ItemName = userItem.ItemName
            };

            var content = CreateJsonContent(updatedUserItem);

            var response = await HttpClient.PutAsync($"/UserItems", content);

            return response.IsSuccessStatusCode;
        }
        catch (Exception ex)
        {
            await _loggerService.SendLog($"Error al actualizar el item del usuario: {ex.Message}", ELogType.Error);
            return false;
        }
    }
}
