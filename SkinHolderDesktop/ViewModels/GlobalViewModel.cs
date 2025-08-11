using CommunityToolkit.Mvvm.ComponentModel;

namespace SkinHolderDesktop.ViewModels;

public partial class GlobalViewModel : ObservableObject
{
    [ObservableProperty]
    private string? currentUsername;

    [ObservableProperty]
    private string? token;

    [ObservableProperty] 
    private int userId;

    [ObservableProperty]
    private bool isAuthenticated;
}
