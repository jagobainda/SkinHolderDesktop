using SkinHolderDesktop.ViewModels;
using System.ComponentModel;
using System.IO;
using System.Runtime.InteropServices;
using System.Runtime.Versioning;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Interop;
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
        Icon = new BitmapImage(new Uri(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Images/logo_fondo.png")));

        PasswordBox.Password = _viewModel.Password;
        _viewModel.PropertyChanged += ViewModel_PropertyChanged;

        if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            Loaded += (_, _) => EnableDarkTitleBar(this);
    }

    private void PasswordBox_PasswordChanged(object sender, RoutedEventArgs e)
    {
        if (DataContext is LoginViewModel vm) if (vm.Password != ((PasswordBox)sender).Password) vm.Password = ((PasswordBox)sender).Password;
    }

    private void ViewModel_PropertyChanged(object? sender, PropertyChangedEventArgs e)
    {
        if (e.PropertyName == nameof(LoginViewModel.Password)) if (PasswordBox.Password != _viewModel.Password) PasswordBox.Password = _viewModel.Password ?? string.Empty;
    }

    [SupportedOSPlatform("windows")]
    [DllImport("dwmapi.dll")]
    private static extern int DwmSetWindowAttribute(IntPtr hwnd, int attr, ref int attrValue, int attrSize);

    private const int DWMWA_USE_IMMERSIVE_DARK_MODE = 20;

    [SupportedOSPlatform("windows")]
    public static void EnableDarkTitleBar(Window window)
    {
        var hwnd = new WindowInteropHelper(window).Handle;
        if (Environment.OSVersion.Version.Build >= 17763)
        {
            int useDarkMode = 1;
            _ = DwmSetWindowAttribute(hwnd, DWMWA_USE_IMMERSIVE_DARK_MODE, ref useDarkMode, sizeof(int));
        }
    }
}
