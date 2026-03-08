using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using SkinHolderDesktop.Core;
using SkinHolderDesktop.Enums;
using SkinHolderDesktop.Models;
using SkinHolderDesktop.Services;
using SkinHolderDesktop.ViewModels.Shared;
using SkinHolderDesktop.Views.Shared;
using System.Diagnostics;
using System.IO;
using System.Text.Json;

namespace SkinHolderDesktop.ViewModels;

public partial class RegistrosViewModel(IRegistroService registroService, IItemPrecioService itemPrecioService, IUserItemService userItemService, ISteamRequestService steamRequestService, 
    IExtSitesRequestService extSitesRequestService, ILoggerService loggerService, IAuthSession authSession, IMessenger messenger, 
    Func<RegistroDetailsViewModel> detailsViewModelFactory, Func<RegistroDetails> detailsWindowFactory, Func<RegistroListViewModel> listViewModelFactory, Func<RegistroList> listWindowFactory) : ObservableObject, IDisposable
{
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

    [ObservableProperty] private bool detallesSteamEnabled = false;
    [ObservableProperty] private bool detallesGamerPayEnabled = false;
    [ObservableProperty] private bool detallesCSFloatEnabled = false;
    [ObservableProperty] private bool botonesHabilitados = true;

    [RelayCommand]
    private async Task ConsultarAsync()
    {
        await EjecutarConsulta(async () =>
        {
            await ObtenerUserItems();

            TotalItems = _userItems.Count;

            await ObtenerPrecios();
        });
    }

    private async Task EjecutarConsulta(Func<Task> accion)
    {
        BotonesHabilitados = false;
        DetallesSteamEnabled = false;
        DetallesGamerPayEnabled = false;
        DetallesCSFloatEnabled = false;

        try
        {
            Reset();
            await accion();
            await GuardarRegistro();
        }
        catch (Exception ex)
        {
            await loggerService.SendLog($"Error al consultar precios: {ex.Message}", ELogType.Error);
        }
        finally
        {
            BotonesHabilitados = true;
            DetallesSteamEnabled = TotalSteam > 0;
            DetallesGamerPayEnabled = TotalGamerPay > 0;
            DetallesCSFloatEnabled = TotalCSFloat > 0;
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
        _itemPrecios = [];
        _registro = new Registro();
    }

    private async Task ObtenerUserItems()
    {
        _userItems = await userItemService.GetUserItemsAsync();
    }

    private async Task ObtenerPrecios()
    {
        var gamerPayResponse = await extSitesRequestService.MakeGamerPayRequestAsync();

        if (gamerPayResponse.Length == 0) await loggerService.SendLog("Consulta cancelada porque no se han podido obtener los items de GamerPay.", ELogType.Error);

        foreach (var userItem in _userItems)
        {
            var steamHashName = string.IsNullOrWhiteSpace(userItem.SteamHashName) ? userItem.ItemName : userItem.SteamHashName;
            var gamerPayLookupName = string.IsNullOrWhiteSpace(userItem.GamerPayName) ? userItem.ItemName : userItem.GamerPayName;

            var steamResponse = await steamRequestService.MakeRequestAsync(steamHashName);

            var gamerPayItem = gamerPayResponse.FirstOrDefault(gp => string.Equals(gp.Name?.Trim(), gamerPayLookupName?.Trim(), StringComparison.OrdinalIgnoreCase));

            if (steamResponse.IsWarning) ItemsWarningSteam++;

            if (steamResponse.IsError) ItemsErrorSteam++;

            if (steamResponse.Price > 0) TotalSteam += steamResponse.Price * userItem.Cantidad;

            if (gamerPayItem != null) TotalGamerPay += gamerPayItem.Price * userItem.Cantidad;

            // TODO: Implement CSFloat

            _itemPrecios.Add(new ItemPrecio
            {
                Preciosteam = Math.Max(steamResponse.Price, 0.0m),
                Preciogamerpay = gamerPayItem?.Price ?? 0.0m,
                Preciocsfloat = 0.0m,
                Useritemid = userItem.Useritemid
            });

            ProgresoSteam++;
            ProgresoGamerPay++;
            ProgresoCSFloat++;

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
            Userid = authSession.UserId
        };

        var registroId = await registroService.CreateRegistroAsync(_registro);

        _itemPrecios.ForEach(ip => ip.Registroid = registroId);

        var successItemPrecios = await itemPrecioService.CreateItemPreciosAsync(_itemPrecios);

        if (registroId < 1 || !successItemPrecios) return;

        messenger.Send(new RefreshLastRegistroMessage());
    }

    [RelayCommand]
    private void MostrarDetalles()
    {
        var viewModel = detailsViewModelFactory();
        viewModel.Initialize(_registro, _userItems, _itemPrecios, "Detalles");

        var detailsWindow = detailsWindowFactory();
        detailsWindow.DataContext = viewModel;

        detailsWindow.ShowDialog();
    }

    [RelayCommand]
    private void HistorialRegistros()
    {
        var viewModel = listViewModelFactory();
        _ = viewModel.InitializeAsync();

        var listWindow = listWindowFactory();
        listWindow.DataContext = viewModel;

        listWindow.ShowDialog();
    }

    [RelayCommand]
    private async Task ExportarJsonAsync()
    {
        var registros = await registroService.GetRegistrosAsync();

        var jsonString = JsonSerializer.Serialize(registros, new JsonSerializerOptions { WriteIndented = true });

        var saveFileDialog = new Microsoft.Win32.SaveFileDialog
        {
            FileName = $"registros_{authSession.CurrentUsername}_{DateTime.Now:yyyy-MM-dd}.json",
            Filter = "JSON files (*.json)|*.json|All files (*.*)|*.*"
        };

        if (saveFileDialog.ShowDialog() != true) return;

        var filePath = saveFileDialog.FileName;

        await File.WriteAllTextAsync(filePath, jsonString);

        var folderPath = Path.GetDirectoryName(filePath);

        if (folderPath != null) Process.Start("explorer.exe", folderPath);
    }

    public void Dispose()
    {
        if (_disposed) return;

        _disposed = true;
        GC.SuppressFinalize(this);
    }
}