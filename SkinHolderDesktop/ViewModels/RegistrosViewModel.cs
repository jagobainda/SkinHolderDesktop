using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using SkinHolderDesktop.Models;
using SkinHolderDesktop.Services;

namespace SkinHolderDesktop.ViewModels;

public partial class RegistrosViewModel(IRegistroService registroService, IUserItemService userItemService, ISteamRequestService steamRequestService, ILoggerService loggerService, GlobalViewModel globalViewModel, IMessenger messenger) : ObservableObject, IDisposable
{
    private readonly IRegistroService _registroService = registroService;
    private readonly IUserItemService _userItemService = userItemService;
    private readonly ISteamRequestService _steamRequestService = steamRequestService;
    private readonly ILoggerService _loggerService = loggerService;
    private readonly GlobalViewModel _global = globalViewModel;
    private readonly IMessenger _messenger = messenger;

    private List<UserItem> _userItems = [];

    private Registro _registro = new();

    private List<ItemPrecio> _itemPrecios = [];

    private bool _disposed;

    [ObservableProperty] private decimal totalSteam = 0.0m;
    [ObservableProperty] private decimal totalGamerPay = 0.0m;
    [ObservableProperty] private decimal totalCSFloat = 0.0m;
    [ObservableProperty] private int progresoSteam = 0;
    [ObservableProperty] private int progresoGamerPay = 0;
    [ObservableProperty] private int progresoCSFloat = 0;
    [ObservableProperty] private int totalItems = 0;
    [ObservableProperty] private int itemsNoListadosGamerPay = 0;
    [ObservableProperty] private int itemsWarningSteam = 0;
    [ObservableProperty] private int itemsErrorSteam = 0;

    [ObservableProperty] private bool detallesSteamEnabled;
    [ObservableProperty] private bool detallesGamerPayEnabled;
    [ObservableProperty] private bool detallesCSFloatEnabled;
    [ObservableProperty] private bool botonesHabilitados = true;

    [RelayCommand]
    private async Task ConsultarAmbosAsync()
    {
        await EjecutarConsulta(async () =>
        {
            await ObtenerUserItems();

            TotalItems = _userItems.Count;

            await ObtenerPrecios();

            await GuardarRegistro();
        });
    }

    private async Task EjecutarConsulta(Func<Task> accion)
    {
        BotonesHabilitados = false;

        try
        {
            Reset();
            await accion();
            await GuardarRegistro();
        }
        catch (Exception ex)
        {
            await _loggerService.SendLog($"Error al consultar precios: {ex.Message}", 3);
        }
        finally
        {
            BotonesHabilitados = true;
        }
    }

    private void Reset()
    {
        TotalSteam = 0.0m;
        TotalGamerPay = 0.0m;
        TotalCSFloat = 0.0m;
        ProgresoSteam = 0;
        ProgresoGamerPay = 0;
        ProgresoCSFloat = 0;
        TotalItems = 0;
        ItemsNoListadosGamerPay = 0;
        ItemsWarningSteam = 0;
        ItemsErrorSteam = 0;
        _itemPrecios.Clear();
        _registro = new Registro();
    }

    private async Task ObtenerUserItems()
    {
        _userItems = await _userItemService.GetUserItemsAsync();
    }

    private async Task ObtenerPrecios()
    {
        foreach (var userItem in _userItems)
        {
            var steamResponse = await _steamRequestService.MakeRequestAsync(userItem.SteamHashName);

            if (steamResponse.IsWarning) ItemsWarningSteam++;

            if (steamResponse.IsError) ItemsErrorSteam++;

            if (steamResponse.Price > 0) TotalSteam += steamResponse.Price * userItem.Cantidad;

            // TODO: Implementar GamerPay y CSFloat

            _itemPrecios.Add(new ItemPrecio
            {
                Preciosteam = steamResponse.Price,
                Preciogamerpay = 0.0m,
                Preciocsfloat = 0.0m,
                Useritemid = userItem.Useritemid
            });

            ProgresoSteam++;
            //ProgresoGamerPay++;
            //ProgresoCSFloat++;

            await Task.Delay(3000);
        }
    }

    private async Task GuardarRegistro()
    {
        _registro = new Registro
        {
            Fechahora = DateTime.Now,
            Totalsteam = TotalSteam,
            Totalgamerpay = TotalGamerPay,
            Totalcsfloat = TotalCSFloat,
            Userid = _global.UserId
        };

        var success = await _registroService.CreateRegistroAsync(_registro);

        if (success) _messenger.Send(new RefreshLastRegistroMessage());

        // TODO: Save ItemPrecios to database
    }

    [RelayCommand]
    private void MostrarDetallesSteam()
    {
        // TODO
    }

    [RelayCommand]
    private void MostrarDetallesGamerPay()
    {
        // TODO
    }

    [RelayCommand]
    private void MostrarDetallesCSFloat()
    {
        // TODO
    }

    public void Dispose()
    {
        if (_disposed) return;

        _disposed = true;
        GC.SuppressFinalize(this);
    }
}