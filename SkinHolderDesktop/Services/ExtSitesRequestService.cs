using SkinHolderDesktop.Models;
using System.Net.Http;
using System.Text.Json;

namespace SkinHolderDesktop.Services;

public interface IExtSitesRequestService
{
    Task<GamerPayItemInfo[]> MakeGamerPayRequestAsync();
}

public class ExtSitesRequestService(ILoggerService loggerService) : IExtSitesRequestService
{
    private readonly ILoggerService _loggerService = loggerService;

    public async Task<GamerPayItemInfo[]> MakeGamerPayRequestAsync()
    {
        try
        {
            using var client = new HttpClient();

            var response = await client.GetAsync("https://api.gamerpay.gg/prices");
            response.EnsureSuccessStatusCode();
            var jsonContent = await response.Content.ReadAsStringAsync();

            using var doc = JsonDocument.Parse(jsonContent);

            if (doc.RootElement.ValueKind == JsonValueKind.Array)
            {
                List<GamerPayItemInfo> items = [];

                items.AddRange(from element in doc.RootElement.EnumerateArray()
                               let nombre = element.GetProperty("item").GetString()
                               let precio = element.GetProperty("price").GetDouble()
                               select new GamerPayItemInfo { Name = nombre, Price = (decimal)precio });

                return [.. items];
            }
        }
        catch (HttpRequestException e)
        {
            await _loggerService.SendLog($"Error while making request to GamerPay API: {e.Message}", 3);
        }
        catch (JsonException e)
        {
            await _loggerService.SendLog($"Error while parsing JSON from GamerPay API: {e.Message}", 3);
        }
        catch (Exception e)
        {
            await _loggerService.SendLog($"Unexpected error: {e.Message}", 3);
        }

        return [];
    }
}
