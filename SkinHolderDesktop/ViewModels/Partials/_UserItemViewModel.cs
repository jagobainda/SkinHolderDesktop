using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using SkinHolderDesktop.Models;
using SkinHolderDesktop.Services;
using System.Windows;
using System.Windows.Media;

namespace SkinHolderDesktop.ViewModels;

public partial class UserItemViewModel : ObservableObject
{
    [ObservableProperty] public partial string Nombre { get; set; }
    [ObservableProperty] public partial int Cantidad { get; set; }

    [ObservableProperty] public partial bool IsBusy { get; set; }
    [ObservableProperty] public partial Brush NombreForeground { get; set; } = Brushes.WhiteSmoke;

    private readonly IUserItemService _userItemService;

    public IRelayCommand IncrementarCommand { get; }
    public IRelayCommand DecrementarCommand { get; }
    public IAsyncRelayCommand GuardarCommand { get; }

    public UserItemViewModel(UserItem model, IUserItemService userItemService)
    {
        Nombre = model.ItemName;
        Cantidad = model.Cantidad;
        _userItemService = userItemService;

        IncrementarCommand = new RelayCommand(() => Cantidad++, () => !IsBusy);
        DecrementarCommand = new RelayCommand(() => { if (Cantidad > 0) Cantidad--; }, () => !IsBusy);

        GuardarCommand = new AsyncRelayCommand(async () =>
        {
            IsBusy = true;
            RaiseCanExecuteChanged();

            var originalBrush = NombreForeground;

            try
            {
                var success = await _userItemService.UpdateUserItemAsync(model, Cantidad);

                var primaryBrush = Application.Current.TryFindResource("PrimaryBrush") as Brush ?? Brushes.WhiteSmoke;
                NombreForeground = success ? primaryBrush : Brushes.Red;

                await Task.Delay(1000);
            }
            catch
            {
                NombreForeground = Brushes.Red;
                await Task.Delay(1000);
            }
            finally
            {
                NombreForeground = originalBrush;

                IsBusy = false;
                RaiseCanExecuteChanged();
            }
        });

    }

    private void RaiseCanExecuteChanged()
    {
        (IncrementarCommand as RelayCommand)?.NotifyCanExecuteChanged();
        (DecrementarCommand as RelayCommand)?.NotifyCanExecuteChanged();
        (GuardarCommand as AsyncRelayCommand)?.NotifyCanExecuteChanged();
    }
}
