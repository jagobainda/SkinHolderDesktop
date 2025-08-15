using CommunityToolkit.Mvvm.ComponentModel;
using SkinHolderDesktop.Models;
using SkinHolderDesktop.Services;
using System.Collections.ObjectModel;

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
        set
        {
            if (SetProperty(ref _searchText, value))
            {
                OnPropertyChanged(nameof(FilteredItems));
            }
        }
    }

    public IEnumerable<Item> FilteredItems
    {
        get
        {
            if (string.IsNullOrWhiteSpace(SearchText))
                return Items;
            return Items.Where(i => i.Nombre?.Contains(SearchText, StringComparison.OrdinalIgnoreCase) == true);
        }
    }

    [ObservableProperty]
    private ObservableCollection<UserItemViewModel> _userItems = [];

    [ObservableProperty]
    private ObservableCollection<Item> _items = [];

    public UserItemsViewModel(IUserItemService userItemService, IItemsService itemsService, GlobalViewModel globalViewModel)
    {
        _userItemService = userItemService;
        _itemsService = itemsService;
        _globalViewModel = globalViewModel;

        _ = LoadItems();
    }

    public async Task LoadItems()
    {
        var userItems = await _userItemService.GetUserItemsAsync();

        UserItems = new ObservableCollection<UserItemViewModel>(userItems.Select(u => new UserItemViewModel(u, _userItemService)));

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
}
