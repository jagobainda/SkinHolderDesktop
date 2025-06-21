using SkinHolderDesktop.ViewModels;
using System.ComponentModel;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;

namespace SkinHolderDesktop.Views;

public partial class LoginWindow : Window
{
    private readonly LoginViewModel _viewModel;

    public LoginWindow(LoginViewModel viewModel)
    {
        InitializeComponent();
        DataContext = viewModel;
        _viewModel = viewModel;

        Logo.Source = new BitmapImage(new Uri(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Images/icono.png")));
        Icon = new BitmapImage(new Uri(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Images/icono.png")));

        PasswordBox.Password = _viewModel.Password;

        _viewModel.PropertyChanged += ViewModel_PropertyChanged;
    }

    private void PasswordBox_PasswordChanged(object sender, RoutedEventArgs e)
    {
        if (DataContext is LoginViewModel vm) if (vm.Password != ((PasswordBox)sender).Password) vm.Password = ((PasswordBox)sender).Password;
    }

    private void ViewModel_PropertyChanged(object? sender, PropertyChangedEventArgs e)
    {
        if (e.PropertyName == nameof(LoginViewModel.Password)) if (PasswordBox.Password != _viewModel.Password) PasswordBox.Password = _viewModel.Password ?? string.Empty;
    }
}