using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using SkinHolderDesktop.Models;
using System.Windows;

namespace SkinHolderDesktop.ViewModels;

public partial class UserItemViewModel : ObservableObject
{
    [ObservableProperty] private string _nombre;
    [ObservableProperty] private int _cantidad;

    public IRelayCommand IncrementarCommand { get; }
    public IRelayCommand DecrementarCommand { get; }
    public IRelayCommand GuardarCommand { get; }

    public UserItemViewModel(UserItem model)
    {
        _nombre = model.ItemName;
        _cantidad = model.Cantidad;

        IncrementarCommand = new RelayCommand(() => Cantidad++);
        DecrementarCommand = new RelayCommand(() => { if (Cantidad > 0) Cantidad--; });
        GuardarCommand = new RelayCommand(() =>
        {
            MessageBox.Show($"Guardando {Cantidad} unidades de {Nombre}.");
        });
    }
}