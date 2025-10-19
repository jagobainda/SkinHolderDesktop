using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Extensions.DependencyInjection;
using SkinHolderDesktop.Models;
using SkinHolderDesktop.Services;
using SkinHolderDesktop.Views.Shared;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows.Data;

namespace SkinHolderDesktop.ViewModels.Shared;

public class RegistroItem
{
    public long Registroid { get; set; }
    public DateTime Fechahora { get; set; }
    public decimal Totalsteam { get; set; }
    public decimal Totalgamerpay { get; set; }
    public decimal Totalcsfloat { get; set; }

    public string FechaFormatted => Fechahora.ToString("dd/MM/yyyy HH:mm");
    public string SteamFormatted => $"{Totalsteam:F2} €";
    public string GamerPayFormatted => $"{Totalgamerpay:F2} €";
    public string CSFloatFormatted => $"{Totalcsfloat:F2} €";
}

public enum RegistroSortColumn
{
    None,
    Fecha,
    Steam,
    GamerPay,
    CSFloat
}

public partial class RegistroListViewModel : ObservableObject
{
    private readonly IRegistroService _registroService;
    private readonly IItemPrecioService _itemPrecioService;
    private readonly IUserItemService _userItemService;
    private readonly IServiceProvider _serviceProvider;

    [ObservableProperty] private ObservableCollection<RegistroItem> registros;
    [ObservableProperty] private string title = "Historial de Registros";
    [ObservableProperty] private bool isLoading = false;

    [ObservableProperty] private RegistroSortColumn currentSortColumn = RegistroSortColumn.None;
    [ObservableProperty] private ListSortDirection currentSortDirection = ListSortDirection.Descending;

    [ObservableProperty] private string fechaSortIndicator = "▼";
    [ObservableProperty] private string steamSortIndicator = "";
    [ObservableProperty] private string gamerPaySortIndicator = "";
    [ObservableProperty] private string csFloatSortIndicator = "";

    private ICollectionView? _registrosView;
    private List<Registro> _fullRegistros = [];

    public RegistroListViewModel(IRegistroService registroService, IItemPrecioService itemPrecioService, IUserItemService userItemService, IServiceProvider serviceProvider)
    {
        _registroService = registroService;
        _itemPrecioService = itemPrecioService;
        _userItemService = userItemService;
        _serviceProvider = serviceProvider;

        Registros = [];
    }

    public async Task InitializeAsync()
    {
        IsLoading = true;

        try
        {
            _fullRegistros = await _registroService.GetRegistrosAsync();

            var registroItems = _fullRegistros.Select(r => new RegistroItem
            {
                Registroid = r.Registroid,
                Fechahora = r.Fechahora,
                Totalsteam = r.Totalsteam,
                Totalgamerpay = r.Totalgamerpay,
                Totalcsfloat = r.Totalcsfloat
            }).ToList();

            Registros = new ObservableCollection<RegistroItem>(registroItems);

            _registrosView = CollectionViewSource.GetDefaultView(Registros);
            
            _registrosView.SortDescriptions.Add(new SortDescription(nameof(RegistroItem.Fechahora), ListSortDirection.Descending));
            CurrentSortColumn = RegistroSortColumn.Fecha;
            UpdateSortIndicators();
        }
        finally
        {
            IsLoading = false;
        }
    }

    [RelayCommand]
    private void SortByFecha()
    {
        SortByColumn(RegistroSortColumn.Fecha, nameof(RegistroItem.Fechahora));
    }

    [RelayCommand]
    private void SortBySteam()
    {
        SortByColumn(RegistroSortColumn.Steam, nameof(RegistroItem.Totalsteam));
    }

    [RelayCommand]
    private void SortByGamerPay()
    {
        SortByColumn(RegistroSortColumn.GamerPay, nameof(RegistroItem.Totalgamerpay));
    }

    [RelayCommand]
    private void SortByCSFloat()
    {
        SortByColumn(RegistroSortColumn.CSFloat, nameof(RegistroItem.Totalcsfloat));
    }

    private void SortByColumn(RegistroSortColumn column, string propertyName)
    {
        if (_registrosView == null) return;

        var newDirection = ListSortDirection.Ascending;

        if (CurrentSortColumn == column && CurrentSortDirection == ListSortDirection.Ascending)
            newDirection = ListSortDirection.Descending;

        CurrentSortColumn = column;
        CurrentSortDirection = newDirection;

        _registrosView.SortDescriptions.Clear();
        _registrosView.SortDescriptions.Add(new SortDescription(propertyName, newDirection));

        UpdateSortIndicators();
    }

    private void UpdateSortIndicators()
    {
        FechaSortIndicator = "";
        SteamSortIndicator = "";
        GamerPaySortIndicator = "";
        CsFloatSortIndicator = "";

        var indicator = CurrentSortDirection == ListSortDirection.Ascending ? "▲" : "▼";

        switch (CurrentSortColumn)
        {
            case RegistroSortColumn.Fecha:
                FechaSortIndicator = indicator;
                break;
            case RegistroSortColumn.Steam:
                SteamSortIndicator = indicator;
                break;
            case RegistroSortColumn.GamerPay:
                GamerPaySortIndicator = indicator;
                break;
            case RegistroSortColumn.CSFloat:
                CsFloatSortIndicator = indicator;
                break;
        }
    }

    [RelayCommand]
    private async Task VerDetallesAsync(long registroId)
    {
        IsLoading = true;

        try
        {
            var registro = _fullRegistros.FirstOrDefault(r => r.Registroid == registroId);
            if (registro == null) return;

            var itemPrecios = await _itemPrecioService.GetItemPreciosAsync(registroId);
            var userItems = await _userItemService.GetUserItemsAsync();

            var relevantUserItems = userItems
                .Where(ui => itemPrecios.Any(ip => ip.Useritemid == ui.Useritemid))
                .ToList();

            var viewModel = _serviceProvider.GetRequiredService<RegistroDetailsViewModel>();
            viewModel.Initialize(registro, relevantUserItems, itemPrecios, $"Detalles - {registro.Fechahora:dd/MM/yyyy HH:mm}");

            var detailsWindow = _serviceProvider.GetRequiredService<RegistroDetails>();
            detailsWindow.DataContext = viewModel;

            detailsWindow.ShowDialog();
        }
        finally
        {
            IsLoading = false;
        }
    }

    [RelayCommand]
    private async Task EliminarRegistroAsync(long registroId)
    {
        IsLoading = true;

        try
        {
            await _itemPrecioService.DeleteItemPreciosAsync(registroId);
            await _registroService.DeleteRegistroAsync(registroId);

            await InitializeAsync();
        }
        finally
        {
            IsLoading = false;
        }
    }
}
