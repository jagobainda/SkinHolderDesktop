using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using SkinHolderDesktop.Models;
using SkinHolderDesktop.Services;
using System.Collections.ObjectModel;
using System.Windows;

namespace SkinHolderDesktop.ViewModels;

public partial class RegistrosViewModel : ObservableObject
{
    private readonly IRegistroService _registroService;
    private readonly IUserItemService _userItemService;
    private readonly GlobalViewModel _global;

    public RegistrosViewModel(IRegistroService registroService, IUserItemService userItemService, GlobalViewModel globalViewModel)
    {
        _registroService = registroService;
        _userItemService = userItemService;
        _global = globalViewModel;
        
        ItemPrecios = [];
    }

    public ObservableCollection<ItemPrecio> ItemPrecios { get; }

    [ObservableProperty] private RegistrosViewModel registro;
    [ObservableProperty] private float totalSteam = 0.0f;
    [ObservableProperty] private float totalGamerPay = 0.0f;
    [ObservableProperty] private int progresoSteam = 0;
    [ObservableProperty] private int progresoGamerPay = 0;
    [ObservableProperty] private int totalItems = 0;
    [ObservableProperty] private int itemsNoListados = 0;
    [ObservableProperty] private int itemsWarning = 0;
    [ObservableProperty] private int itemsError = 0;

    [ObservableProperty] private bool detallesSteamEnabled;
    [ObservableProperty] private bool detallesGamerPayEnabled;
    [ObservableProperty] private bool botonesHabilitados = true;

    [RelayCommand]
    private async Task ConsultarAmbosAsync()
    {
        await EjecutarConsulta(async () =>
        {
            //await ObtenerItems();
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
        //var items = await _userItemService.ObtenerItemsUsuario(_global.Token!);
        //if (items.Count == 0) return;

        //Registro.UserId = items.First().UserId;

        //foreach (var item in items)
        //    ItemPrecios.Add(item);

        //TotalItems = ItemPrecios.Count;
    }

    private async Task ObtenerPreciosSteam()
    {
        //DetallesSteamEnabled = false;

        //var hashes = ItemPrecios.Select(x => x.SteamHashName).ToList();
        //var resultados = await _steamService.ObtenerPrecios(hashes);

        //foreach (var resultado in resultados)
        //{
        //    var item = ItemPrecios.FirstOrDefault(i => i.SteamHashName == resultado.HashName);
        //    if (item == null) continue;

        //    item.PrecioSteam = resultado.Precio;
        //    item.Fallo = resultado.Fallo;

        //    ProgresoSteam++;
        //}

        //TotalSteam = ItemPrecios
        //    .Where(i => i.PrecioSteam > 0)
        //    .Sum(i => i.PrecioSteam * i.Cantidad);

        //ItemsWarning = ItemPrecios.Count(i => i.Fallo && i.PrecioSteam > 0);
        //ItemsError = ItemPrecios.Count(i => i.PrecioSteam < 0);

        //DetallesSteamEnabled = true;
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

    }

    [RelayCommand]
    private void MostrarDetallesCSFloatCommand()
    {

    }
}