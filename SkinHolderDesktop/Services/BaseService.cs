using System.Net.Http;
using System.Text;
using System.Text.Json;

namespace SkinHolderDesktop.Services;

public abstract class BaseService(HttpClient httpClient, JsonSerializerOptions jsonOptions)
{
    protected readonly HttpClient HttpClient = httpClient;
    protected readonly JsonSerializerOptions JsonOptions = jsonOptions;

    protected StringContent CreateJsonContent<T>(T payload)
    {
        return new StringContent(
            JsonSerializer.Serialize(payload, JsonOptions),
            Encoding.UTF8,
            "application/json"
        );
    }
}
