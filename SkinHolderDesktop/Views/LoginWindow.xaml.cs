using SkinHolderDesktop.ViewModels;
using System.ComponentModel;
using System.IO;
using System.Net.Http;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
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

        PasswordBox.Password = _viewModel.Password;
        _viewModel.PropertyChanged += ViewModel_PropertyChanged;

        Loaded += async (_, _) => await LoadLogoAsync();
    }

    private static readonly HttpClient _httpClient = new();
    private const string BgImgUrl = "https://cdn.jagoba.dev/imgs/bg_login.png";
    private const string IconUrl = "https://cdn.jagoba.dev/imgs/logo.ico";

    private async Task LoadLogoAsync()
    {
        try
        {
            var bytes = await _httpClient.GetByteArrayAsync(BgImgUrl);
            using var stream = new MemoryStream(bytes);
            var bitmap = new BitmapImage();
            bitmap.BeginInit();
            bitmap.CacheOption = BitmapCacheOption.OnLoad;
            bitmap.StreamSource = stream;
            bitmap.EndInit();
            bitmap.Freeze();
            Logo.Source = bitmap;
        }
        catch { }

        try
        {
            var bytes = await _httpClient.GetByteArrayAsync(IconUrl);
            using var stream = new MemoryStream(bytes);
            var bitmap = new BitmapImage();
            bitmap.BeginInit();
            bitmap.CacheOption = BitmapCacheOption.OnLoad;
            bitmap.StreamSource = stream;
            bitmap.EndInit();
            bitmap.Freeze();
            TitleBarIcon.Source = bitmap;
        }
        catch { }
    }

    private void PasswordBox_PasswordChanged(object sender, RoutedEventArgs e)
    {
        if (DataContext is LoginViewModel vm) if (vm.Password != ((PasswordBox)sender).Password) vm.Password = ((PasswordBox)sender).Password;
    }

    private void ViewModel_PropertyChanged(object? sender, PropertyChangedEventArgs e)
    {
        if (e.PropertyName == nameof(LoginViewModel.Password)) if (PasswordBox.Password != _viewModel.Password) PasswordBox.Password = _viewModel.Password ?? string.Empty;
    }

    private void TitleBar_MouseLeftButtonDown(object sender, MouseButtonEventArgs e) => DragMove();

    private void BtnMinimize_Click(object sender, RoutedEventArgs e) => WindowState = WindowState.Minimized;

    private void BtnClose_Click(object sender, RoutedEventArgs e) => Close();
}
