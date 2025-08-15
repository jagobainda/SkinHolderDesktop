using SkinHolderDesktop.Models;
using SkinHolderDesktop.ViewModels;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text.Json;
using System.Windows;

namespace SkinHolderDesktop.Services;

public interface IUserItemService
{
    Task<List<UserItem>> GetUserItemsAsync();
    Task AddUserItemAsync(UserItem userItem);
    Task<bool> UpdateUserItemAsync(UserItem userItem, int cantidad);
}

public class UserItemService(HttpClient httpClient, JsonSerializerOptions jsonOptions, GlobalViewModel globalViewModel, ILoggerService loggerService) : BaseService(httpClient, jsonOptions), IUserItemService
{
    public GlobalViewModel GlobalViewModel { get; } = globalViewModel;
    public ILoggerService LoggerService { get; } = loggerService;

    public async Task<List<UserItem>> GetUserItemsAsync()
    {
        try
        {
            var token = GlobalViewModel.Token;

            HttpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            var response = await HttpClient.GetAsync("/UserItems");

            response.EnsureSuccessStatusCode();
            var contentStream = await response.Content.ReadAsStreamAsync();
            var userItems = await JsonSerializer.DeserializeAsync<List<UserItem>>(contentStream, JsonOptions);

            return userItems!;
        }
        catch (Exception ex) 
        {
            await LoggerService.SendLog($"Error al obtener los items del usuario: {ex.Message}", 3);
            return [];
        }
    }

    public async Task AddUserItemAsync(UserItem userItem)
    {
        try
        {
            // TODO
        }
        catch (Exception ex)
        {
            await LoggerService.SendLog($"Error al agregar el item del usuario: {ex.Message}", 3);
        }
    }

    public async Task<bool> UpdateUserItemAsync(UserItem userItem, int cantidad)
    {
        try
        {
            var token = GlobalViewModel.Token;
            
            HttpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

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
            await LoggerService.SendLog($"Error al actualizar el item del usuario: {ex.Message}", 3);
            return false;
        }
    }
}
