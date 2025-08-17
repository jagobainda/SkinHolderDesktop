using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using SkinHolderDesktop.Models;
using SkinHolderDesktop.Services;
using SkinHolderDesktop.Views.Partials;
using System.Collections.ObjectModel;
using System.Windows;

namespace SkinHolderDesktop.ViewModels;

public partial class RegistrosViewModel(IRegistroService registroService, IUserItemService userItemService, ISteamRequestService steamRequestService, ILoggerService loggerService, GlobalViewModel globalViewModel) : ObservableObject, IDisposable
{
    private readonly IRegistroService _registroService = registroService;
    private readonly IUserItemService _userItemService = userItemService;
    private readonly ISteamRequestService _steamRequestService = steamRequestService;
    private readonly ILoggerService _loggerService = loggerService;
    private readonly GlobalViewModel _global = globalViewModel;

    private List<UserItem> _userItems = [];

    private Registro _registro = new();

    private List<ItemPrecio> _itemPrecios = [];

    private bool _disposed;

    [ObservableProperty] private double totalSteam = 0.0;
    [ObservableProperty] private double totalGamerPay = 0.0;
    [ObservableProperty] private double totalCSFloat = 0.0;
    [ObservableProperty] private int progresoSteam = 0;
    [ObservableProperty] private int progresoGamerPay = 0;
    [ObservableProperty] private int totalItems = 0;
    [ObservableProperty] private int itemsNoListados = 0;
    [ObservableProperty] private int itemsWarning = 0;
    [ObservableProperty] private int itemsError = 0;

    [ObservableProperty] private bool detallesSteamEnabled;
    [ObservableProperty] private bool detallesGamerPayEnabled;
    [ObservableProperty] private bool detallesCSFloatEnabled;
    [ObservableProperty] private bool botonesHabilitados = true;

    [RelayCommand]
    private async Task ConsultarAmbosAsync()
    {
        await EjecutarConsulta(async () =>
        {
            await ObtenerItems();
            //await Task.WhenAll(ObtenerPreciosSteam(), ObtenerPreciosGamerPay());
            //Registro.RegistroTypeId = ERegistroType.All.GetHashCode();
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
            // TODO: loggear o mostrar error
        }
        finally
        {
            BotonesHabilitados = true;
        }
    }

    private void Reset()
    {
        //Registro = new RegistrosViewModel { FechaHora = DateTime.Now };
        //ItemPrecios.Clear();
        //TotalSteam = TotalGamerPay = 0;
        //ProgresoSteam = ProgresoGamerPay = 0;
        //DetallesSteamEnabled = DetallesGamerPayEnabled = false;
        //ItemsError = ItemsWarning = ItemsNoListados = 0;
    }

    private async Task ObtenerItems()
    {
        _userItems = await _userItemService.GetUserItemsAsync();
    }

    private async Task ObtenerPreciosSteam()
    {
        
    }

    private async Task ObtenerPreciosGamerPay()
    {
        //DetallesGamerPayEnabled = false;

        //var precios = await _registroService.ObtenerPreciosGamerPay(_global.Token!);

        //TotalGamerPay = 0;
        //ItemsNoListados = 0;

        //foreach (var item in ItemPrecios)
        //{
        //    if (precios.TryGetValue(item.GamerPayNombre, out var precio))
        //    {
        //        item.PrecioGamerPay = precio;
        //        TotalGamerPay += precio * item.Cantidad;
        //    }
        //    else
        //    {
        //        ItemsNoListados++;
        //    }

        //    ProgresoGamerPay++;
        //}

        //DetallesGamerPayEnabled = true;
    }

    private async Task GuardarRegistro()
    {
        //Registro.TotalSteam = TotalSteam;
        //Registro.TotalGamerPay = TotalGamerPay;

        //var registroId = await _registroService.CrearRegistro(Registro, _global.Token!);
        //ItemPrecios.ToList().ForEach(i => i.RegistroId = registroId);

        //await _registroService.GuardarItems(ItemPrecios.ToList(), _global.Token!);
    }

    [RelayCommand]
    private void MostrarDetallesSteamCommand()
    {
        MessageBox.Show("Detalles de Steam no implementados aún.", "Información", MessageBoxButton.OK, MessageBoxImage.Information);
    }

    [RelayCommand]
    private void MostrarDetallesGamerPayCommand()
    {
        MessageBox.Show("Detalles de GamerPay no implementados aún.", "Información", MessageBoxButton.OK, MessageBoxImage.Information);
    }

    [RelayCommand]
    private void MostrarDetallesCSFloatCommand()
    {
        MessageBox.Show("Detalles de CSFloat no implementados aún.", "Información", MessageBoxButton.OK, MessageBoxImage.Information);
    }

    public void Dispose()
    {
        if (_disposed) return;

        //ItemPrecios.Clear();

        _disposed = true;
        GC.SuppressFinalize(this);
    }
}