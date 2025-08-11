using SkinHolderDesktop.Models;
using SkinHolderDesktop.ViewModels;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text.Json;
using System.Windows;

namespace SkinHolderDesktop.Services;

public interface ILoggerService
{
    Task SendLog(string message, int logType);
}

public class LoggerService(HttpClient httpClient, JsonSerializerOptions jsonOptions, GlobalViewModel globalViewModel) : BaseService(httpClient, jsonOptions), ILoggerService
{
    public GlobalViewModel GlobalViewModel { get; } = globalViewModel;

    public async Task SendLog(string message, int logType)
    {
        try
        {
            var token = GlobalViewModel.Token;
            var userId = GlobalViewModel.UserId;

            HttpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            var log = new Logger
            {
                LogDescription = message,
                LogPlaceId = 1, // Desktop
                LogTypeId = logType,
                UserId = userId,
            };

            var content = new StringContent(JsonSerializer.Serialize(log, JsonOptions), System.Text.Encoding.UTF8, "application/json");

            var response = await HttpClient.PostAsync("/Log/AddLog", content);
        }
        catch
        {
            MessageBox.Show("Error en el reporte de errores", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }
}
