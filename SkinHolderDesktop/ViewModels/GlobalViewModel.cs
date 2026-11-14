using CommunityToolkit.Mvvm.ComponentModel;
using SkinHolderDesktop.Core;

namespace SkinHolderDesktop.ViewModels;

public partial class GlobalViewModel : ObservableObject, IAuthSession
{
    [ObservableProperty]
    public partial string? CurrentUsername { get; set; }

    [ObservableProperty]
    public partial string? Token { get; set; }

    [ObservableProperty]
    public partial int UserId { get; set; }

    [ObservableProperty]
    public partial bool IsAuthenticated { get; set; }
}
