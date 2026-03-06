using SkinHolderDesktop.Core;
using SkinHolderDesktop.Models;
using System.Net.Http;
using System.Text.Json;
using System.Windows;

namespace SkinHolderDesktop.Services;

public interface ILoggerService
{
    Task SendLog(string message, int logType);
}

public class LoggerService(HttpClient httpClient, JsonSerializerOptions jsonOptions, IAuthSession authSession) : BaseService(httpClient, jsonOptions), ILoggerService
{

    public async Task SendLog(string message, int logType)
    {
        try
        {
            var log = new Logger
            {
                LogDescription = message,
                LogPlaceId = 1, // Desktop
                LogTypeId = logType,
                UserId = authSession.UserId,
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
