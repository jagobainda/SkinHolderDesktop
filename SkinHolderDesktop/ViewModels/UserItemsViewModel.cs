using CommunityToolkit.Mvvm.ComponentModel;
using SkinHolderDesktop.Models;
using SkinHolderDesktop.Services;
using System.Collections.ObjectModel;

namespace SkinHolderDesktop.ViewModels;

public partial class UserItemsViewModel : ObservableObject
{
    private readonly IUserItemService _userItemService;
    private readonly IItemsService _itemsService;
    private readonly GlobalViewModel _globalViewModel;

    public UserItemsViewModel(IUserItemService userItemService, IItemsService itemsService, GlobalViewModel globalViewModel)
    {
        _userItemService = userItemService;
        _itemsService = itemsService;
        _globalViewModel = globalViewModel;

        _ = LoadItems();
    }

    [ObservableProperty] private ObservableCollection<UserItem> _userItems = [];
    [ObservableProperty] private ObservableCollection<Item> _items = [];

    public async Task LoadItems()
    {
        UserItems = new ObservableCollection<UserItem>(await _userItemService.GetUserItemsAsync());
        Items = new ObservableCollection<Item>(await _itemsService.GetItemsAsync());


    }
}