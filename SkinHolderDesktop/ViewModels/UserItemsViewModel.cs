using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using SkinHolderDesktop.Models;
using SkinHolderDesktop.Services;
using System.Collections.ObjectModel;
using System.Windows;

namespace SkinHolderDesktop.ViewModels;

public partial class UserItemsViewModel : ObservableObject, IDisposable
{
    private readonly IUserItemService _userItemService;
    private readonly IItemsService _itemsService;
    private readonly GlobalViewModel _globalViewModel;
    private bool _disposed;

    private string _searchText = string.Empty;
    public string SearchText
    {
        get => _searchText;
        set { if (SetProperty(ref _searchText, value)) OnPropertyChanged(nameof(FilteredItems)); }
    }

    public IEnumerable<Item> FilteredItems
    {
        get
        {
            if (string.IsNullOrWhiteSpace(SearchText)) return Items;

            return Items.Where(i => i.Nombre?.Contains(SearchText, StringComparison.OrdinalIgnoreCase) == true);
        }
    }

    [ObservableProperty]
    private ObservableCollection<UserItemViewModel> _userItems = [];

    [ObservableProperty]
    private ObservableCollection<Item> _items = [];

    [ObservableProperty]
    public string _newItemNum = string.Empty;

    [ObservableProperty]
    public Item? _selectedFilteredItem;

    private List<UserItem> _rawUserItems = [];

    public IAsyncRelayCommand AddItemCommand { get; }

    public UserItemsViewModel(IUserItemService userItemService, IItemsService itemsService, GlobalViewModel globalViewModel)
    {
        _userItemService = userItemService;
        _itemsService = itemsService;
        _globalViewModel = globalViewModel;

        AddItemCommand = new AsyncRelayCommand(AddItem);

        _ = LoadItems();
    }

    public async Task LoadItems()
    {
        _rawUserItems = await _userItemService.GetUserItemsAsync();

        UserItems = new ObservableCollection<UserItemViewModel>(_rawUserItems.Select(u => new UserItemViewModel(u, _userItemService)));

        Items = new ObservableCollection<Item>(await _itemsService.GetItemsAsync());
        OnPropertyChanged(nameof(FilteredItems));
    }

    public void Dispose()
    {
        if (_disposed) return;

        foreach (var vm in UserItems) (vm as IDisposable)?.Dispose();

        UserItems.Clear();
        Items.Clear();

        _disposed = true;
        GC.SuppressFinalize(this);
    }

    private async Task AddItem()
    {
        if (Items.Count == 0)
        {
            MessageBox.Show("No hay items disponibles para agregar.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            return;
        }

        var selectedItem = SelectedFilteredItem;

        if (selectedItem == null)
        {
            MessageBox.Show("Es necesario seleccionar un item.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            return;
        }

        var existingUserItem = _rawUserItems.FirstOrDefault(ui => ui.Itemid == selectedItem.ItemId);

        if (existingUserItem != null)
        {
            MessageBox.Show("Ya tienes stock de este item.", "Información", MessageBoxButton.OK, MessageBoxImage.Information);
            return;
        }

        if (!int.TryParse(NewItemNum, out int cantidad) || cantidad <= 0)
        {
            MessageBox.Show("Introduce un cantidad válida", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            return;
        }

        var newUserItem = new UserItem
        {
            Useritemid = 0, 
            Itemid = selectedItem.ItemId,
            ItemName = selectedItem.Nombre,
            Cantidad = cantidad,
            Userid = _globalViewModel.UserId
        };

        var success = await _userItemService.AddUserItemAsync(newUserItem);

        if (!success)
        {
            MessageBox.Show("Error al agregar el item.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            return;
        }

        SearchText = string.Empty;

        NewItemNum = string.Empty;

        _ = LoadItems();
    }
}
