using SkinHolderDesktop.Models;
using System.Net.Http;
using System.Text.Json;

namespace SkinHolderDesktop.Services;

public interface ISteamRequestService
{
    Task<SteamItemInfo> MakeRequestAsync(string marketHashName, string country = "ES", int currency = 3, int appId = 730);
}

public class SteamRequestService(ILoggerService loggerService) : ISteamRequestService
{
    private const int MaxRetryAttempts = 5;
    private readonly HttpClient Client = new();
    private const string BaseUrl = "https://steamcommunity.com/market/priceoverview/?country={0}&currency={1}&appid={2}&market_hash_name={3}";

    private readonly ILoggerService _loggerService = loggerService;

    public async Task<SteamItemInfo> MakeRequestAsync(string marketHashName, string country = "ES", int currency = 3, int appId = 730)
    {
        var url = string.Format(BaseUrl, country, currency, appId, marketHashName);
        var attempts = 0;

        while (attempts <= MaxRetryAttempts)
        {
            try
            {
                var response = await Client.GetAsync(url);
                if (response.IsSuccessStatusCode)
                {
                    var responseContent = await response.Content.ReadAsStringAsync();

                    return new SteamItemInfo
                    {
                        HashName = marketHashName,
                        Price = await ExtractPriceFromJson(responseContent),
                        IsError = false,
                        IsWarning = attempts != 0
                    };
                }
            }
            catch (HttpRequestException ex)
            {
                await _loggerService.SendLog($"Error while making request to Steam API: {ex.Message}", 3);
            }

            attempts++;
        }

        return new SteamItemInfo
        {
            HashName = marketHashName,
            Price = -1,
            IsError = true,
            IsWarning = false
        };
    }

    private async Task<decimal> ExtractPriceFromJson(string input)
    {
        if (string.IsNullOrEmpty(input))
        {
            return -1m;
        }

        try
        {
            using var doc = JsonDocument.Parse(input);

            var priceString = doc.RootElement.GetProperty("lowest_price").GetString()?
                .Replace("-", "0")
                .Replace("€", "") ?? string.Empty;

            return decimal.TryParse(priceString, out var price) ? price : -1m;
        }
        catch (JsonException ex)
        {
            await _loggerService.SendLog($"Error parsing JSON response: {ex.Message}", 3);
            return -1m;
        }
    }
}
