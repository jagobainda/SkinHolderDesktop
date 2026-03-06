using CommunityToolkit.Mvvm.ComponentModel;
using SkinHolderDesktop.Core;

namespace SkinHolderDesktop.ViewModels;

public partial class GlobalViewModel : ObservableObject, IAuthSession
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
