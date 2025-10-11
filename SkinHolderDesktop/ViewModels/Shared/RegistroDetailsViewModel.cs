using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using SkinHolderDesktop.Models;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows.Data;

namespace SkinHolderDesktop.ViewModels;

public class ItemDetalle
{
    public string ItemName { get; set; } = string.Empty;
    public int Cantidad { get; set; }
    public decimal PrecioSteam { get; set; }
    public decimal PrecioGamerPay { get; set; }
    public decimal PrecioCSFloat { get; set; }
    public decimal TotalSteam => PrecioSteam * Cantidad;
    public decimal TotalGamerPay => PrecioGamerPay * Cantidad;
    public decimal TotalCSFloat => PrecioCSFloat * Cantidad;

    public string PrecioSteamFormatted => $"{PrecioSteam:F2}  €";
    public string PrecioGamerPayFormatted => $"{PrecioGamerPay:F2} €";
    public string PrecioCSFloatFormatted => $"{PrecioCSFloat:F2} €";
    public string TotalSteamFormatted => $"{TotalSteam:F2} €";
    public string TotalGamerPayFormatted => $"{TotalGamerPay:F2} €";
    public string TotalCSFloatFormatted => $"{TotalCSFloat:F2} €";
}

public enum SortColumn
{
    None,
    ItemName,
    Cantidad,
    PrecioSteam,
    PrecioGamerPay,
    PrecioCSFloat
}

public partial class RegistroDetailsViewModel : ObservableObject
{
    [ObservableProperty] private Registro registro;
    [ObservableProperty] private ObservableCollection<ItemDetalle> itemsDetalle;
    [ObservableProperty] private string title;
    [ObservableProperty] private string fechaRegistro = string.Empty;
    [ObservableProperty] private string totalSteamFormatted = "0.00€";
    [ObservableProperty] private string totalGamerPayFormatted = "0.00€";
    [ObservableProperty] private string totalCSFloatFormatted = "0.00€";

    [ObservableProperty] private SortColumn currentSortColumn = SortColumn.None;
    [ObservableProperty] private ListSortDirection currentSortDirection = ListSortDirection.Ascending;

    [ObservableProperty] private string nombreSortIndicator = "";
    [ObservableProperty] private string cantidadSortIndicator = "";
    [ObservableProperty] private string steamSortIndicator = "";
    [ObservableProperty] private string gamerPaySortIndicator = "";
    [ObservableProperty] private string csFloatSortIndicator = "";

    private ICollectionView? _itemsView;
    private List<ItemDetalle> _originalItems = new();

    public RegistroDetailsViewModel()
    {
        Registro = new Registro();
        ItemsDetalle = [];
        Title = "Detalles";
    }

    public void Initialize(Registro registro, List<UserItem> userItems, List<ItemPrecio> itemPrecios, string title)
    {
        Registro = registro;
        Title = title;
        FechaRegistro = registro.Fechahora.ToString("dd/MM/yyyy HH:mm");
        TotalSteamFormatted = $"{registro.Totalsteam:F2}€";
        TotalGamerPayFormatted = $"{registro.Totalgamerpay:F2}€";
        TotalCSFloatFormatted = $"{registro.Totalcsfloat:F2}€";

        var itemsDetalleList = userItems.Select(userItem =>
        {
            var itemPrecio = itemPrecios.FirstOrDefault(ip => ip.Useritemid == userItem.Useritemid);
            return new ItemDetalle
            {
                ItemName = userItem.ItemName,
                Cantidad = userItem.Cantidad,
                PrecioSteam = itemPrecio?.Preciosteam ?? 0m,
                PrecioGamerPay = itemPrecio?.Preciogamerpay ?? 0m,
                PrecioCSFloat = itemPrecio?.Preciocsfloat ?? 0m
            };
        }).ToList();

        ItemsDetalle = new ObservableCollection<ItemDetalle>(itemsDetalleList);
        _originalItems = [.. itemsDetalleList];

        _itemsView = CollectionViewSource.GetDefaultView(ItemsDetalle);
    }

    [RelayCommand]
    private void SortByNombre()
    {
        SortByColumn(SortColumn.ItemName, nameof(ItemDetalle.ItemName));
    }

    [RelayCommand]
    private void SortByCantidad()
    {
        SortByColumn(SortColumn.Cantidad, nameof(ItemDetalle.Cantidad));
    }

    [RelayCommand]
    private void SortBySteam()
    {
        SortByColumn(SortColumn.PrecioSteam, nameof(ItemDetalle.PrecioSteam));
    }

    [RelayCommand]
    private void SortByGamerPay()
    {
        SortByColumn(SortColumn.PrecioGamerPay, nameof(ItemDetalle.PrecioGamerPay));
    }

    [RelayCommand]
    private void SortByCSFloat()
    {
        SortByColumn(SortColumn.PrecioCSFloat, nameof(ItemDetalle.PrecioCSFloat));
    }

    private void SortByColumn(SortColumn column, string propertyName)
    {
        if (_itemsView == null) return;

        var newDirection = ListSortDirection.Ascending;
    
        if (CurrentSortColumn == column && CurrentSortDirection == ListSortDirection.Ascending) newDirection = ListSortDirection.Descending;
    
        CurrentSortColumn = column;
        CurrentSortDirection = newDirection;

        _itemsView.SortDescriptions.Clear();

        _itemsView.SortDescriptions.Add(new SortDescription(propertyName, newDirection));

        UpdateSortIndicators();
    }

    private void UpdateSortIndicators()
    {
        NombreSortIndicator = "";
        CantidadSortIndicator = "";
        SteamSortIndicator = "";
        GamerPaySortIndicator = "";
        CsFloatSortIndicator = "";

        var indicator = CurrentSortDirection == ListSortDirection.Ascending ? "▲" : "▼";

        switch (CurrentSortColumn)
        {
            case SortColumn.ItemName:
                NombreSortIndicator = indicator;
                break;
            case SortColumn.Cantidad:
                CantidadSortIndicator = indicator;
                break;
            case SortColumn.PrecioSteam:
                SteamSortIndicator = indicator;
                break;
            case SortColumn.PrecioGamerPay:
                GamerPaySortIndicator = indicator;
                break;
            case SortColumn.PrecioCSFloat:
                CsFloatSortIndicator = indicator;
                break;
        }
    }
}