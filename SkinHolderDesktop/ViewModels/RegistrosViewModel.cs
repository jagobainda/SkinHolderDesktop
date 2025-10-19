using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using Microsoft.Extensions.DependencyInjection;
using SkinHolderDesktop.Models;
using SkinHolderDesktop.Services;
using SkinHolderDesktop.ViewModels.Shared;
using SkinHolderDesktop.Views.Shared;
using System.Diagnostics;
using System.IO;
using System.Text.Json;

namespace SkinHolderDesktop.ViewModels;

public partial class RegistrosViewModel(IRegistroService registroService, IItemPrecioService itemPrecioService, IUserItemService userItemService, ISteamRequestService steamRequestService, IExtSitesRequestService extSitesRequestService, ILoggerService loggerService, GlobalViewModel globalViewModel, IMessenger messenger, IServiceProvider serviceProvider) : ObservableObject, IDisposable
{
    private readonly IRegistroService _registroService = registroService;
    private readonly IItemPrecioService _itemPrecioService = itemPrecioService;
    private readonly IUserItemService _userItemService = userItemService;
    private readonly ISteamRequestService _steamRequestService = steamRequestService;
    private readonly IExtSitesRequestService _extSitesRequestService = extSitesRequestService;
    private readonly ILoggerService _loggerService = loggerService;
    private readonly GlobalViewModel _global = globalViewModel;
    private readonly IMessenger _messenger = messenger;
    private readonly IServiceProvider _serviceProvider = serviceProvider;

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
            await _loggerService.SendLog($"Error al consultar precios: {ex.Message}", 3);
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
        _userItems = await _userItemService.GetUserItemsAsync();
    }

    private async Task ObtenerPrecios()
    {
        var gamerPayResponse = await _extSitesRequestService.MakeGamerPayRequestAsync();

        if (gamerPayResponse.Length == 0) await _loggerService.SendLog("Consulta cancelada porque no se han podido obtener los items de GamerPay.", 3);

        foreach (var userItem in _userItems)
        {
            var steamResponse = await _steamRequestService.MakeRequestAsync(userItem.SteamHashName);

            var gamerPayItem = gamerPayResponse.FirstOrDefault(gp => gp.Name == userItem.GamerPayName);

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
            Userid = _global.UserId
        };

        var registroId = await _registroService.CreateRegistroAsync(_registro);

        _itemPrecios.ForEach(ip => ip.Registroid = registroId);

        var successItemPrecios = await _itemPrecioService.CreateItemPreciosAsync(_itemPrecios);

        if (registroId < 1 || !successItemPrecios) return;

        _messenger.Send(new RefreshLastRegistroMessage());
    }

    [RelayCommand]
    private void MostrarDetalles()
    {
        var viewModel = _serviceProvider.GetRequiredService<RegistroDetailsViewModel>();
        viewModel.Initialize(_registro, _userItems, _itemPrecios, "Detalles");

        var detailsWindow = _serviceProvider.GetRequiredService<RegistroDetails>();
        detailsWindow.DataContext = viewModel;

        detailsWindow.ShowDialog();
    }

    [RelayCommand]
    private void HistorialRegistros()
    {
        var viewModel = _serviceProvider.GetRequiredService<RegistroListViewModel>();
        _ = viewModel.InitializeAsync();

        var listWindow = _serviceProvider.GetRequiredService<RegistroList>();
        listWindow.DataContext = viewModel;

        listWindow.ShowDialog();
    }

    [RelayCommand]
    private async Task ExportarJsonAsync()
    {
        var registros = await _registroService.GetRegistrosAsync();

        var jsonString = JsonSerializer.Serialize(registros, new JsonSerializerOptions { WriteIndented = true });

        var saveFileDialog = new Microsoft.Win32.SaveFileDialog
        {
            FileName = $"registros_{_global.CurrentUsername}_{DateTime.Now:yyyy-MM-dd}.json",
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