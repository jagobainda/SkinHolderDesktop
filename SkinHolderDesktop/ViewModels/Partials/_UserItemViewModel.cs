using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using SkinHolderDesktop.Models;
using SkinHolderDesktop.Services;
using System.Windows;
using System.Windows.Media;

namespace SkinHolderDesktop.ViewModels;

public partial class UserItemViewModel : ObservableObject
{
    [ObservableProperty] private string _nombre;
    [ObservableProperty] private int _cantidad;

    [ObservableProperty] private bool _isBusy;
    [ObservableProperty] private Brush _nombreForeground = Brushes.WhiteSmoke;

    private readonly IUserItemService _userItemService;

    public IRelayCommand IncrementarCommand { get; }
    public IRelayCommand DecrementarCommand { get; }
    public IAsyncRelayCommand GuardarCommand { get; }

    public UserItemViewModel(UserItem model, IUserItemService userItemService)
    {
        _nombre = model.ItemName;
        _cantidad = model.Cantidad;
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
