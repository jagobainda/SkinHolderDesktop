using CommunityToolkit.Mvvm.ComponentModel;

namespace SkinHolderDesktop.ViewModels;

public partial class MainViewModel : ObservableObject
{
    [ObservableProperty]
    private string? currentUsername;

    [ObservableProperty]
    private string? token;

    [ObservableProperty]
    private bool isAuthenticated;
}
