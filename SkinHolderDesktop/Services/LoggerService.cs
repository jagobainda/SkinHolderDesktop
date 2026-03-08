using SkinHolderDesktop.Core;
using SkinHolderDesktop.Enums;
using SkinHolderDesktop.Models;
using System.Diagnostics;
using System.Net.Http;
using System.Text.Json;

namespace SkinHolderDesktop.Services;

public interface ILoggerService
{
    Task SendLog(string message, ELogType logType);
}

public class LoggerService(HttpClient httpClient, JsonSerializerOptions jsonOptions, IAuthSession authSession) : BaseService(httpClient, jsonOptions), ILoggerService
{

    public async Task SendLog(string message, ELogType logType)
    {
        try
        {
            var log = new Logger
            {
                LogDescription = message,
                LogPlaceId = (int)ELogPlace.Desktop,
                LogTypeId = (int)logType,
                UserId = authSession.UserId,
            };

            await HttpClient.PostAsync("/Log/AddLog", CreateJsonContent(log));
        }
        catch
        {
            Debug.WriteLine("Error en el reporte de errores");
        }
    }
}
