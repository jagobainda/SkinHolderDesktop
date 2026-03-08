using SkinHolderDesktop.Models;
using System.Net.Http;
using System.Text.Json;
using SkinHolderDesktop.Enums;

namespace SkinHolderDesktop.Services;

public interface IRegistroService
{
    Task<Registro> GetLastRegistroAsync();
    Task<List<Registro>> GetRegistrosAsync();
    Task<long> CreateRegistroAsync(Registro registroDto);
    Task<bool> DeleteRegistroAsync(long registroId);
}

public class RegistroService(HttpClient httpClient, JsonSerializerOptions jsonOptions, ILoggerService loggerService) : BaseService(httpClient, jsonOptions), IRegistroService
{
    private readonly ILoggerService _loggerService = loggerService;

    public async Task<Registro> GetLastRegistroAsync()
    {
        try
        {
            var response = await HttpClient.GetAsync("/Registros/GetLastRegistro");

            response.EnsureSuccessStatusCode();

            var contentStream = await response.Content.ReadAsStreamAsync();
            var registro = await JsonSerializer.DeserializeAsync<Registro>(contentStream, JsonOptions);

            return registro!;
        }
        catch (HttpRequestException ex)
        {
            await _loggerService.SendLog($"Error de conexión obteniendo último registro: {ex.Message}", ELogType.Error);
            return new Registro();
        }
        catch (JsonException ex)
        {
            await _loggerService.SendLog($"Error parseando último registro: {ex.Message}", ELogType.Error);
            return new Registro();
        }
        catch (Exception ex)
        {
            await _loggerService.SendLog($"Error inesperado obteniendo último registro: {ex.Message}", ELogType.Error);
            return new Registro();
        }
    }

    public async Task<List<Registro>> GetRegistrosAsync()
    {
        try
        {
            var response = await HttpClient.GetAsync("/Registros");

            response.EnsureSuccessStatusCode();

            var contentStream = await response.Content.ReadAsStreamAsync();
            var registros = await JsonSerializer.DeserializeAsync<List<Registro>>(contentStream, JsonOptions);

            return registros ?? [];
        }
        catch (HttpRequestException ex)
        {
            await _loggerService.SendLog($"Error de conexión obteniendo registros: {ex.Message}", ELogType.Error);
            return [];
        }
        catch (JsonException ex)
        {
            await _loggerService.SendLog($"Error parseando listado de registros: {ex.Message}", ELogType.Error);
            return [];
        }
        catch (Exception ex)
        {
            await _loggerService.SendLog($"Error inesperado obteniendo registros: {ex.Message}", ELogType.Error);
            return [];
        }
    }

    public async Task<long> CreateRegistroAsync(Registro registro)
    {
        try
        {
            var content = CreateJsonContent(registro);

            var response = await HttpClient.PostAsync("/Registros", content);

            if (!response.IsSuccessStatusCode)
            {
                await _loggerService.SendLog($"No se pudo crear el registro. Código: {(int)response.StatusCode}", ELogType.Warning);
                return 0;
            }

            var registroIdString = await response.Content.ReadAsStringAsync();

            return long.TryParse(registroIdString, out var registroId) ? registroId : 0;
        }
        catch (HttpRequestException ex)
        {
            await _loggerService.SendLog($"Error de conexión creando registro: {ex.Message}", ELogType.Error);
            return 0;
        }
        catch (Exception ex)
        {
            await _loggerService.SendLog($"Error inesperado creando registro: {ex.Message}", ELogType.Error);
            return 0;
        }
    }

    public async Task<bool> DeleteRegistroAsync(long registroId)
    {
        try
        {
            var response = await HttpClient.DeleteAsync($"/Registros?registroId={registroId}");

            if (!response.IsSuccessStatusCode)
            {
                await _loggerService.SendLog($"No se pudo borrar el registro {registroId}. Código: {(int)response.StatusCode}", ELogType.Warning);
            }

            return response.IsSuccessStatusCode;
        }
        catch (HttpRequestException ex)
        {
            await _loggerService.SendLog($"Error de conexión borrando registro {registroId}: {ex.Message}", ELogType.Error);
            return false;
        }
        catch (Exception ex)
        {
            await _loggerService.SendLog($"Error inesperado borrando registro {registroId}: {ex.Message}", ELogType.Error);
            return false;
        }
    }
}